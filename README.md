# RimHUD
![](https://img.shields.io/badge/Mod_Version-1.0.0-brightgreen.svg)
![](https://img.shields.io/badge/Built_for_RimWorld-B19-brightgreen.svg)
![](https://img.shields.io/badge/Powered_by_Harmony-1.2.0.1-brightgreen.svg)

[Link to Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1503185309)

---

RimHUD is a mod that displays a compact window containing detailed information about a selected character or creature. The window stays up as long as a pawn is selected and provides useful information that would otherwise require scrolling through character tabs.

Visual warnings will appear if a pawn has any life threatening conditions, wounds that need tending or potentially unstable mental states.

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
