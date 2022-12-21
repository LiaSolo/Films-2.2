using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Films_2._2
{
    //internal class ApplicationContext : DbContext
    //{
    //    //public DbSet<User> Users => Set<User>();

    //    public DbSet<Course> Courses { get; set; }
    //    public DbSet<User> Students { get; set; }
    //    public DbSet<SetFriends> Friends { get; set; }
    //    public ApplicationContext() => Database.EnsureCreated();

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=1"); // postgre
    //    }
    //}

    internal class ApplicationContext2 : DbContext
    {
        //public DbSet<User> Users => Set<User>();

        public DbSet<User1> Students1 { get; set; }
        public DbSet<SetFriends> Friends { get; set; }
        public ApplicationContext2() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test2;Username=postgres;Password=1"); // postgre
        }
    }
}
