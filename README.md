# RimHUD
![Mod Version](https://img.shields.io/badge/Mod_Version-1.10.2-blue.svg)
![RimWorld Version](https://img.shields.io/badge/Built_for_RimWorld-1.3-blue.svg)
![Harmony Version](https://img.shields.io/badge/Powered_by_Harmony-2.2-blue.svg)\
![Steam Downloads](https://img.shields.io/steam/downloads/1508850027?colorB=blue&label=Steam+Downloads)
![GitHub Downloads](https://img.shields.io/github/downloads/Jaxe-Dev/RimHUD/total?colorB=blue&label=GitHub+Downloads)

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
- **[Download the latest release](https://github.com/Jaxe-Dev/RimHUD/releases/latest) and unzip it into your *RimWorld\Mods* folder.**

---

**Note to Modders**: *Adding a layout preset in your mod's `RimHUD\Presets` folder will be automatically added to RimHUD's built-in presets.*

---

**Note to Translators**: *Although the offers are appreciated, for maintenance reasons only English will included with the base mod. All translations should be uploaded as language submods with full permission to use the mod's images in any way.*

---

The following base methods are patched with Harmony:
```
Prefix* : RimWorld.InspectPaneFiller.DoPaneContentsFor
Prefix* : RimWorld.InspectPaneUtility.DoTabs
Prefix* : RimWorld.InspectPaneUtility.InspectPaneOnGUI
Prefix* : RimWorld.InspectPaneUtility.PaneSizeFor
Postfix : RimWorld.InspectPaneUtility.PaneWidthFor
Prefix* : RimWorld.ITab.PaneTopY
Prefix  : RimWorld.MainTabWindow.InitialSize
Postfix : RimWorld.MainTabWindow.InitialSize
Prefix* : RimWorld.MainTabWindow_Inspect.PaneTopY
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Prefix* : RimWorld.Tutor.TutorOnGUI
Postfix : Verse.Game.FinalizeInit
Prefix* : Verse.LetterStack.LettersOnGUI
Prefix  : Verse.MapInterface.MapInterfaceOnGUI_AfterMainTabs
Postfix : Verse.MapInterface.Notify_SwitchedMap
Prefix  : Verse.Profile.MemoryUtility.ClearAllMapsAndWorld
```
*A prefix marked by a \* denotes that in some circumstances the original method will be bypassed*
