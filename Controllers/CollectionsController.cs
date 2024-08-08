using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Identity;
using CollectionManager.Enums;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    //[Route("collections")]
    public class CollectionsController : Controller
    {
        private readonly CollectionMangerDbContext _context;
        private readonly UserManager<User> _userManager;

        public CollectionsController(CollectionMangerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {

            var collectionMangerDbContext = _context.collections.Include(c => c.User);
            return View(await collectionMangerDbContext.ToListAsync());
        }


        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.collections
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            var collection = new CollectionModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            return View(collection);
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CollectionModel collection)
        {
            if (ModelState.IsValid)
            {
                var newCollection = new Collection
                {
                    Name = collection.Name,
                    Description = collection.Description,
                    Category = collection.Category,
                    ImageUrl = collection.ImageUrl,
                    UserId = collection.UserId
                };
                _context.Add(newCollection);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", collection.UserId);
            return View(collection);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", collection.UserId);

            var collectionModel = new CollectionModel
            {
                Id = collection.Id,
                Name = collection.Name,
                Description = collection.Description,
                Category = collection.Category,
                ImageUrl = collection.ImageUrl,
                UserId = collection.UserId
            };

            return View(collectionModel);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CollectionModel collectionModel)
        {
            if (id != collectionModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var collection = await _context.collections.FindAsync(id);

                    collection.Name = collectionModel.Name;
                    collection.Category = collectionModel.Category;
                    collection.Description = collectionModel.Description;
                    collection.ImageUrl = collectionModel.ImageUrl;

                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collectionModel.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", collectionModel.UserId);
            return View(collectionModel);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.collections
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.collections.FindAsync(id);
            if (collection != null)
            {
                _context.collections.Remove(collection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageCustomField(int collectionId)
        {
            var collection = await _context.collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(x => x.Id == collectionId);
            return View(collection);
        }

        private bool CollectionExists(int id)
        {
            return _context.collections.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomField([FromBody] CustomField customField) {
            var existing = await _context.customFields
               .FirstOrDefaultAsync(
                    c => c.CollectionId == customField.CollectionId && 
                    c.Name == customField.Name && 
                    c.Type == customField.Type
                );

            if(existing != null)
            {
                return BadRequest("Field already exist");
            }

            await _context.customFields.AddAsync(customField);
            await _context.SaveChangesAsync();
            return Ok(new {Id = customField.Id});
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomField(int Id)
        {
            var field = await _context.customFields.FindAsync(Id);
            if(field == null)
            {
                return BadRequest();
            }
            _context.customFields.Remove(field);
            await _context.SaveChangesAsync();
            return Ok(new { Id = field.Id });
        }
    }
}
