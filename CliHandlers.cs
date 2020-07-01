using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using LibGit2Sharp;
using ugh.Api;

namespace ugh
{
    public partial class Cli
    {
        private async Task SendHelp()
        {
            Display("<p>Использование: ugh [команды]</p>" +
                    "<br><p>Доступные команды:</p>" +

                    "<h1>Пользователь</h1>" +
                    "<li>user login\t\tвойти в уч. запись</li>" +
                    "<li>user logout\t\tвыйти из уч. записи</li>" +
                    "<li>user [польз.]\t\tполучить сведения о пользователе</li>" +
                    "<li>user\t\t\tсведения о текущей уч. записи</li>" +

                    "<h1>Репозиторий</h1>" +
                    "<li>repo\t\t\tполучить сведения о тек. реп.</li>" +
                    "<li>repo owner/name\tполучить сведения о репозитории</li>" +

                    "<h1>SSH-ключи</h1>" +
                    "<li>keys user\t\tпоказать ключи пользователя</li>" +
                    "<li>keys user N\t\tскопировать ключ номер N</li>");

        }

        private Repository? getCurrentGitRepo(string pwd)
        {
            try
            {
                return new Repository(pwd);
            }
            catch
            {
                return null;
            }
        }

        private async Task SendRepo(params string[] args)
        {
            GitHubApiResponse.Repository? repo = null;
            DateTime? headDate = null;
            if (args.Length == 1)
            {
                var gitRepo = getCurrentGitRepo(_pwd);
                if (gitRepo != null)
                {
                    foreach (var origin in gitRepo.Network.Remotes)
                    {
                        if (origin.Url.Contains("github.com"))
                        {
                            var originPath = origin.Url.Split(':')[1];
                            var ownerAndRepo = originPath.Replace(".git", "").Split('/'); 
                            repo = await _ghc.Api.GetRepository(ownerAndRepo[0], ownerAndRepo[1]);
                            
                            headDate = gitRepo.Head.Tip.Committer.When.DateTime.ToUniversalTime();
                            break;
                        }
                    }
                    if (repo == null)
                    {
                      Display(@"<p>Это не GitHub-репозиторий.</p>"); 
                    }
                }
                else
                {
                    Display(@"<p>Чтобы вызывать ugh repo без аргумента, 
                    надо находиться в директории с репозиторием.</p>");
                    return;
                }
            } 
            else if (args.Length != 2)
            {
                Display(@"<e>Ошибка: вы что-то не то написали<e>");
                return;
            }
            else
            {
                repo = await _ghc.Api.GetRepository(args[0], args[1]);
            }
            
            Display($@"
            <h1>Репозиторий {repo.FullName}</h1>
            <p>{repo.Description}</p>
            <p>⭐ {repo.Stars} 👁 {repo.Watchers} ❗️ {repo.Issues}</p>
            <p>{repo.HtmlUrl}</p>
            <br>
            <p>Склонировать:</p>
            <ul>HTTP → {repo.HttpUrl}</ul>
            <ul>SSH  → {repo.SshUrl}</ul>
            ");

            if (headDate < repo.PushTime)
            {
                Display($@"<br><warn>Ваша копия репозитория устарела.</warn>
                <p>Дата последнего коммита на сайте:</p>
                <li>{repo.PushTime:yyyy-MM-dd HH:mm:ss UTC}</li>
                <p>Дата последнего коммита в вашей копии:</p>
                <li>{headDate:yyyy-MM-dd HH:mm:ss UTC}</li>
                <br><i>Обновить: git pull</i>");
            }
        }

        private async Task LogIn()
        {
            if (_ghc.Auth.IsAuthenticated())
            {
                Display(@"<e>Чтобы войти, необходимо сперва выйти!</e>
<i>Для этого воспользуйтесь командой ugh user logout.</i>");
                return;
            }
            Display($@"<p>Чтобы войти, перейдите по ссылке:</p>
            <p>{_ghc.Auth.GetAuthLink()}</p>");
            try
            {
                await _ghc.Auth.LogIn();
                Display($"<h1>Вы вошли как {_ghc.Auth.Session.User.Name}</h1>");
            }
            catch
            {
                Display("<e>Не судьба!</e>");
            }
        }

        private async Task LogOut()
        {
            if (!_ghc.Auth.IsAuthenticated())
            {
                Display(@"<e>Чтобы выйти, необходимо сперва войти!</e>
<p>Для этого воспользуйтесь командой ugh user login.</p>");
                return;
            }
            _ghc.Auth.Session.Clear();
           Display("<p>Вы успешно вышли!</p>"); 
        }

        private string FormatUser(GitHubApiResponse.User user)
        {
            return $@"
            <h1>{user.Name} @{user.Login}</h1>
            {(user.Bio != "" ? ("<p>" + user.Bio + "</p>") : "[Нет описания]")}
            <p>👥 {user.Followers} 🕵️ {user.Following} 🗂️ {user.Repos}</p>
            <u>https://github.com/${user.Login}</u>
            <p></p>
            ";
        }
        
        private async Task SendUser()
        {
            if (!_ghc.Auth.IsAuthenticated())
            {
                Display($@"<e>О вас ничего неизвестно.<e><i>Авторизуйтесь командой ugh user login</i>");
            }
            else
            {
                var user = _ghc.Api.GetCurrentUser();
                var result = FormatUser(await user);
                result += "<i>Вы можете выйти с помощью команды ugh user logout</i>";
                Display(result);
            }
        }

        private async Task SendUser(string login)
        {
            var user = _ghc.Api.GetUser(login);
            var result = FormatUser(await user);
            Display(result);
        }

        private async Task SendKeys(string login)
        {
            var keys = await _ghc.Api.GetKeys(login);
            if (keys.Count == 0)
            {
                Display($"<p>У пользователя {login} нет публичных SSH-ключей.</p>");
            }
            for (var i=0; i<keys.Count; i++)
            {
                Display($"<h1>🔑 {i+1}/{keys.Count}</h1>" +
                        $"<p>{keys[i].Value}</p><br>");
            }
        }

        private async Task SendKeys(string login, string countStr)
        {
            int count = -1;
            try
            {
                count = Int32.Parse(countStr);
            }
            catch
            {
                Display($"Надо номер ключа, а не номер не надо!");
                return;
            }
            GitHubApiResponse.Key? key = (await _ghc.Api.GetKeys(login)).ElementAtOrDefault(count-1);
            if (key != null)
            {
                await TextCopy.ClipboardService.SetTextAsync(key.Value);
                Display($"<h1>🔑 Ключ {count} пользователя {login} скопирован в буфер обмена.<br>");
            }
            else
            {
                Display($"Нет такого ключа!<br>");
            }
        }
    }
}
