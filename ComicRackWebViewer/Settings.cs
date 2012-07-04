using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace ComicRackWebViewer
{
    public static class Settings
    {
        private const string DIRECTORY = "ComicRackWebViewer";
        private const string SETTINGS_FILE = "ComicRackWebViewer\\settings.txt";

        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        static Settings()
        {
            var file = GetIsoFile();
            if (file.FileExists(SETTINGS_FILE))
            {
                using (var reader = new StreamReader(file.OpenFile(SETTINGS_FILE, FileMode.Open)))
                {
                    string line;
                    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        var setting = line.Split(',');
                        if (setting.Length == 2)
                        {
                            settings.Add(setting[0], setting[1]);
                        }
                    }
                }
            }
        }

        public static string GetSetting(string key)
        {
            string value;
            if (settings.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        public static void SaveSetting(string key, string value)
        {
            settings[key] = value;
            SaveAll();
        }

        private static IsolatedStorageFile GetIsoFile()
        {
            var file = IsolatedStorageFile.GetUserStoreForDomain();
            if (!file.DirectoryExists(DIRECTORY))
            {
                file.CreateDirectory(DIRECTORY);
            }
            return file;
        }

        private static void SaveAll()
        {
            var file = GetIsoFile();
            using (var writer = new StreamWriter(file.OpenFile(SETTINGS_FILE, FileMode.OpenOrCreate)))
            {
                foreach (var kvp in settings)
                {
                    writer.WriteLine(kvp.Key + "," + kvp.Value);
                }
            }
        }
    }
}
