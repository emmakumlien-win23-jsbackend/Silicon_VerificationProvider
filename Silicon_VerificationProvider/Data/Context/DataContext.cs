using Microsoft.EntityFrameworkCore;
using Silicon_VerificationProvider.Data.Entities;

namespace Silicon_VerificationProvider.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
   public DbSet<VerificaionRequestEntity> VerificaionRequest {  get; set; }
}
