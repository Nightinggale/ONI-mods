using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace MoreTemperatureSensors
{
    class MoreTemperatureSensorsConfig
    {
        private const string fileName = "MoreTemperatureSensorsConfig.json";

        public int ItemSensorItemCountFastThreshold = 3;
        public float ItemSensorUpdateIntervalSeconds = 0.15f;
        public float BatterySensorUpdateIntervalSeconds = 0.15f;
        public float GasPressureSensorMax = 50f;
        public float LiquidPressureSensorMax = 2000f;

        public float GetItemInterval
        {
            get
            {
                return ItemSensorUpdateIntervalSeconds <= 0.15f ? 0.15f : ItemSensorUpdateIntervalSeconds;
            }
        }

        public float GetBatteryInterval
        {
            get
            {
                return BatterySensorUpdateIntervalSeconds <= 0.15f ? 0.15f : BatterySensorUpdateIntervalSeconds;
            }
        }

        // end of config file variables

        private static MoreTemperatureSensorsConfig internalConfig = null;

        public static MoreTemperatureSensorsConfig Config
        {
            get
            {
                if (internalConfig == null)
                {
                    // load the config file, but only the first time the method is called.
                    string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    internalConfig = LoadConfig<MoreTemperatureSensorsConfig>(Path.Combine(assemblyFolder, fileName));
                }
                return internalConfig;
            }
        }

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
