using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ugh.Api;

namespace ugh.Auth
{
    [Serializable]
    public class GitHubAuthSession
    {
        public GitHubApiResponse.User? User;
        public readonly GitHubApiResponse.Token? Token;

        private static readonly IsolatedStorageFile Store = 
            IsolatedStorageFile.GetUserStoreForApplication();

        public GitHubAuthSession()
        {
            var savedSession = LoadFromStore();
            Token = savedSession?.Token;
            User = savedSession?.User;
        }

        public GitHubAuthSession(GitHubApiResponse.Token token, GitHubApiResponse.User user)
        {
            Token = token;
            User = user;
            WriteToStore();
        }

        public void Clear()
        {
            DeleteStore();
        }
        
        private void DeleteStore()
        {
            if (Store.FileExists(Config.SessionFile))
            {
                Store.DeleteFile(Config.SessionFile);
            }
        }

        private GitHubAuthSession? LoadFromStore()
        {
            if (Store.FileExists(Config.SessionFile))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream authFileStream = Store.OpenFile(Config.SessionFile, FileMode.Open);
                try
                {
                    var savedSession = formatter.Deserialize(authFileStream) as GitHubAuthSession;
                    authFileStream.Close();
                    return savedSession;

                }
                catch (SerializationException)
                {
                    authFileStream.Close();
                    DeleteStore();
                }
            }
            // else + catch
            return null;
        }
        
        private void WriteToStore()
        {
            Stream saveFileStream = Store.CreateFile(Config.SessionFile);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, this);
            saveFileStream.Close();
        }
    }
}

