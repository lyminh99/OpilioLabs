using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SyndiiWWWMVC.Data;
using SyndiiWWWMVC.Models;


/// <summary>
/// Subscriber controller
/// </summary>
namespace SyndiiWWWMVC.Controllers
{
    /// <summary>
    /// The controller for subscriber page, logic and system
    /// </summary>
    public class SubscribersController : Controller
    {
        // Setting for the email variable
        private readonly EmailSettings _emailSettings;
        // The context model we use for the controller
        private readonly SubscribersContext _context;
        // Session string
        const string SessionLoggedIn = "false";

        /// <summary>
        /// Initialize the setting
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        public SubscribersController(SubscribersContext context, IOptions<EmailSettings> settings)
        {
            _context = context;
            _emailSettings = settings.Value;
        }

        //// GET: Subscribers
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Subscribers.ToListAsync());
        //}

        // GET: Subscribers/Details/5
        /// <summary>
        /// Detail logic to show user information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (subscriber == null)
            {
                return NotFound();
            }

            return View(subscriber);
        }

        /// <summary>
        /// Login page, simply return a view
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login logic to set the session for logged in
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken, ActionName("Login")]
        public IActionResult Login(string userName, string password)
        {
            if (userName == "admin" && password == "password")
            {
                HttpContext.Session.SetString(SessionLoggedIn, "true");
                return RedirectToAction(nameof(Index), "Subscribers");
            }
            else
            {
                HttpContext.Session.SetString(SessionLoggedIn, "false");

                return RedirectToAction(nameof(Index), "Home");
            }

        }

        /// <summary>
        /// Index page of the subscribers page, only can see if the user logged in admin
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="sizePage"></param>
        /// <param name="currentSizePage"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, string sizePage, string currentSizePage)
        {
            if(HttpContext.Session.GetString(SessionLoggedIn) == "false" | HttpContext.Session.GetString(SessionLoggedIn) == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            { 
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmailSortParm"] = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["DateRemovedSortParm"] = sortOrder == "Date2" ? "date_desc2" : "Date2";
            if (searchString != null)
            {
                page = 1;
            }

            else
            {
                searchString = currentFilter;
            }

            if (sizePage != null)
            {
                page = 1;
            }

            else
            {
                sizePage = currentSizePage;
            }
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSizePage"] = sizePage;
            var subscribers = from s in _context.Subscribers
                           select s;
            if (!String.IsNullOrEmpty(sizePage))
            {
                int size = int.Parse(sizePage);
                subscribers = subscribers.FromSql("SELECT Top " + size + " * FROM dbo.Subscriber");
            }
            else
            {
                subscribers = subscribers.FromSql("SELECT Top 20 * FROM dbo.Subscriber");
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                subscribers = subscribers.Where(s => s.Email.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "email_desc":
                    subscribers = subscribers.OrderByDescending(s => s.Email);
                    break;
                case "Date":
                    subscribers = subscribers.OrderBy(s => s.DateAdded);
                    break;
                case "date_desc":
                    subscribers = subscribers.OrderByDescending(s => s.DateAdded);
                    break;
                case "Date2":
                    subscribers = subscribers.OrderBy(s => s.DateRemoved);
                    break;
                case "date_desc2":
                    subscribers = subscribers.OrderByDescending(s => s.DateRemoved);
                    break;
                default:
                    subscribers = subscribers.OrderBy(s => s.Email);
                    break;
            }

            int pageSize = 5;



            return View(await PaginatedList<Subscriber>.CreateAsync(subscribers.AsNoTracking(), page ?? 1, pageSize));
            }
        }
       
        // GET: Subscribers/Create
        /// <summary>
        /// Create method for user, can manually create user.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString(SessionLoggedIn) == "false" | HttpContext.Session.GetString(SessionLoggedIn) == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                return View();
            }
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Logic to create a user
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {

                _context.Add(subscriber);
                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Subscribe method to return a view, can see the detail of the view by go to the view folder with the same name
        /// </summary>
        /// <returns></returns>
        public IActionResult Subscribe()
        {
                return View();
        }

        // POST: Subscribers/Subscribe
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // The same as create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {
            // If subscriber not exist in system
            if (!SubscriberExists(subscriber.Email))
            {
                subscriber.DateAdded = DateTime.Now;
                subscriber.IsActive = true;
                _context.Add(subscriber);
                await _context.SaveChangesAsync();
                string ctokenlink = Url.Action("Unsubscribe", "Subscribers", new
                {
                    ID = subscriber.ID,
                }, protocol: HttpContext.Request.Scheme);
                var myUser = _emailSettings.UsernameEmail;
                var myPassword = _emailSettings.UsernamePassword;
                var domainSetting = _emailSettings.PrimaryDomain;
                var port = int.Parse(_emailSettings.PrimaryPort);
                var sslSetting = _emailSettings.EnableSsl;
                var subject = _emailSettings.Subject;
                var displayName = _emailSettings.DisplayName;
                var body = _emailSettings.Body;
                ViewBag.token = ctokenlink;
                SmtpClient client = new SmtpClient(domainSetting, port);
                client.EnableSsl = sslSetting;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(myUser, myPassword);
                //Send mail
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(myUser, displayName);
                mailMessage.To.Add(subscriber.Email);
                mailMessage.Body = body + ViewBag.token;
                mailMessage.Subject = subject;

                client.Send(mailMessage);


                return RedirectToAction(nameof(ThankYou), new { id = subscriber.ID });

                //return View(subscriber);
            }
            else
            {
                var query2 = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().IsActive;
                if (query2 == true)
                {
                    var query = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().ID;
                    return RedirectToAction(nameof(Unsubscribe), new { id = query });
                }
                else
                {
                    var query = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().ID;
                    return RedirectToAction(nameof(Resubscribe), new { id = query });
                }
            }
        }

        /// <summary>
        /// A thank you page with an unsubscribe link 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ThankYou(int? id)
        {

                if (id == null)
                {
                    return NotFound();
                }

                var subscriber = await _context.Subscribers
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (subscriber == null)
                {
                    return NotFound();
                }
                string ctokenlink = Url.Action("Unsubscribe", "Subscribers", new
                {
                    ID = id,
                }, protocol: HttpContext.Request.Scheme);
                ViewBag.token = ctokenlink;
                return View(subscriber);

        }
        /// <summary>
        /// Contact view in shared
        /// </summary>
        /// <returns></returns>
        public IActionResult Contact()
        {
            return View();
        }
        // POST: Subscribers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Contact logic for having the subscribe method
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {
            if (!SubscriberExists(subscriber.Email))
            {
                
                    subscriber.DateAdded = DateTime.Now;
                    _context.Add(subscriber);
                    subscriber.IsActive = true;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                var query2 = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().IsActive;
                if (query2 == true)
                { 
                var query = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().ID;
                return RedirectToAction(nameof(Unsubscribe), new { id = query });
                }
                else
                {
                    var query = _context.Subscribers.Where<Subscriber>(a => a.Email == subscriber.Email).FirstOrDefault().ID;
                    return RedirectToAction(nameof(Resubscribe), new { id = query });
                }
            }
            //return View(subscriber);
        }
        // GET: Subscribers/Resubscribe/5
        /// <summary>
        /// Resubscribe method if the user existed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Resubscribe(int? id)
        {

                if (id == null)
                {
                    return NotFound();
                }

                var subscriber = await _context.Subscribers.SingleOrDefaultAsync(m => m.ID == id);
                if (subscriber == null)
                {
                    return NotFound();
                }
                return View(subscriber);
        }

        // POST: Subscribers/Resubscribe/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Logic for resubscribe page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resubscribe(int id, [Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {
            if (id != subscriber.ID)
            {
                return NotFound();
            }


                try
                {   
                    subscriber.DateAdded = DateTime.Now;
                    subscriber.DateRemoved = null;
                    subscriber.IsActive = true;
                    _context.Update(subscriber);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriberExists(subscriber.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
        }

        /// <summary>
        /// Delete user method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Subscribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString(SessionLoggedIn) == "false" | HttpContext.Session.GetString(SessionLoggedIn) == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                if (id == null)
                {
                    return NotFound();
                }

                var subscriber = await _context.Subscribers
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (subscriber == null)
                {
                    return NotFound();
                }
                string ctokenlink = Url.Action("Unsubscribe", "Subscribers", new
                {
                    ID = id,
                }, protocol: HttpContext.Request.Scheme);
                ViewBag.token = ctokenlink;
                return View(subscriber);
            }
        }

        /// <summary>
        /// Logic for delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriber = await _context.Subscribers.SingleOrDefaultAsync(m => m.ID == id);
            _context.Subscribers.Remove(subscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Subscribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers.SingleOrDefaultAsync(m => m.ID == id);
            if (subscriber == null)
            {
                return NotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edit User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {
            if (id != subscriber.ID)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(subscriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriberExists(subscriber.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            return RedirectToAction(nameof(Index));
    }

        /// <summary>
        /// Unsubscribe the existed user subscribed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Subscribers/Delete/5
        public async Task<IActionResult> Unsubscribe(int? id)
        {
            
                if (id == null)
                {
                    return NotFound();
                }

                var subscriber = await _context.Subscribers
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (subscriber == null)
                {
                    return NotFound();
                }
                string ctokenlink = Url.Action("Unsubscribe", "Subscribers", new
                {
                    ID = id,
                }, protocol: HttpContext.Request.Scheme);
                ViewBag.token = ctokenlink;
                return View(subscriber);

        }

        /// <summary>
        /// Unsubscribe logic
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Unsubscribe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe([Bind("ID,Email,DateAdded,DateRemoved,IsActive")] Subscriber subscriber)
        {
            var query = _context.Subscribers.AsNoTracking().Where<Subscriber>(a => a.ID == subscriber.ID).FirstOrDefault().DateAdded;
            var query2 = _context.Subscribers.AsNoTracking().Where<Subscriber>(a => a.ID == subscriber.ID).FirstOrDefault().Email;
            subscriber.Email = query2;
            subscriber.DateAdded = query;
            subscriber.DateRemoved = DateTime.Now;
            subscriber.IsActive = false;
            _context.Update(subscriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Return a boolean if the subscriber existed by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SubscriberExists(int id)
        {
            return _context.Subscribers.Any(e => e.ID == id);
        }
        /// <summary>
        /// Return a boolean if the subscriber existed by email.
        /// </summary>
        private bool SubscriberExists(string email)
        {
            return _context.Subscribers.Any(e => e.Email == email);
        }

    }
}
