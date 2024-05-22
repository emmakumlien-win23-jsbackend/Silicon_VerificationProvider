using System.ComponentModel.DataAnnotations;

namespace Silicon_VerificationProvider.Data.Entities;

public class VerificaionRequestEntity
{
    [Key]
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMinutes(5);
}
