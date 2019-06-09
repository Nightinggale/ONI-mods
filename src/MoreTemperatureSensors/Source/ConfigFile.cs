using System;
using System.IO;
using Newtonsoft.Json;

namespace MoreTemperatureSensors
{
    class MoreTemperatureSensorsConfig
    {
        public int ItemSensorItemCountFastThreshold = 3;
        public float ItemSensorUpdateIntervalSeconds = 0.15f;
        public float BatterySensorUpdateIntervalSeconds = 0.15f;
        public float GasPressureSensorMax = 50f;
        public float LiquidPressureSensorMax = 2000f;

        public float GetBatteryInterval
        {
            get
            {
                return BatterySensorUpdateIntervalSeconds <= 0.15f ? 0.15f : BatterySensorUpdateIntervalSeconds;
            }
        }

        public static void OnLoad(string modPath)
        {
            LoadConfig(modPath);
        }

        public static MoreTemperatureSensorsConfig Config = null;
        public static void LoadConfig(string modPath) { Config = LoadConfig<MoreTemperatureSensorsConfig>(Path.Combine(modPath, "MoreTemperatureSensorsConfig.json")); }

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
                Console.WriteLine("FAILED TO LOAD CONFIG FILE: " + ModInfo.Name);
                return (T)Activator.CreateInstance(typeof(T));
            }
            
        }
    }
}
