using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PersonApi_sync_REST
{
    /// <summary>
    /// визначає контекст бази даних (є основним класом, що координує функціональні можливості Entity Framework для моделі даних.)
    /// </summary>
    public class PersonDB(DbContextOptions<PersonDB> options) : DbContext(options)
    {
        public DbSet<PersonItem> Persons => Set<PersonItem>();

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonItem>().Property(b => b.Unzr).IsRequired(required: true);
            modelBuilder.Entity<PersonItem>().Property(b => b.LastUpdated).ValueGeneratedOnAddOrUpdate().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }
        #endregion
    }
}
