using System;
using System.IO;
using RimHUD.Data.Extensions;
using RimHUD.Data.Storage;
using RimHUD.Interface.HUD;
using Verse;

namespace RimHUD.Data.Integration
{
    internal class LayoutPreset
    {
        public const string RootElementName = "Preset";
        public const string VersionAttributeName = "Version";

        public static LayoutPreset[] FixedList { get; } = Persistent.GetFixedPresets();
        public static LayoutPreset[] UserList { get; private set; } = Persistent.GetUserPresets();

        public FileInfo File { get; }
        public string Name { get; }
        public string Mod { get; }
        public string Label => Name + " " + Mod.Size(Theme.Theme.SmallTextStyle.ActualSize).Italic();
        public bool IsUserMade { get; }

        private LayoutPreset(string name, string mod, FileInfo file)
        {
            File = file;
            Name = name;
            IsUserMade = mod == null;
            Mod = mod ?? Lang.Get("Layout.UserPreset");
        }

        public static LayoutPreset Prepare(ModContentPack mod, FileInfo file)
        {
            var isBuiltIn = mod == RimHUD.Mod.ContentPack;
            var modName = isBuiltIn ? Lang.Get("Layout.BuiltIn") : mod?.Name;
            var name = file.NameWithoutExtension();

            var xml = Persistent.LoadXml(file);
            if (xml == null)
            {
                RimHUD.Mod.Warning($"'{file.FullName}' is an invalid RimHUD preset file");
                return null;
            }

            var versionText = xml.Attribute(VersionAttributeName)?.Value;

            var preset = new LayoutPreset(name, modName, file);
            if (isBuiltIn) { return preset; }

            if (versionText == null)
            {
                RimHUD.Mod.Warning($"{name} ({modName}) is does not contain a version check");
                return preset;
            }
            var version = new Version(versionText);
            if (new Version(RimHUD.Mod.Version).ComparePartial(version) == 1) { RimHUD.Mod.Warning($"{name} {(modName == null ? null : "(" + modName + ")")} was built for a lower version of RimHUD ({versionText})"); }

            return preset;
        }

        public static void RefreshUserPresets() => UserList = Persistent.GetUserPresets();

        public bool Load()
        {
            if (!File.ExistsNow())
            {
                RefreshUserPresets();
                return false;
            }

            var xml = Persistent.LoadXml(File);
            if (xml == null)
            {
                RimHUD.Mod.Error($"Unable to load preset '{Label}'");
                return false;
            }

            var docked = xml.Element(HudLayout.DockedElementName);
            var floating = xml.Element(HudLayout.FloatingElementName);

            if (docked != null) { HudLayout.Docked = HudLayout.FromXml(docked); }
            if (floating != null) { HudLayout.Floating = HudLayout.FromXml(floating); }

            Persistent.Save();

            return true;
        }
    }
}
