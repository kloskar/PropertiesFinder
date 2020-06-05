using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseConnection;
using Models;
using Application;
using Application.Done;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]

    
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EntriesController(DatabaseContext context)
        {
            _context = context;
        }

        ~EntriesController()
        {
            _context.Dispose();
        }

        // GET: api/Entries
        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetDbEntries()
        {
            var entries =  await _context.DbEntries.Include(x=>x.PropertyAddress)
                .Include(x => x.PropertyDetails)
                .Include(x => x.PropertyFeatures)
                .Include(x => x.PropertyPrice)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).ToListAsync();
            if (entries.Count == 0)
                return new NotFoundResult();
            return entries;
        }

        // GET: api/Entries/5
        [Authorize(Policy = "User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntry(int id)
        {
            if (id < 0)
            {
                return new BadRequestResult();
            }

            var entry = await _context.DbEntries.Include(x=>x.PropertyAddress)
                .Include(x => x.PropertyDetails)
                .Include(x => x.PropertyFeatures)
                .Include(x => x.PropertyPrice)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).Where(x=>x.EntryID == id).ToListAsync();

            if (entry == null)
            {
                return new NotFoundResult();
            }

            return entry;
        }

        // GET: api/Entries/5/1
        [Authorize(Policy = "User")]
        [HttpGet("{pagelimit}/pageid")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntry(int PageLimit, int PageId)
        {
            if (PageId < 0 || PageLimit < 0)
            {
                return new BadRequestResult();
            }

            int StartPage = (PageId-1) * PageLimit + 1;
            int StopPage = PageId * PageLimit;

            var entry = await _context.DbEntries.Include(x => x.PropertyAddress)
                .Include(x => x.PropertyDetails)
                .Include(x => x.PropertyFeatures)
                .Include(x => x.PropertyPrice)
                .Include(x => x.OfferDetails).ThenInclude(x => x.SellerContact).Where(x => x.EntryID >= StartPage && x.EntryID <= StopPage).ToListAsync();

            if (entry == null)
            {
                return new NotFoundResult();
            }

            return entry;
        }

        // PUT: api/Entries/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntry(int id, Entry entry)
        {
            if (id != entry.EntryID)
            {
                return BadRequest();
            }

            _context.Entry(entry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _context.Update(entry);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Entries
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "User")]
        [HttpPost]
        public async Task<ActionResult<Entry>> PostEntry(Entry entry)
        {
            Integration integration = new Integration();
            WebPage web = new WebPage();
            web.Url = "http://nportal.pl/mieszkania/?page=270";
            List<Entry> entries = integration.GetEntries(web);

            if (entries.Count() == 0)
            {
                return new NotFoundResult();
            }

            _context.DbEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.EntryID }, entry);
        }

        [HttpPost("{page}")]
        public async Task<ActionResult<Entry>> PostEntryPage(int page)
        {
            Entry entry = new Entry();
            if (page < 0)
            {
                return new BadRequestResult();
            }

            Integration integration = new Integration();
            WebPage web = new WebPage();
            web.Url = "http://nportal.pl/mieszkania/?page="+page.ToString();
            List<Entry> entries = integration.GetEntriesPage(web);


            if (entries.Count() == 0)
            {
                return new NotFoundResult();
            }

            _context.DbEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = entry.EntryID }, entries);
        }

        // DELETE: api/Entries/5
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Entry>> DeleteEntry(int id)
        {
            var entry = await _context.DbEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }

            _context.DbEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        private bool EntryExists(int id)
        {
            return _context.DbEntries.Any(e => e.EntryID == id);
        }

    }
}
