using DotNetEnv;

namespace Zone_Radio.Utility
{
    internal sealed class ZEnv
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        public ZEnv()
        {
            Env.TraversePath().Load();
            string clientId = Env.GetString("CLIENT_ID");
            string clientSecret = Env.GetString("CLIENT_SECRET");

            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId))
            {
                throw new NullReferenceException("Failed to load .env variables");
            }

            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}
