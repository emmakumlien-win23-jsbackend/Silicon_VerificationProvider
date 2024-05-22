using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Silicon_VerificationProvider.Data.Context;
using Silicon_VerificationProvider.Functions;

namespace Silicon_VerificationProvider.Services;

public class VerificationCleanerService(ILogger<VerificationCleanerService> logger, DataContext context) : IVerificationCleanerService
{
    private readonly ILogger<VerificationCleanerService> _logger = logger;
    private readonly DataContext _context = context;

    public async Task RemoveExpiredRecordsAsync()
    {
        try
        {
            var expired = await _context.VerificaionRequest.Where(x => x.ExpiryDate >= DateTime.Now).ToListAsync();
            _context.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: VerificationCleanerService.RemoveExpiredRecordsAsync :: {ex.Message}");
        }
    }
}
