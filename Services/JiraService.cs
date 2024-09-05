namespace CollectionManager.Services
{
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Text;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using CollectionManager.Models;

    public class JiraService : IJiraService
    {
        private readonly string _jiraBaseUrl;
        private readonly string _jiraEmail;
        private readonly string _jiraApiToken;
        private readonly string _jiraProjectKey;

        public JiraService(IConfiguration configuration)
        {
            _jiraBaseUrl = configuration["Jira:API"];
            _jiraEmail = configuration["Jira:Username"];
            _jiraApiToken = configuration["Jira:Passkey"];
            _jiraProjectKey = configuration["Jira:ProjectKey"];
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            var authenticationString = $"{_jiraEmail}:{_jiraApiToken}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            return httpClient;
        }

        public async Task<string> CreateUserAsync(object newUser)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(newUser);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_jiraBaseUrl}/user", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);
                    return jsonResponse.SelectToken("accountId")?.ToString();
                }

                return null;
            }
        }

        public async Task<string> CreateIssueAsync(object newIssue)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(newIssue);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_jiraBaseUrl}/issue", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseContent);
                    return jsonResponse.SelectToken("id")?.ToString();
                }

                return null;
            }
        }

        public async Task<bool> SetIssueWebLinkAsync(string issueId, IssueWebLink webLink)
        {
            using (var httpClient = CreateHttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(webLink);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_jiraBaseUrl}/issue/{issueId}/remotelink", httpContent);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<JiraResponse> GetIssuesOfReporterAsync(string reporterId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{_jiraBaseUrl}/search?jql=project={_jiraProjectKey}%20AND%20reporter={reporterId}&fields=id,key,name,status,summary,priority");
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JiraResponse>(json) ?? new JiraResponse();
            }
        }
    }

}
