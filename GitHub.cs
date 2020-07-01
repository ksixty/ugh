using ugh.Api;
using ugh.Auth;

namespace ugh
{
    public class GitHub
    {
        public readonly GitHubApi Api;
        public readonly GitHubAuth Auth;
        
        public GitHub(string clientId, string clientSecret)
        {
            this.Api = new GitHubApi(clientId, clientSecret);
            this.Auth = new GitHubAuth(this.Api);
        }
    }
}