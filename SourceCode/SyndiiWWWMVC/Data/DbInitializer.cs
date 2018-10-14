using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndiiWWWMVC.Models;
namespace SyndiiWWWMVC.Data
{
    /// <summary>
    /// Initalizer for database
    /// </summary>
    public class DbInitializer
    {
        public static void Initialize(SubscribersContext context)
        {
            context.Database.EnsureCreated();
            if (context.Subscribers.Any())
            {
                return;   // DB has been seeded
            }
            var subscirbers = new Subscriber[]
            {
                new Subscriber{Email="lyminh99@gmail.com",DateAdded=DateTime.Parse("2005-09-01"),DateRemoved=DateTime.Parse("2014-09-01"),IsActive=false},
                new Subscriber{Email="lyminh98@gmail.com",DateAdded=DateTime.Parse("2005-09-01"),DateRemoved= null,IsActive=true}

            };
            foreach (Subscriber s in subscirbers)
            {
                context.Subscribers.Add(s);
            }
            context.SaveChanges();
        }
    }
}
