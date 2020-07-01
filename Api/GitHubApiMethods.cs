using System.Collections.Generic;
using System.Threading.Tasks;

namespace ugh.Api
{
    public partial class GitHubApi
    {
        public async Task<GitHubApiResponse.User> GetUser(string user)
        {
            return await GetResponse<GitHubApiResponse.User>("users", user);
        }

        public async Task<GitHubApiResponse.User> GetCurrentUser()
        {
            return await GetRestrictedResponse<GitHubApiResponse.User>("user");
        }

        public async Task<GitHubApiResponse.Repository> GetRepository(string owner, string name)
        {
            return await GetResponse<GitHubApiResponse.Repository>("repos", owner, name);
        }

        public async Task<List<GitHubApiResponse.Key>> GetKeys(string user)
        {
            return await GetResponse<List<GitHubApiResponse.Key>>("users", user, "keys");
        }
        
        public async Task<GitHubApiResponse.Token> PostTempCode(string tempCode)
        {
            var form = new Dictionary<string, string>()
            {
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"code", tempCode},
            };
            return await PostResponse<GitHubApiResponse.Token>(form,
                $"{Config.OAuthUrl}/access_token");
        }
    }
}