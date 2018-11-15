# RimHUD
![Mod Version](https://img.shields.io/badge/Mod_Version-1.3.2-blue.svg)
![RimWorld Version](https://img.shields.io/badge/Built_for_RimWorld-1.0-blue.svg)
![Harmony Version](https://img.shields.io/badge/Powered_by_Harmony-1.2.0.1-blue.svg)\
![Steam Subscribers](https://img.shields.io/badge/dynamic/xml.svg?label=Steam+Subscribers&query=//table/tr[2]/td[1]&colorB=blue&url=https://steamcommunity.com/sharedfiles/filedetails/%3Fid=1508850027&suffix=+total)
![GitHub Downloads](https://img.shields.io/github/downloads/Jaxe-Dev/RimHUD/total.svg?colorB=blue&label=GitHub+Downloads)

[Link to Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027)\
[Link to Ludeon Forum thread](https://ludeon.com/forums/index.php?topic=45787.0)

---

RimHUD is a UI mod that displays a detailed information about a selected character or creature. The HUD display is integrated into the inspect pane which can be resized to fit the additional information. Alternatively the HUD can a separate floating window and docked to any position on the screen.

Visual warnings will appear if a pawn has any life threatening conditions, has wounds that need tending or is close to a mental breakdown.

---

##### STEAM INSTALLATION
- **[Go to the Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027) and subscribe to the mod.**

---

##### NON-STEAM INSTALLATION
- **[Download the latest release](https://github.com/Jaxe-Dev/RimHUD/releases/latest) and unzip it into your *RimWorld/Mods* folder.**

---

**Note to Translators**: Save your energy for now as there are a few upcoming updates that may add, remove or modify the key names and strings.

---

The following base methods are patched with Harmony:
```
Prefix* : RimWorld.InspectPaneFiller.DoPaneContentsFor
Prefix* : RimWorld.InspectPaneUtility.DoTabs
Prefix* : RimWorld.InspectPaneUtility.InspectPaneOnGUI
Prefix* : RimWorld.InspectPaneUtility.PaneSizeFor
Postfix : RimWorld.InspectPaneUtility.PaneWidthFor
Prefix* : RimWorld.ITab.PaneTopY
Prefix* : RimWorld.MainTabWindow_Inspect.PaneTopY
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Prefix* : RimWorld.Tutor.TutorOnGUI
Postfix : Verse.Game.FinalizeInit
Prefix* : Verse.LetterStack.LettersOnGUI
Prefix  : Verse.MapInterface.MapInterfaceOnGUI_AfterMainTabs
Postfix : Verse.MapInterface.Notify_SwitchedMap
Postfix : Verse.PlayDataLoader.DoPlayLoad
Prefix  : Verse.Profile.MemoryUtility.ClearAllMapsAndWorld

A prefix marked by a * denotes that in some circumstances the original method will be bypassed
```
