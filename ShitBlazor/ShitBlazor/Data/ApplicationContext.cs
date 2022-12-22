﻿using System;
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
            base.OnModelCreating(modelBuilder); ;
        
        }
            
    }

    //internal class ApplicationContext2 : DbContext
    //{
    //    public DbSet<Movie2> Movies { get; set; }
    //    public DbSet<Top10> Top10 { get; set; }

    //    public ApplicationContext2() => Database.EnsureCreated();

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=hellbitchtop;Username=postgres;Password=1"); // postgre
    //    }
    //}
}