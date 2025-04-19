using Microsoft.EntityFrameworkCore;

namespace ResourceCreatorFunction.Data
{
    public class ResourceCreatorDbContext : DbContext
    {
        public ResourceCreatorDbContext(DbContextOptions<ResourceCreatorDbContext> options) : base(options) { }

        // Define a DbSet for your entities
        //public DbSet<Resource> Resources { get; set; }
    }

    // Example entity class
    //public class Resource
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //}
}