using Newtonsoft.Json;

namespace Launcher
{
    class Config
    {
        // Singleton instance.
        public static Config I;

        public string Name { get; set; }
        public string Path { get; set; }
        public string ID { get; set; }
        public string Version { get; set; }
        public string Domain { get; set; }

        public static void Init()
        {
            var json = System.IO.File.ReadAllText("config.json");

            I = JsonConvert.DeserializeObject<Config>(json);
        }

        public void Save()
        {
            // Serialize config to JSON.
            string json = JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            System.IO.File.WriteAllText("config.json", json);
        }
    }
}
