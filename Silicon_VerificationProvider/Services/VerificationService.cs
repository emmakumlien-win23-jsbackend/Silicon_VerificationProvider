﻿using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Silicon_VerificationProvider.Data.Context;
using Silicon_VerificationProvider.Models;


namespace Silicon_VerificationProvider.Services;
public class VerificationService(ILogger<VerificationService> logger, IServiceProvider serviceProvider) : IVerificationService
{

    private readonly ILogger<VerificationService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;



    public VerificationRequestModel UnpackVerificationRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var verificationRequest = JsonConvert.DeserializeObject<VerificationRequestModel>(message.Body.ToString());
            if (verificationRequest != null && !string.IsNullOrEmpty(verificationRequest.Email))
            {
                return verificationRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: GenerateVerificationCode.UnpackVerificationRequest :: {ex.Message}");
        }
        return null!;
    }
    public string GenerateCode()
    {
        try
        {
            var rnd = new Random();
            var code = rnd.Next(100000, 999999);
            return code.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: GenerateVerificationCode.GenerateCode :: {ex.Message}");
        }
        return null!;
    }
    public async Task<bool> SaveVerficationRequest(VerificationRequestModel verificationRequest, string code)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DataContext>();

            var existingRequest = await context.VerificaionRequest.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
            if (existingRequest != null)
            {
                existingRequest.Code = code;
                existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                context.Entry(existingRequest).State = EntityState.Modified;

            }
            else
            {
                context.VerificaionRequest.Add(new Data.Entities.VerificaionRequestEntity
                {
                    Code = code,
                    Email = verificationRequest.Email


                });
                await context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: GenerateVerificationCode.SaveVerficationRequest :: {ex.Message}");
        }
        return false;
    }

    public EmailRequestModel GenerateEmailRequest(VerificationRequestModel verificationRequest, string code)
    {
        try
        {
            if (!string.IsNullOrEmpty(verificationRequest.Email) && !string.IsNullOrEmpty(code))
            {
                var emailRequest = new EmailRequestModel()
                {
                    To = verificationRequest.Email,
                    Subject = $"Verification code {code}",
                    HtmlBody = @$"
                                <html lang='en'>
                                <head>
                                <meta charset='UTF-8'>
                                <meta name ='viewport' content='width=device-width, initial-scale=1.0' >
                                <title>Verification Code</title>
                                </head>

                                <body>
                                    <div style='color: #191919; max-width:500px'>
                                        <div style='background-color:#4F85F6; color: white; text-align: center; padding:20px 0'>
                                            <h1 style='font-weight:400'>Verification Code</h1>
                                        </div>
    
    
                                        <div style='background-color:#f4f4f4; padding:1rem 2rem'>
                                            <p>Dear user</p>
                                            <p>We recieved a request to sign in to your account using you email address {verificationRequest.Email}. Please verify your account by using the code below</p>
                                            <p style='font-weight:700; text-align:center; font-size:48px'>{code}</p>
                                            <div>
                                                <p style='color:#191919; font-Size:11px;'>If you did not request a sign in code it might be someone trying to log in on your account. Please contact userservice</p>
                                            </div>
                                        </div>
                                    </div>
                                </body>


                                </html>

                                ",
                    PlainText = $"We recieved a request to sign in to your account using you email address: {verificationRequest.Email}. Please verify your account by using the code:{code}. If you did not request a sign in code it might be someone trying to log in on your account. Please contact userservice"

                };
                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: GenerateVerificationCode.GenerateEmailRequest :: {ex.Message}");
        }
        return null!;
    }

    public string GenerateServiceBusEmailRequest(EmailRequestModel emailRequest)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
            {
                return payload;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: GenerateVerificationCode.GenerateServiceBusEmailRequest :: {ex.Message}");
        }
        return null!;
    }
}
