using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TweetDotNet.Data;
using TweetDotNet.Models;

namespace TweetDotNet.Controllers
{
    [Authorize(Policy = "AccountRequired")]
    public class TweetBlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TweetBlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TweetBlog
        public async Task<IActionResult> Index()
        {
            return View(await _context.TweetBlogs.ToListAsync());
        }

        [HttpGet]
        [Authorize(Policy = "MinimumAge")]
        public async Task<IActionResult> GetAdvancedTopics()
        {
            return View(await _context.TweetBlogs.Where(x => x.LevelAdvanced == true).ToListAsync());
        }

        // GET: TweetBlog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweetBlog = await _context.TweetBlogs
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tweetBlog == null)
            {
                return NotFound();
            }

            return View(tweetBlog);
        }

        // GET: TweetBlog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TweetBlog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,HeadLine,DateCreated,Content")] TweetBlog tweetBlog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tweetBlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tweetBlog);
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweetBlog = await _context.TweetBlogs.SingleOrDefaultAsync(m => m.Id == id);
            if (tweetBlog == null)
            {
                return NotFound();
            }
            return View(tweetBlog);
        }

        // POST: TweetBlog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,HeadLine,DateCreated,Content")] TweetBlog tweetBlog)
        {
            if (id != tweetBlog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tweetBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TweetBlogExists(tweetBlog.Id))
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
            return View(tweetBlog);
        }

        // GET: TweetBlog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweetBlog = await _context.TweetBlogs
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tweetBlog == null)
            {
                return NotFound();
            }

            return View(tweetBlog);
        }

        // POST: TweetBlog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tweetBlog = await _context.TweetBlogs.SingleOrDefaultAsync(m => m.Id == id);
            _context.TweetBlogs.Remove(tweetBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TweetBlogExists(int id)
        {
            return _context.TweetBlogs.Any(e => e.Id == id);
        }
    }
}
