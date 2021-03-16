using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        //for likes - table
        public DbSet<UserLike> Likes {get; set;}

        //give the entites some configuration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //bc we overriding this method we'll just pass in to class that we're driving from
            base.OnModelCreating(builder);
            //user like entity
            builder.Entity<UserLike>()
            //forming PRIMARY KEY for this table
                .HasKey(k => new {k.SourceUserId, k.LikedUserId});

                //configure relationships 
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                //sourceUser can like many other users
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                //if we delete user, we delete realted entity also
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}