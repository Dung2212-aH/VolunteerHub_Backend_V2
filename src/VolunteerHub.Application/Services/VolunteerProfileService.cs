using VolunteerHub.Application.Abstractions;
using VolunteerHub.Application.Common;
using VolunteerHub.Contracts.Requests;
using VolunteerHub.Contracts.Responses;
using VolunteerHub.Domain.Entities;

namespace VolunteerHub.Application.Services;

public class VolunteerProfileService : IVolunteerProfileService
{
    private readonly IVolunteerProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VolunteerProfileService(
        IVolunteerProfileRepository profileRepository, 
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VolunteerProfileResponse>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure<VolunteerProfileResponse>(Error.NotFound);

        return Result.Success(MapToResponse(profile));
    }

    public async Task<Result> CreateProfileAsync(Guid userId, CreateProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (await _profileRepository.ExistsForUserAsync(userId, cancellationToken))
            return Result.Failure(Error.ProfileAlreadyExists);

        var profile = new VolunteerProfile
        {
            UserId = userId,
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Bio = request.Bio,
            BloodGroup = request.BloodGroup,
            Avatar = request.Avatar
        };

        UpdateCollections(profile, request.Skills, request.Languages, request.Interests);

        _profileRepository.Add(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);
        if (profile == null)
            return Result.Failure(Error.NotFound);

        profile.FullName = request.FullName;
        profile.Phone = request.Phone;
        profile.Address = request.Address;
        profile.Latitude = request.Latitude;
        profile.Longitude = request.Longitude;
        profile.Bio = request.Bio;
        profile.BloodGroup = request.BloodGroup;
        profile.Avatar = request.Avatar;

        UpdateCollections(profile, request.Skills, request.Languages, request.Interests);

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private void UpdateCollections(VolunteerProfile profile, List<string> skills, List<string> languages, List<string> interests)
    {
        profile.Skills.Clear();
        foreach (var name in skills.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            profile.Skills.Add(new VolunteerSkill { Name = name, VolunteerProfileId = profile.Id });
        }

        profile.Languages.Clear();
        foreach (var name in languages.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            profile.Languages.Add(new VolunteerLanguage { Name = name, VolunteerProfileId = profile.Id });
        }

        profile.Interests.Clear();
        foreach (var name in interests.Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            profile.Interests.Add(new VolunteerInterest { Name = name, VolunteerProfileId = profile.Id });
        }
    }

    private VolunteerProfileResponse MapToResponse(VolunteerProfile profile)
    {
        return new VolunteerProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.FullName,
            Phone = profile.Phone,
            Address = profile.Address,
            Latitude = profile.Latitude,
            Longitude = profile.Longitude,
            Bio = profile.Bio,
            BloodGroup = profile.BloodGroup,
            Avatar = profile.Avatar,
            TotalVolunteerHours = profile.TotalVolunteerHours,
            IsProfileComplete = CalculateProfileCompleteness(profile),
            Skills = profile.Skills.Select(s => s.Name).ToList(),
            Languages = profile.Languages.Select(l => l.Name).ToList(),
            Interests = profile.Interests.Select(i => i.Name).ToList()
        };
    }

    private bool CalculateProfileCompleteness(VolunteerProfile profile)
    {
        return !string.IsNullOrWhiteSpace(profile.FullName) &&
               !string.IsNullOrWhiteSpace(profile.Phone) &&
               !string.IsNullOrWhiteSpace(profile.Address) &&
               !string.IsNullOrWhiteSpace(profile.Bio) &&
               profile.Skills.Any() &&
               profile.Languages.Any() &&
               profile.Interests.Any();
    }
}
