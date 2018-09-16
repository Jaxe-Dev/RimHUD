# RimHUD
![](https://img.shields.io/badge/Mod_Version-1.0.1-blue.svg)
![](https://img.shields.io/badge/Built_for_RimWorld-B19-blue.svg)
![](https://img.shields.io/badge/Powered_by_Harmony-1.2.0.1-blue.svg)

[Link to Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1503185309)

---

RimHUD is a mod that displays a compact window containing detailed information about a selected character or creature. The window stays up as long as a pawn is selected and provides useful information that would otherwise require scrolling through character tabs.

Visual warnings will appear if a pawn has any life threatening conditions, has wounds that need tending or is close to a mental breakdown.

---

##### INSTALLATION
- **[Download the latest release](https://github.com/Jaxe-Dev/RimHUD/releases/latest) and unzip it into your *RimWorld/Mods* folder.**

---

The following base methods are patched with Harmony:
```C#
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Prefix  : RimWorld.Tutor.TutorOnGUI
Prefix  : Verse.LetterStack.LettersOnGUI
Postfix : Verse.MapInterface.MapInterfaceOnGUI_BeforeMainTabs
```
