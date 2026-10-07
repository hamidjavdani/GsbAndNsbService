using GSB.Test.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GSB.Test.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SanadCallback> SanadCallbacks { get; set; }

    public DbSet<UniqueIdentifierCallback> UniqueIdentifierCallbacks { get; set; }
}
