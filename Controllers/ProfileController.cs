using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace CollectionManager.Controllers
{
    [Route("profile")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        const string accessUrl = "https://login.salesforce.com/services/oauth2/token";
        const string apiUrl = "https://practicecom-11d-dev-ed.develop.my.salesforce.com/services/data/v58.0/sobjects";
        public ProfileController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyProfile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userProfile = await _unitOfWork.User.GetUserProfileAsync(userId);
            ViewData["UserId"] = userId;
            return View(userProfile);
        }


        [HttpGet("support-ticket")]
        public async Task<IActionResult> JiraSupportTicket()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.GetUserAsync(userId);
            var reporterId = await GetJiraReporterId(user.Email);

            var tickets = await _unitOfWork.Jira.GetIssuesOfReporterAsync(reporterId);

            return View(tickets);
        }

        [HttpGet("link-salesforce")]
        public IActionResult LinkToSalesforce()
        {
            return View();
        }

        [HttpPost("link-salesforce")]
        public async Task<IActionResult> LinkToSalesforce(ContactModel contact)
        {
            if(!ModelState.IsValid)
            {
                return View(contact);
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.GetUserAsync(userId);

            var salesforceUserAccount = new SalesforceAccountModel
            {
                Id = user.Id,
                Name = string.Concat(user.FirstName, " ", user.LastName),
                Email = user.Email,
                Status = user.Status.ToString(),
                Country = contact.Country,
                City = contact.City,
                Phone = contact.Phone,
            };

            string accessToken = await GetSalesforceAccessToken();

            if(!string.IsNullOrEmpty(accessToken))
            {
                var success = await CreateSalesforceUserAccountAsync(accessToken, salesforceUserAccount);
                if (success)
                {
                    await _unitOfWork.User.UpdateSalesforceConnectionStatusAsync(userId, true);
                    await _unitOfWork.Save();
                    TempData["ToastrMessage"] = "Successfully connected to salesforce";
                    TempData["Toastrtype"] = "success";
                }
                else
                {
                    TempData["ToastrMessage"] = "An error occured connecting to salesforce";
                    TempData["Toastrtype"] = "error";
                }
            } 

            return RedirectToAction("MyProfile", "Profile");
        }

        private async Task<string> GetSalesforceAccessToken()
        {
            using var httpClient = new HttpClient();

            var id = _configuration["Salesforce:ClientId"];

            string clientId = _configuration["Salesforce:ClientId"];
            string clientSecret = _configuration["Salesforce:ClientSecret"];
            string username = _configuration["Salesforce:Username"];
            string passkey = _configuration["Salesforce:PassKey"];

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", passkey)
            });

            var response = await httpClient.PostAsync(accessUrl, content);

            var responseString = await response.Content.ReadAsStringAsync();

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
            return jsonResponse.access_token;
        }

        public async Task<bool> CreateSalesforceUserAccountAsync(string accessToken, SalesforceAccountModel userModel)
        {
            string url = apiUrl +  "/User__c";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var jsonContent = JsonConvert.SerializeObject(new
                {
                    UserId__c = userModel.Id,
                    Name = userModel.Name,
                    Email__c = userModel.Email,
                    Status__c = userModel.Status,
                    Phone__c = userModel.Phone,
                    Country__c = userModel.Country,
                    City__c = userModel.City,
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        private async Task<string> GetJiraReporterId(string email)
        {
            var newJiraUser = new
            {
                emailAddress = email,
                products = new string[] { }
            };
            return await _unitOfWork.Jira.CreateUserAsync(newJiraUser);
        }


    }
}
