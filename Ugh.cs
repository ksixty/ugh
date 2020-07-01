using System;
using System.Threading.Tasks;

namespace ugh
{
    class Ugh
    {
        public async static Task Main(string[] args)
        {
            var ghc = new GitHub(Config.ClientId, Config.ClientSecret);
            var cli = new Cli(ghc);
            await cli.Run(args);
        }
    }
}
