using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectionManager.Components
{
    public class JiraTicket: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public JiraTicket(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync(string userId, string email)
        {
            var user = await _unitOfWork.User.GetUserAsync(userId);
            var reporterId = await GetJiraReporterId(email);
            var tickets = await _unitOfWork.Jira.GetIssuesOfReporterAsync(reporterId);
            return View(tickets);
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
