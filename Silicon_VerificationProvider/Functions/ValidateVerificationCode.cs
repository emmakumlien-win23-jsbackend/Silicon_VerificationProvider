using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Silicon_VerificationProvider.Data.Context;
using Silicon_VerificationProvider.Models;

namespace Silicon_VerificationProvider.Functions;

public class ValidateVerificationCode(ILogger<ValidateVerificationCode> logger, DataContext context)
{
    private readonly ILogger<ValidateVerificationCode> _logger = logger;
    private readonly DataContext _context = context;

    [Function("ValidateVerificationCode")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route ="validate")] HttpRequest req)
    {

        try
        {
            var validateRequest = await UnpackValidateRequestAsync(req);
            if(validateRequest != null)
            {
                var validateResult = await ValidateCodeAsync(validateRequest);
                if (validateResult)
                {
                    return new OkResult();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: ValidateVerificationCode.Run :: {ex.Message}");
        }
        
        return new UnauthorizedResult();
    }



    public async Task<ValidateRequestModel> UnpackValidateRequestAsync(HttpRequest req)
    {

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if(!string.IsNullOrEmpty(body))
            {
                var validateRequest = JsonConvert.DeserializeObject<ValidateRequestModel>(body);
                if(validateRequest != null)
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
            var entity = await _context.VerificaionRequest.FirstOrDefaultAsync(x=> x.Email == validateRequest.Email && x.Code == validateRequest.Code);
            if(entity != null)
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
