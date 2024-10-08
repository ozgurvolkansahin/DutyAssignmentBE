using System.Reflection;
using DotNetEnv;

namespace DutyAssignment.Configuration.Config
{
    public class Config
    {
        public static string AppVersion = string.Empty;
        public static readonly string EnvName;
        public static string MongoUrl { get => Env.GetString("MONGO_URL"); }
        public static string DatabaseName { get => Env.GetString("DATABASE_NAME"); }

        static Config()
        {
            // get config values from .env file
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var assemblyName = entryAssembly.GetName();
                if (assemblyName != null)
                {
                    var version = assemblyName.Version;
                    if (version != null)
                    {
                        AppVersion = version.ToString();
                    }
                }
            }
            EnvName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "default_environment";
            LoadOptions options = Env.TraversePath();
            List<KeyValuePair<string, string>> list = Env.Load(".env", options).ToList();
            string path = (".env." + EnvName).TrimEnd('.');
            list.AddRange(Env.Load(path, options));
        }
    }
}


