using DeliveryServiceApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DeliveryServiceApp.Persistence.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Point> Points { get; set; }

        public DbSet<Step> Steps { get; set; }

        public DbSet<RouteBase> RouteBases { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Table "Role"
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Role>().HasKey(p => p.Name);
            modelBuilder.Entity<Role>().HasIndex(p => p.Id).IsUnique();
            modelBuilder.Entity<Role>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>().HasData
            (
                new Role { Id = 0, Name = "admin" },
                new Role { Id = 1, Name = "basic" }
            );
            #endregion

            #region Table "User"
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(p => p.Id);
            modelBuilder.Entity<User>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().HasIndex(p => p.Username).IsUnique();
            modelBuilder.Entity<User>().Property(p => p.Username).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.PasswordHash).IsRequired();
            modelBuilder.Entity<User>().Property(p => p.PasswordSalt).IsRequired();

            CreatePasswordHash("12345", out byte[] passHash, out byte[] passSalt);
            modelBuilder.Entity<User>().HasData
            (
                new User
                {
                    Id = 1,
                    Username = "nuno.admin",
                    FirstName = "Nuno",
                    LastName = "Admin",
                    RoleName = "admin",
                    PasswordHash = passHash,
                    PasswordSalt = passSalt
                }
            );
            #endregion

            #region Table "Point"
            modelBuilder.Entity<Point>().ToTable("Point");
            modelBuilder.Entity<Point>().HasKey(p => p.Id);
            modelBuilder.Entity<Point>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Point>().HasIndex(p => p.Description).IsUnique();
            modelBuilder.Entity<Point>().Property(p => p.Description).IsRequired();

            modelBuilder.Entity<Point>().HasData
            (
                new Point { Id = 1, Description = "A" },
                new Point { Id = 2, Description = "B" },
                new Point { Id = 3, Description = "C" },
                new Point { Id = 4, Description = "D" },
                new Point { Id = 5, Description = "E" },
                new Point { Id = 6, Description = "F" },
                new Point { Id = 7, Description = "G" },
                new Point { Id = 8, Description = "H" },
                new Point { Id = 9, Description = "I" }
            );
            #endregion

            #region Table "RouteBase"
            modelBuilder.Entity<RouteBase>().ToTable("RouteBase");
            modelBuilder.Entity<RouteBase>().HasKey(p => p.Id);
            modelBuilder.Entity<RouteBase>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<RouteBase>().HasOne(r => r.Origin).WithMany().HasForeignKey("OriginId").IsRequired();
            modelBuilder.Entity<RouteBase>().HasOne(r => r.Destination).WithMany().HasForeignKey("DestinationId").IsRequired();

            modelBuilder.Entity<RouteBase>().HasData
            (
                new RouteBase
                {
                    Id = 1,
                    OriginId = 1,
                    DestinationId = 2
                }
            );
            #endregion

            #region Table "Step"
            modelBuilder.Entity<Step>().ToTable("Step");
            modelBuilder.Entity<Step>().HasKey(p => p.Id);
            modelBuilder.Entity<Step>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Step>().Property(p => p.Time).IsRequired();
            modelBuilder.Entity<Step>().Property(p => p.Cost).IsRequired();

            modelBuilder.Entity<Step>().HasOne(s => s.Start).WithMany().HasForeignKey("StartId").IsRequired();
            modelBuilder.Entity<Step>().HasOne(s => s.End).WithMany().HasForeignKey("EndId").IsRequired();

            modelBuilder.Entity<Step>().HasData
            (
                new Step { Id = 1, StartId = 1, EndId = 3, Time = 1, Cost = 20 },
                new Step { Id = 2, StartId = 1, EndId = 5, Time = 30, Cost = 5 },
                new Step { Id = 3, StartId = 1, EndId = 8, Time = 10, Cost = 1 },
                new Step { Id = 4, StartId = 3, EndId = 2, Time = 1, Cost = 12 },
                new Step { Id = 5, StartId = 4, EndId = 6, Time = 4, Cost = 50 },
                new Step { Id = 6, StartId = 5, EndId = 4, Time = 3, Cost = 5 },
                new Step { Id = 7, StartId = 6, EndId = 7, Time = 40, Cost = 50 },
                new Step { Id = 8, StartId = 6, EndId = 9, Time = 45, Cost = 50 },
                new Step { Id = 9, StartId = 7, EndId = 2, Time = 64, Cost = 73 },
                new Step { Id = 10, StartId = 8, EndId = 5, Time = 30, Cost = 1 },
                new Step { Id = 11, StartId = 9, EndId = 2, Time = 65, Cost = 5 }
            );
            #endregion
        }


        #region Private methods

        /// <summary>Creates the password hash.</summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <param name="passwordSalt">The password salt.</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        #endregion
    }
}
