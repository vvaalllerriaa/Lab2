using Microsoft.EntityFrameworkCore;
using Theatre.DAL.Entities;

namespace Theatre.DAL;

public class TheatreDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Performance> Performances => Set<Performance>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public TheatreDbContext(DbContextOptions<TheatreDbContext> options) : base(options)
    {
    }
}