using System;
using System.Threading.Tasks;
using System.Transactions;
using ugh.Api;

#nullable enable
namespace ugh.Auth
{
    public class GitHubAuth
    {
        public GitHubAuthSession Session;
        private GitHubApi Api; 

        public GitHubAuth(GitHubApi api)
        {
            Api = api;
            Session = new GitHubAuthSession();
            if (IsAuthenticated())
            {
                api.SetToken(Session.Token.Value);
            }
        }
        
        public Uri GetAuthLink() =>
            new Uri($"{Config.OAuthUrl}/authorize?client_id={Api.ClientId}");

        public bool IsAuthenticated() => (Session.Token != null);
        
        public async Task LogIn()
        {
            var server = new GitHubAuthServer();
            var tempCode = await server.CatchTempCode();
            var authToken = await Api.PostTempCode(tempCode);
            Api.SetToken(authToken.Value);
            Session = new GitHubAuthSession(authToken, await Api.GetCurrentUser());
        }

        public void LogOut()
        {
            Session.Clear();
            Api.RemoveToken();
        }
        
    }
}
#nullable restore