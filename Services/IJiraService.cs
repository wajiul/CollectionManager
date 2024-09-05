using CollectionManager.Models;

namespace CollectionManager.Services
{
    public interface IJiraService
    {
        Task<string> CreateIssueAsync(object newIssue);
        Task<string> CreateUserAsync(object newUser);
        Task<JiraResponse> GetIssuesOfReporterAsync(string reporterId);
        Task<bool> SetIssueWebLinkAsync(string issueId, IssueWebLink webLink);
    }
}