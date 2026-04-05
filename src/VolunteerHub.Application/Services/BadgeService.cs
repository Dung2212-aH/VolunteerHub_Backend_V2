
using VolunteerHub.Application.Abstractions;
using VolunteerHub.Contracts.Recognition;
using VolunteerHub.Application.Common;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class BadgeService : IBadgeService
{
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public BadgeService(
        IRecognitionRepository recognitionRepository, 
        IAttendanceRepository attendanceRepository,
        IVolunteerProfileRepository profileRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _recognitionRepository = recognitionRepository;
        _attendanceRepository = attendanceRepository;
        _profileRepository = profileRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> EvaluateBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activeBadges = await _recognitionRepository.GetAllActiveBadgesAsync(cancellationToken);
            if (!activeBadges.Any()) return Result.Success();

            var certificateCount = await _recognitionRepository.GetActiveCertificateCountAsync(volunteerProfileId, cancellationToken);
            var totalApprovedHours = await _attendanceRepository.GetTotalApprovedHoursAsync(volunteerProfileId, cancellationToken);

            foreach (var badge in activeBadges)
            {
                bool eligible = EvaluateRule(badge.Code, certificateCount, totalApprovedHours);
                if (eligible)
                {
                    var existing = await _recognitionRepository.GetVolunteerBadgeAsync(volunteerProfileId, badge.Id, cancellationToken);
                    if (existing == null)
                    {
                        var award = new VolunteerBadge
                        {
                            VolunteerProfileId = volunteerProfileId,
                            BadgeId = badge.Id,
                            AwardReason = "System evaluated",
                            SourceReference = "System"
                        };
                        _recognitionRepository.AddVolunteerBadge(award);

                        // Trigger notification — resolve profileId to Identity userId
                        var profile = await _profileRepository.GetByIdWithDetailsAsync(volunteerProfileId, cancellationToken);
                        if (profile != null)
                        {
                            await _notificationService.NotifyBadgeAwardedAsync(
                                profile.UserId, badge.Name, badge.Id, cancellationToken);
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(new Error("Badge.EvaluationFailed", "Badge evaluation process failed."));
        }
    }

    private bool EvaluateRule(string badgeCode, int certificates, double hours)
    {
        return badgeCode switch
        {
            "FIRST_STEP" => certificates >= 1,
            "HELPING_HAND" => certificates >= 3,
            "CONSISTENT_VOLUNTEER" => hours >= 20,
            "COMMUNITY_CHAMPION" => certificates >= 10,
            _ => false
        };
    }

    public async Task<Result<List<BadgeResponse>>> GetMyBadgesAsync(Guid volunteerProfileId, CancellationToken cancellationToken = default)
    {
        var badges = await _recognitionRepository.GetMyBadgesAsync(volunteerProfileId, cancellationToken);
        var response = badges.Select(b => new BadgeResponse
        {
            Id = b.Id,
            Code = b.Badge.Code,
            Name = b.Badge.Name,
            Description = b.Badge.Description,
            AwardedAt = b.AwardedAt,
            AwardReason = b.AwardReason
        }).ToList();
        
        return Result.Success(response);
    }
}
