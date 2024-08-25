using AutoMapper;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CollectionManager.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/{userId}/collections")]
    [Authorize(Roles = "admin")]
    public class ManageUserCollectionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ManageUserCollectionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Index(string userId)
        {
            ViewData["UserId"] = userId;
            return View();
        }

        [HttpGet("{collectonId}")]
        public async Task<IActionResult> Details(int collectionId, string userId)
        {

            var collectionExist = _unitOfWork.Collection.IsCollectionExist(collectionId, userId);

            if (!collectionExist)
            {
                return NotFound();
            }

            var collection = await _unitOfWork.Collection.GetCollectionAsync(collectionId);

            var collectionModel = _mapper.Map<CollectionModel>(collection);

            return View(collectionModel);
        }

        [HttpGet("create")]
        public IActionResult Create(string userId)
        {
            var collection = new CollectionModel
            {
                UserId = userId,
            };
            return View(collection);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CollectionModel collectionModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var collectionEntity = _mapper.Map<Collection>(collectionModel);
                    await _unitOfWork.Collection.CreateCollectionAsync(collectionEntity);
                    await _unitOfWork.Save();
                    _unitOfWork.Collection.UpdateSearchVector();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                    {
                        ModelState.AddModelError("","Collection already exist.");
                        return View(collectionModel);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    TempData["ToastrMessage"] = "An error occured inserting collection";
                    TempData["ToastrType"] = "error";
                    return View(collectionModel);
                }

                TempData["ToastrMessage"] = "Collection created successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "ManageUserCollections", new {userId = collectionModel.UserId});
            }
            return View(collectionModel);
        }

        [HttpGet("edith/{collectionId}")]
        public async Task<IActionResult> Edit(int? collectionId, string userId)
        {
            if (collectionId == null)
            {
                return NotFound();
            }

            var collection = await _unitOfWork.Collection.GetCollectionAsync(collectionId.Value);
            if (collection == null)
            {
                return NotFound();
            }

            var collectionModel = _mapper.Map<CollectionModel>(collection);

            return View(collectionModel);
        }


        [HttpPost("edith/{collectionId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int collectionId, CollectionModel collectionModel)
        {
            if (collectionId != collectionModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var collection = await _unitOfWork.Collection.GetCollectionAsync(collectionId);

                    collection.Name = collectionModel.Name;
                    collection.Category = collectionModel.Category;
                    collection.Description = collectionModel.Description;

                    await _unitOfWork.Save();
                    _unitOfWork.Collection.UpdateSearchVector();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                    {
                        ModelState.AddModelError("", "Collection already exist.");
                        return View(collectionModel);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    TempData["ToastrMessage"] = "An error occured updating collection";
                    TempData["ToastrType"] = "error";
                    return View(collectionModel);
                }

                TempData["ToastrMessage"] = "Successfully updated collection";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "ManageUserCollections", new {userId = collectionModel.UserId});
            }
            return View(collectionModel);
        }

        [HttpGet("{collectionId}/delete")]
        public async Task<IActionResult> Delete(int? collectionId, string userId)
        {
            if (collectionId == null)
            {
                return NotFound();
            }

            var collection = await _unitOfWork.Collection.GetCollectionAsync(collectionId.Value);
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
            var collection = await _unitOfWork.Collection.GetCollectionAsync(id.Value);

            if (collection == null)
            {
                return NotFound();
            }

            _unitOfWork.Collection.DeleteCollection(collection);
            await _unitOfWork.Save();
            _unitOfWork.Collection.UpdateSearchVector();

            TempData["ToastrMessage"] = "Successfully deleted collection";
            TempData["ToastrType"] = "success";

            var userId = RouteData.Values["userId"].ToString();

            return RedirectToAction("Index", new {userId = userId});
        }

        [HttpGet("{collectionId}/customfields")]
        public async Task<IActionResult> ManageCustomField(int collectionId, string userId)
        {
            var collection = await _unitOfWork.Collection.GetCollectionWithCustomFieldAsync(collectionId);
            var collectionModel = _mapper.Map<CollectionWithCustomFieldModel>(collection);
            return View(collectionModel);
        }

        private bool CollectionExists(int id)
        {
            return _unitOfWork.Collection.IsCollectionExist(id);
        }

        [HttpPost("customfields/add")]
        public async Task<IActionResult> AddCustomField([FromBody] CustomField customField)
        {
            var existing = _unitOfWork.Collection.IsCustomFieldExist(customField);

            if (existing == true)
            {
                return BadRequest("Field already exist");
            }

            await _unitOfWork.Collection.CreateCustomField(customField);
            await _unitOfWork.Save();
            return Ok(new { Id = customField.Id });
        }

        [HttpDelete("customfields/delete/{id}")]
        public async Task<IActionResult> DeleteCustomField(int Id, string userId)
        {
            var deletePermisible = _unitOfWork.Collection.DoesUserHasCustomField(Id, userId);

            if (!deletePermisible)
            {
                return BadRequest();
            }

            var field = await _unitOfWork.Collection.GetCustomFieldAsync(Id);

            _unitOfWork.Collection.DeleteCustomField(field);
            await _unitOfWork.Save();

            return Ok(new { Id = field.Id });
        }

    }
}
