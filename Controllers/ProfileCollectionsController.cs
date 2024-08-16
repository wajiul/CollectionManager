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
    [Route("profile/my/collections")]
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

            var collection = await _collectionRepository.GetCollectionAsync(id);

            var collectionModel = _mapper.Map<CollectionModel>(collection);

            return View(collectionModel);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var collection = new CollectionModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""
            };
            return View(collection);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CollectionModel collectionModel)
        {
            if (ModelState.IsValid)
            {
                var collectionEntity = _mapper.Map<Collection>(collectionModel);
                await _collectionRepository.CreateCollectionAsync(collectionEntity);
                await _collectionRepository.SaveAsync();

                TempData["ToastrMessage"] = "Collection created successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "ProfileCollections");
            }
            return View(collectionModel);
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
            return View(collectionModel);
        }

        [HttpGet("{id}/delete")]
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
            var collectionModel = _mapper.Map<CollectionModel>(collection);
            return View(collectionModel);
        }
        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var collection = await _collectionRepository.GetCollectionAsync(id.Value);
            if(collection == null)
            {
                return NotFound();
            }

            _collectionRepository.DeleteCollection(collection);
            await _collectionRepository.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet("{collectionId}/customfields")]
        public async Task<IActionResult> ManageCustomField(int collectionId)
        {
            var collection = await _collectionRepository.GetCollectionWithCustomFieldAsync(collectionId);
            var collectionModel = _mapper.Map<CollectionWithCustomFieldModel>(collection);
            return View(collectionModel);
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
