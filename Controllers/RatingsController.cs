#nullable disable
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    public class RatingsController : Controller
    {
        private readonly ChatAppContext _context;

        public RatingsController(ChatAppContext context)
        {
            _context = context;
        }

        // GET: Ratingss
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ratings.ToListAsync());
        }
        public async Task<IActionResult> Create2()
        {
            return View(await _context.Ratings.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var Ratings = from Rating in _context.Ratings
                         where Rating.Name.Contains(query)
                         select Rating;
            return View(await Ratings.ToListAsync());
        }
        // GET: Ratingss/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Ratings = await _context.Ratings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Ratings == null)
            {
                return NotFound();
            }

            return View(Ratings);
        }

        // GET: Ratingss/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ratingss/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rate,Text,Name, Time")] Rating Rating)
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            Rating.Time = time;
            if (ModelState.IsValid)
            {
                _context.Add(Rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Rating);
        }

        // GET: Ratingss/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Ratings = await _context.Ratings.FindAsync(id);
            if (Ratings == null)
            {
                return NotFound();
            }

            return View(Ratings);
        }

        // POST: Ratingss/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rate,Text,Name,Time")] Rating Rating)
        {
            if (id != Rating.Id)
            {
                return NotFound();
            }
            string time = DateTime.Now.ToString("h:mm:ss tt");
            Rating.Time = time;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingsExists(Rating.Id))
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
            return View(Rating);
        }

        // GET: Ratingss/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Ratings = await _context.Ratings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Ratings == null)
            {
                return NotFound();
            }

            return View(Ratings);
        }

        // POST: Ratingss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Ratings = await _context.Ratings.FindAsync(id);
            _context.Ratings.Remove(Ratings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingsExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
