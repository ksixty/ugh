using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace ugh.Auth
{
    public class GitHubAuthServer
    {
        private readonly HttpListener _listener;

        public GitHubAuthServer()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://127.0.0.1:{Config.ServerPort}/");
            _listener.Start();
        }

        private async Task SendText(HttpListenerResponse res, string text)
        {
            Stream output = res.OutputStream;
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            res.ContentLength64 = buffer.Length;
            res.ContentType = "text/plain; charset=utf-8";
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }
        
        public async Task<string> CatchTempCode()
        {
            while (true)
            {
                HttpListenerContext ctx = await _listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;

                string? tempCode = req.QueryString.Get("code");
                if (tempCode == null)
                {
                    await SendText(res, "Вы как сюда попали вообще?...");
                }
                else
                {
                    await SendText(res, "Вроде получилось. Теперь можно пойти назад в приложение.");
                    _listener.Close();
                    return tempCode;
                }
            }
        } 
    }
}
#nullable restore