using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Models;
using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CatImage> CatImages { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CatImage>()
                .HasMany(ci => ci.Tags)
                .WithMany(t => t.CatImages)
                .UsingEntity(j => j.ToTable("CatImageTags"));

            builder.Entity<Comment>()
                .HasOne(c => c.CatImage)
                .WithMany(ci => ci.Comments)
                .HasForeignKey(c => c.CatImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vote>()
                .HasOne(v => v.CatImage)
                .WithMany(ci => ci.Votes)
                .HasForeignKey(v => v.CatImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.PostedBy)
                .WithMany()
                .HasForeignKey(c => c.PostedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Roles

            builder.Entity<IdentityRole>().HasData(
        new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
        new IdentityRole { Id = "2", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
    );

        }
    }
}