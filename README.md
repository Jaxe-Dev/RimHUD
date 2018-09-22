# RimHUD
![](https://img.shields.io/badge/Mod_Version-1.1.0.1-blue.svg)
![](https://img.shields.io/badge/Built_for_RimWorld-B19-blue.svg)
![](https://img.shields.io/badge/Powered_by_Harmony-1.2.0.1-blue.svg)

[Link to Steam Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=1508850027)
[Link to Ludeon Forum thread](https://ludeon.com/forums/index.php?topic=45787.0)

---

RimHUD is a UI mod that displays a detailed information about a selected character or creature. The HUD display is integrated into the inspect pane which can be resized to fit the additional information. Alternatively the HUD can a separate floating window and docked to any position on the screen.

Visual warnings will appear if a pawn has any life threatening conditions, has wounds that need tending or is close to a mental breakdown.

---

##### INSTALLATION
- **[Download the latest release](https://github.com/Jaxe-Dev/RimHUD/releases/latest) and unzip it into your *RimWorld/Mods* folder.**

---

**Translators**: Save your energy for now as there are a few upcoming updates that may add, remove or modify the key names and strings.

---

The following base methods are patched with Harmony:
```
Prefix* : RimWorld.InspectPaneFiller.DoPaneContentsFor
Prefix* : RimWorld.InspectPaneUtility.DoTabs
Prefix* : RimWorld.InspectPaneUtility.InspectPaneOnGUI
Prefix* : RimWorld.InspectPaneUtility.PaneSizeFor
Postfix : RimWorld.InspectPaneUtility.PaneWidthFor
Prefix* : RimWorld.ITab.PaneTopY
Postfix : RimWorld.MainTabWindow.Inspect_DoInspectPaneButtons
Prefix* : RimWorld.MainTabWindow_Inspect.PaneTopY
Postfix : RimWorld.PlaySettings.DoPlaySettingsGlobalControls
Prefix* : RimWorld.Tutor.TutorOnGUI
Prefix* : Verse.LetterStack.LettersOnGUI
Prefix  : Verse.MapInterface.MapInterfaceOnGUI_AfterMainTabs
Prefix  : Verse.Profile.MemoryUtility.ClearAllMapsAndWorld

A prefix marked by a * means it in some circumstances it will bypass the original method
```
