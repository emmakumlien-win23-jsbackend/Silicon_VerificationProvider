using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Silicon_VerificationProvider.Data.Context;
using Silicon_VerificationProvider.Functions;
using Silicon_VerificationProvider.Models;

namespace Silicon_VerificationProvider.Services;

public class ValidateVerificationCodeService(ILogger<ValidateVerificationCodeService> logger, DataContext context) : IValidateVerificationCodeService
{
    private readonly ILogger<ValidateVerificationCodeService> _logger = logger;
    private readonly DataContext _context = context;

    public async Task<ValidateRequestModel> UnpackValidateRequestAsync(HttpRequest req)
    {

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var validateRequest = JsonConvert.DeserializeObject<ValidateRequestModel>(body);
                if (validateRequest != null)
                {
                    return validateRequest;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: ValidateVerificationCode.UnpackValidateRequestAsync :: {ex.Message}");
        }
        return null!;
    }
    public async Task<bool> ValidateCodeAsync(ValidateRequestModel validateRequest)
    {
        try
        {
            var entity = await _context.VerificaionRequest.FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);
            if (entity != null)
            {
                _context.VerificaionRequest.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: ValidateVerificationCode.ValidateCodeAsync :: {ex.Message}");
        }
        return false;
    }
}
