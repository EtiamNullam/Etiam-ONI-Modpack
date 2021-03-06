# ONI Modpack by Etiam

A bunch of mods made for [Klei](https://www.klei.com/)'s Oxygen Not Included.

![modpack_preview.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/Modpack.png)

## [Thread on Klei's Forum](https://forums.kleientertainment.com/forums/topic/101902-mods-etiams-modpack/)

## Installation

### Steam Workshop

Most of my mods are now available on [My Steam Workshop](https://steamcommunity.com/profiles/76561197993782918/myworkshopfiles/?appid=457140), so simply subscribe to the mods you want, then enable them in-game (`Main Menu -> Mods`).

### Alternative

If you prefer to not use the steam workshop:
- Use this [Download Link](https://github.com/EtiamNullam/Etiam-ONI-Modpack/archive/master.zip) or press `"Clone or download" -> "Download ZIP"`.
- Extract to `Documents/Klei/OxygenNotIncluded/mods/Local` (on Windows, not sure about other platforms)

NOTE: As Steam Workshop approach is clearly superior I'm not going focus too much on the old way of installation.

NOTE: You should avoid having two different instances of one mod installed, it will most likely crash.

### Modloader

Does NOT require the [Modloader](https://github.com/javisar/ONI-Modloader) anymore, but should still work with it.

## Donate

[![Support me on Patreon](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/become-a-patron.png)](https://www.patreon.com/bePatron?u=16564340)

## FAQ

### Game crashes with mod installed

Its most likely Steam Workshop gives you old version of the mod. You can use [Mod Updater](https://steamcommunity.com/sharedfiles/filedetails/?id=2018291283) or [check mod version manually](#How-to-check-mod-version) and make sure you have the latest version.

If Steam Workshop keeps giving you outdated mod you might want to download certain mod [directly from GitHub](https://github.com/EtiamNullam/Etiam-ONI-Modpack/tree/master/Mods).

If there was some recent update to Oxygen Not Included it might broke one of my mods and I might not know about it yet. If you think this is the case [Submit a new issue](https://github.com/EtiamNullam/Etiam-ONI-Modpack/issues/new/choose) explain the problem and provide `Player.log`. First check if there is similar issue open - I might need your help dealing with that crash there.

### How to check mod version

There are two ways:

- Look in `output_log.txt` (located in `~/AppData/LocalLow/Klei/Oxygen Not Included/output_log.txt`) after launching game for line this:

```
[21:09:11.057] [1] [INFO] GasOverlay: Initialized successfully. Version: 2.6.0.0, Path: C:\Users\<username>\Documents\Klei\OxygenNotIncluded\mods\Steam\1737859934, Config root: C:\Users\<username>\Documents\Klei\OxygenNotIncluded\mods\Steam\1737859934
```

- Find mod directory (for mod from Steam Workshop it will be inside `~\Documents\Klei\OxygenNotIncluded\mods\Steam`) then check `Properties -> Details` of the file or just hover mouse over it and look for `File version` or `Product version`.

### Where to find game logs

Game logs are very useful for troubleshooting so make sure to attach them when you are asking for help with errors or crashes.

Windows: `%AppData%\..\LocalLow\klei\Oxygen Not Included\Player.log`

MacOS: `~/Library/Logs/Klei/Oxygen Not Included/Player.log`

Linux: `~/.config/unity3d/Klei/Oxygen Not Included/Player.log`

More info: [Logs and Useful Information for Bug-Reports](https://support.klei.com/hc/en-us/articles/360029555392-Logs-and-Useful-Information-for-Bug-Reports)

### If everything else fails

If you can't find a solution for your problem go ahead and create a new issue. I will try to help the best I can.

## Special thanks to:
- [@pardeike](https://github.com/pardeike) for his patching library - [Harmony](https://github.com/pardeike/Harmony)
- [@javisar](https://github.com/javisar) for taking care of my mods in [his repo](https://github.com/javisar/ONI-Modloader-Mods) while I was absent and for the [ModLoader](https://github.com/javisar/ONI-Modloader)
- [@Cairath](https://github.com/Cairath) for being a great help during mod development and for [creating plenty of cool mods](https://github.com/Cairath/ONI-Mods)
- [@Killface80](https://github.com/Killface1980) for porting my mods to ModLoader and for great contributions to the GasOverlay
- [@Moonkis](https://github.com/Moonkis) for OnionPatcher and its merge with my modpack
- Everyone else involved

## Mods preview

NOTE: Many of my mods are configurable through `.json` files inside their directories. Changes are reflected immediately as soon as you save the file (where applicable).

### MaterialColor

Highly customizable mod that shows what buildings are made from by tinting them accordingly.

![lavatories.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/lavatories.png)

![blocks.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/blocks.png)

![forges.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/forges.png)

![mushers.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/mushers.png)

![pipes.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/pipes.png)

[AvailableMaterials.txt](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/MaterialColor/AvailableMaterials.txt) - This will come in handy if you want to override custom materials.

### GasOverlay

Replaces vanilla "breathable overlay" with an overlay which shows every gas in its own, easily distinguishable color.

![natural-overview.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/GasOverlay/natural-overview.png)

![gas-preview.png](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/GasOverlay/gas-preview.png)

![meteor-shower.gif](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/GasOverlay/meteor-shower.gif)

![electrolyzer-show.gif](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/GasOverlay/electrolyzers.gif)

### FreeCamera

Allows camera to move out of world boundaries without debug mode. Helps with working around the edges.

Now also allows to zoom out to see the whole map without using screenshot or debug mode.

![free-camera.png](https://i.imgur.com/EotAJcg.png)

### RemoveDailyReports

Removes old daily reports every night to decrease saving time and save file size. Might be useful on very high cycle count worlds.

### ClearRegolith

Removes 90% of stored regolith every time you load the save. More like an utility than actual mod, so its available from the [Tools](Tools) directory.

### CustomTemperatureOverlay

Extends vanilla temperature ranges and colors to make extreme temperatures easier to read.
It's also configurable, so you can tweak it to your needs.

### SmartLockerDropPort

Adds additional input port to the Smart Locker, when supplied with a signal makes the locker drop its contents.
Often used in a combination with a door, sometimes a buffer will be needed too.
Some examples:

#### External input

![External Input Example](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/SmartLockerDropPort/1.gif)

#### Self (with a buffer)

![Self Example](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/SmartLockerDropPort/2.gif)

### ChainedDeconstruction

Allows to deconstruct many ladders, tubes or firepoles at once.
Deconstructing one of them will deconstruct all others connected and marked for deconstruction.

![ChainedDeconstruction Preview](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/ChainedDeconstruction/chained-deconstruction.gif)

### LiquefiableSculptures

Allows to build Ice Sculptures from any liquefiable materials, for example Carbon Dioxide or Chlorine.

Works well with MaterialColor mod.

![Chlorine Sculpture Example](https://i.imgur.com/huwisRA.png)

### DisplayAllTemps

Always shows temperature in all units: celsius, fahrenheit and kelvin.

![All temps](https://i.imgur.com/o61vNsE.png)

### DoorIcons

Displays the icon on top of each door which indicates current state or access restrictions.

![DoorIcons Preview](https://github.com/EtiamNullam/Etiam-ONI-Modpack-Previews/blob/master/DoorIcons/door_access.gif)
