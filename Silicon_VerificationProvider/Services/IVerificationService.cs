using Azure.Messaging.ServiceBus;
using Silicon_VerificationProvider.Models;

namespace Silicon_VerificationProvider.Services
{
    public interface IVerificationService
    {
        string GenerateCode();
        EmailRequestModel GenerateEmailRequest(VerificationRequestModel verificationRequest, string code);
        string GenerateServiceBusEmailRequest(EmailRequestModel emailRequest);
        Task<bool> SaveVerficationRequest(VerificationRequestModel verificationRequest, string code);
        VerificationRequestModel UnpackVerificationRequest(ServiceBusReceivedMessage message);
    }
}