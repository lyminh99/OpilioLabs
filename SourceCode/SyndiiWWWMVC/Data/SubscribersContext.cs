using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndiiWWWMVC.Models;
using Microsoft.EntityFrameworkCore;
namespace SyndiiWWWMVC.Data
{
    /// <summary>
    /// Database context
    /// </summary>
    public class SubscribersContext : DbContext
    {
        public SubscribersContext(DbContextOptions<SubscribersContext> options) : base(options)
        {
        }
        public DbSet<Subscriber> Subscribers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscriber>().ToTable("Subscriber");

        }
    }
}
