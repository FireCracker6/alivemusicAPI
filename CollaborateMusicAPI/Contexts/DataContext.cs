using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Contexts;

public class DataContext : DbContext
{
    protected DataContext()
    {
    }
    public DataContext(DbContextOptions options) : base(options)
    {
    }

   
}
