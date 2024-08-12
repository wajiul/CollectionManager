using AutoMapper;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
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
        private readonly CollectionRepository _collectionRepository;
        private readonly IMapper _mapper;

        public ProfileCollectionsController(CollectionMangerDbContext context, CollectionRepository collectionRepository, IMapper mapper)
        {
            _context = context;
            _collectionRepository = collectionRepository;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var collectionExist = _collectionRepository.IsCollectionExist(id, userId);

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

                await _collectionRepository.CreateCollectionAsync(newCollection);
                await _collectionRepository.SaveAsync();

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

            var collection = await _collectionRepository.GetCollectionAsync(id.Value);
            if (collection == null)
            {
                return NotFound();
            }

            var collectionModel = _mapper.Map<CollectionModel>(collection);

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
                    var collection = await _collectionRepository.GetCollectionAsync(id);

                    collection.Name = collectionModel.Name;
                    collection.Category = collectionModel.Category;
                    collection.Description = collectionModel.Description;
                    collection.ImageUrl = collectionModel.ImageUrl;

                    _collectionRepository.UpdateCollection(collection);
                    await _collectionRepository.SaveAsync();
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

            var collection = await _collectionRepository.GetCollectionAsync(id.Value);
            if (collection == null)
            {
                return NotFound();
            }
            _collectionRepository.DeleteCollection(collection);
            await _collectionRepository.SaveAsync();
            return View(collection);
        }

        [HttpGet("{collectionId}/customfields")]
        public async Task<IActionResult> ManageCustomField(int collectionId)
        {
            var collection = await _collectionRepository.GetCollectionWithCustomFieldAsync(collectionId);
            return View(collection);
        }

        private bool CollectionExists(int id)
        {
            return _collectionRepository.IsCollectionExist(id);
        }

        [HttpPost("customfields/add")]
        public async Task<IActionResult> AddCustomField([FromBody] CustomField customField)
        {
            var existing = _collectionRepository.IsCustomFieldExist(customField);

            if (existing == true)
            {
                return BadRequest("Field already exist");
            }

            await _collectionRepository.CreateCustomField(customField);
            await _collectionRepository.SaveAsync();
            return Ok(new { Id = customField.Id });
        }

        [HttpDelete("customfields/delete/{id}")]
        public async Task<IActionResult> DeleteCustomField(int Id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deletePermisible = _collectionRepository.DoesUserHasCustomField(Id, userId);

            if (!deletePermisible)
            {
                return BadRequest();
            }

            var field = await _collectionRepository.GetCustomFieldAsync(Id);

            _collectionRepository.DeleteCustomField(field);
            await _collectionRepository.SaveAsync();

            return Ok(new { Id = field.Id });
        }


    }
}
