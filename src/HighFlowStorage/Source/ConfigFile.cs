using System;
using System.IO;
using Newtonsoft.Json;

namespace HighFlowStorage
{
    class HighFlowStorageConfig
    {
        public float liquidStorageCapacity = 5000f;
        public float liquidStorageConstructionTime = 240f;
        public float liquidStorageMetalCost = 800f;

        public float Gas3StorageCapacity = 300f;
        public float Gas3StorageConstructionTime = 240f;
        public float Gas3StorageMetalCost = 800f;

        public float Gas5StorageCapacity = 300f;
        public float Gas5StorageConstructionTime = 240f;
        public float Gas5StorageMetalCost = 800f;

        public static void OnLoad(string modPath)
        {
            LoadConfig(modPath);
        }

        public static HighFlowStorageConfig Config = null;
        public static void LoadConfig(string modPath) { Config = LoadConfig<HighFlowStorageConfig>(Path.Combine(modPath, "HighFlowStorageConfig.json")); }

        // modified version of https://github.com/javisar/ONI-Modloader-Mods/blob/Q3-Steam/Source/SpeedControl/SpeedControlConfig.cs
        // added error handling to make the game work despite missing/malformated json files or missing variables in said files

        protected static T LoadConfig<T>(string path) where T : class
        {
            try
            {
                JsonSerializer serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { Formatting = Formatting.Indented, ObjectCreationHandling = ObjectCreationHandling.Replace });
                
                // assign default values in case something isn't mentioned in the file
                T result = (T)Activator.CreateInstance(typeof(T));
                using (StreamReader streamReader = new StreamReader(path))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                    {
                        result = serializer.Deserialize<T>(jsonReader);
                        jsonReader.Close();
                    }
                    streamReader.Close();
                }
                return result;
            }
            catch
            {
                // something went wrong while loading the file, possibly missing or malformated file
                // return the default values
                return (T)Activator.CreateInstance(typeof(T));
            }
            
        }
    }
}
