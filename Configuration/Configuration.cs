// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.IO;
using System.Text.Json;

namespace SimpleConfiguration
{
    public static class Configuration
    {
        public static ConfigurationSections Sections { get; set; }

        private static string JsonFileName;

        static Configuration()
        {
            Sections = new ConfigurationSections();
        }

        public static void Load(string jsonFileName)
        {
            JsonFileName = jsonFileName;
            if (File.Exists(JsonFileName))
            {
                var jsonString = File.ReadAllText(JsonFileName);
                Sections = JsonSerializer.Deserialize<ConfigurationSections>(jsonString);
            }
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(Sections, options);
            File.WriteAllText(JsonFileName, jsonString);
        }

        public static void SaveAs(string jsonFileName)
        {
            JsonFileName = jsonFileName;
            Save();
        }

    }
}
