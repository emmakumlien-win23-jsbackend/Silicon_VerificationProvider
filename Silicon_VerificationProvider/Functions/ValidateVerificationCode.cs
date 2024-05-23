using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Silicon_VerificationProvider.Services;

namespace Silicon_VerificationProvider.Functions;

public class ValidateVerificationCode(ILogger<ValidateVerificationCode> logger, IValidateVerificationCodeService validateCodeService)
{
    private readonly ILogger<ValidateVerificationCode> _logger = logger;
    private readonly IValidateVerificationCodeService _validateCodeService = validateCodeService;

    [Function("ValidateVerificationCode")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "validate")] HttpRequest req)
    {

        try
        {
            var validateRequest = await _validateCodeService.UnpackValidateRequestAsync(req);
            if (validateRequest != null)
            {
                var validateResult = await _validateCodeService.ValidateCodeAsync(validateRequest);
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

}
