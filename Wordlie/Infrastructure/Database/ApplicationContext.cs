using Microsoft.EntityFrameworkCore;
using Wordlie.Models;

namespace Wordlie.Infrastructure.Database;

public class ApplicationContext : DbContext
{
    public DbSet<WordModel> Words { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }
}