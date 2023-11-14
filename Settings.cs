using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace ValorantAgentPicker
{
    public class AppSettings
    {
        public bool returnRole { get; set; }
        public ConsoleColor userBackgroundColor { get; set; }
    }
    internal class Settings
    {
        public static void WriteSettings(AppSettings settingsToWrite)
        {
            string jsonString = JsonSerializer.Serialize(settingsToWrite);
            File.WriteAllText(Global.settingsFile, jsonString);
        }
        public static AppSettings ReadSettings()
        {
            if (File.Exists(Global.settingsFile))
            {
                string jsonString = File.ReadAllText(Global.settingsFile);
                AppSettings deserializedSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                return deserializedSettings;
            }
            AppSettings defaultSettings = new AppSettings
            {
                returnRole = true,
                userBackgroundColor = ConsoleColor.Black
            };
            return defaultSettings;
        }
    }
}
