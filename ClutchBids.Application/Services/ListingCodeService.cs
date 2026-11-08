using AutoMapper;
using ClutchBids.Domain.DTOs.ListingCode;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of listing code service for managing listing codes
/// </summary>
public class ListingCodeService : IListingCodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ListingCodeService> _logger;
    private readonly IMapper _mapper;

    public ListingCodeService(IUnitOfWork unitOfWork, ILogger<ListingCodeService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> GenerateCodesAsync(long adminUserId, int count)
    {
        try
        {
            var listingCodes = new List<ListingCode>();

            for (int i = 0; i < count; i++)
            {
                var code = GenerateUniqueCode();

                listingCodes.Add(new ListingCode
                {
                    Code = code,
                    IsUsed = false,
                    CreatedByUserId = adminUserId
                });
            }

            await _unitOfWork.ListingCodes.AddRangeAsync(listingCodes);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Generated {count} listing codes by admin {adminUserId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating listing codes: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ValidateAndUseCodeAsync(string code, long listingId)
    {
        var codes = await _unitOfWork.ListingCodes.FindAsync(lc => lc.Code == code && !lc.IsUsed);
        var listingCode = codes.FirstOrDefault();

        if (listingCode == null)
        {
            _logger.LogWarning($"Invalid or already used code: {code}");
            return false;
        }

        listingCode.IsUsed = true;
        listingCode.UsedAt = DateTime.UtcNow;
        listingCode.UsedByListingId = listingId;

        await _unitOfWork.ListingCodes.UpdateAsync(listingCode);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Code {code} used for listing {listingId}");
        return true;
    }

    public async Task<IEnumerable<ListingCodeDto>> GetUnusedCodesAsync()
    {
        var codes = await _unitOfWork.ListingCodes.FindAsync(lc => !lc.IsUsed);
        return _mapper.Map<IEnumerable<ListingCodeDto>>(codes);
    }

    public async Task<IEnumerable<ListingCodeDto>> GetAllCodesAsync()
    {
        var codes = await _unitOfWork.ListingCodes.GetAllAsync();
        return _mapper.Map<IEnumerable<ListingCodeDto>>(codes);
    }

    private static string GenerateUniqueCode()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
    }
}
