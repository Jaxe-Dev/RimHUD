using System.IO;
using System.Xml.Linq;
using RimHUD.Interface;
using Verse;

namespace RimHUD.Data
{
    internal static class Persistent
    {
        private const string ConfigDirectoryName = "RimHUD";
        private const string ConfigFileName = "Config.xml";

        private static readonly DirectoryInfo ConfigDirectory = new DirectoryInfo(GenFilePaths.ConfigFolderPath).CreateSubdirectory(ConfigDirectoryName);
        private static readonly FileInfo ConfigFile = new FileInfo(Path.Combine(ConfigDirectory.FullName, ConfigFileName));

        public static void Save()
        {
            var doc = new XDocument();

            var theme = Theme.ToXml();
            theme.Add(new XAttribute("Version", Mod.Version));
            doc.Add(theme);

            doc.Save(ConfigFile.FullName);
        }

        public static void Load()
        {
            if (!HasConfigFile())
            {
                Save();
                return;
            }

            var doc = XDocument.Load(ConfigFile.FullName);

            Theme.FromXml(doc.Root);
            var loadedVersion = doc.Root?.Attribute("Version")?.Value;
            if (loadedVersion != Mod.Version) { Mod.Warning($"Loaded config version ({loadedVersion ?? "NULL"}) is different from the current mod version"); }
        }

        private static bool HasConfigFile()
        {
            ConfigFile.Refresh();
            return ConfigFile.Exists;
        }
    }
}
