using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    [Route("profile/collections")]
    public class ProfileCollectionsController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public ProfileCollectionsController(CollectionMangerDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var collectionExist = _context.collections.Any(c => c.UserId == userId && c.Id == id);

            if (!collectionExist)
            {
                return NotFound();
            }

            var collection = await _context.collections
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(collection);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var collection = new CollectionModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            return View(collection);
        }

        [HttpPost("create")]
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
                return RedirectToAction("Index", "ProfileCollections");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", collection.UserId);
            return View(collection);
        }

        [HttpGet("edith/{id}")]
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


        [HttpPost("edith/{id}")]
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
                return RedirectToAction("Index", "ProfileCollections");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", collectionModel.UserId);
            return View(collectionModel);
        }

        [HttpDelete("delete/{id}")]
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

        [HttpGet("{collectionId}/customfields")]
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

        [HttpPost("customfields/add")]
        public async Task<IActionResult> AddCustomField([FromBody] CustomField customField)
        {
            var existing = await _context.customFields
               .FirstOrDefaultAsync(
                    c => c.CollectionId == customField.CollectionId &&
                    c.Name == customField.Name &&
                    c.Type == customField.Type
            );

            if (existing != null)
            {
                return BadRequest("Field already exist");
            }

            await _context.customFields.AddAsync(customField);
            await _context.SaveChangesAsync();
            return Ok(new { Id = customField.Id });
        }

        [HttpDelete("customfields/delete/{id}")]
        public async Task<IActionResult> DeleteCustomField(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deletePermisible = _context.collections
                .Any(c => c.UserId == userId && c.CustomFields.Any(cf => cf.Id == Id));

            if (!deletePermisible)
            {
                return BadRequest();
            }
            var field = await _context.customFields.FindAsync(Id);

            _context.customFields.Remove(field);
            await _context.SaveChangesAsync();
            return Ok(new { Id = field.Id });
        }


    }
}
