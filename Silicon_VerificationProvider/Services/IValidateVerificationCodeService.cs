using Microsoft.AspNetCore.Http;
using Silicon_VerificationProvider.Models;

namespace Silicon_VerificationProvider.Services
{
    public interface IValidateVerificationCodeService
    {
        Task<ValidateRequestModel> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequestModel validateRequest);
    }
}