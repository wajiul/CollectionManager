using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using CollectionManager.Data_Access.Repositories;
using System.Security.Claims;
using System.Net.Sockets;

namespace CollectionManager.Controllers
{
    public class JiraIntegrationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _projectKey;
        public JiraIntegrationController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _projectKey = configuration["Jira:ProjectKey"];
        }

        public IActionResult CreateTicket(string currentUrl)
        {
            TempData["CurrentUrl"] = currentUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(UserTicketInputModel ticket)
        {
            string? pageLink = TempData["CurrentUrl"] as string;

            if (!ModelState.IsValid)
            {
                TempData["CurrentUrl"] = pageLink;
                return View(ticket);
            }

            try
            {
                var jiraAccountId = await CreateUserInJira();

                var issueId = await CreateIssue(_projectKey, ticket.Summary, ticket.PriorityId, jiraAccountId);

                await SetIssueWebLink(issueId, pageLink);

                TempData["ToastrMessage"] = "Successfully added suppprt ticket";
                TempData["ToastrType"] = "success";

                if (!string.IsNullOrEmpty(pageLink))
                {
                    return Redirect(pageLink);
                }

            }
            catch(Exception)
            {
                TempData["ToastrMessage"] = "An error occured creating suppprt ticket";
                TempData["ToastrType"] = "error";
            }

            return View(ticket);
        }

        private async Task<string> CreateIssue(string projectKey, string summary, string priorityId, string reporterId)
        {
            string isseuType_Bug_Id = "10009"; 
            var newIssue = new
            {
                fields = new
                {
                    project = new
                    {
                        key = projectKey
                    },
                    summary = summary,
                    issuetype = new
                    {
                        id = isseuType_Bug_Id
                    },
                    priority = new
                    {
                        id = priorityId,
                    },
                    reporter = new
                    {
                        id = reporterId
                    }
                }
            };

            return await _unitOfWork.Jira.CreateIssueAsync(newIssue);
        }
        
        private async Task<string> CreateUserInJira()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _unitOfWork.User.GetUserAsync(userId);

            var newJiraUser = new
            {
                emailAddress = user.Email,
                products = new string[] { }
            };

            return await _unitOfWork.Jira.CreateUserAsync(newJiraUser);
        }
        private async Task SetIssueWebLink(string issueId, string pageLink)
        {
            var issueWebLink = new IssueWebLink
            {
                WebLinkObject = new WebLinkObject
                {
                    Url = pageLink,
                    Title = "Issue page"
                }
            };

            await _unitOfWork.Jira.SetIssueWebLinkAsync(issueId, issueWebLink);
        }

    }
}
