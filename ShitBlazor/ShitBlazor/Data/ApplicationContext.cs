using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShitBlazor.Data
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Movie_bd> Movies { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);        
        }
            
    }

    public class ApplicationContext2 : DbContext
    {
        public ApplicationContext2(DbContextOptions options) : base(options) { }

        public DbSet<Movie2> Movies { get; set; }
        public DbSet<Top10> Top10 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
