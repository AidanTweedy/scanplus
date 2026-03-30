# ScanPlus

[![GitHub Build Status](https://img.shields.io/github/actions/workflow/status/AidanTweedy/scanplus/build.yml?style=for-the-badge&logo=github)](https://github.com/AidanTweedy/scanplus/actions/workflows/build.yml)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/KiwiPositive/ScanPlus?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/KiwiPositive/ScanPlus/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/KiwiPositive/ScanPlus?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/KiwiPositive/ScanPlus/)

A mod for *Lethal Company* which upgrades the ship's scanner, allowing it to scan lifeforms in addition to scrap.

![ScanPlus](https://i.imgur.com/UQVOitx.png)

## Installation

Place `ScanPlus.dll` in your `BepInEx/plugins` folder.

Alternatively, you can use Thunderstore Mod Manager.

## Usage

To use the upgraded scanner, players must first purchase the `Infrared Scanner` ship upgrade. Once purchased, they can use the `irscan` terminal command to display the creatures currently spawned on the moon.

This upgrade requirement can be disabled in the config file (see below), allowing players to use the enhanced scan without purchasing the upgrade.

The terminal command used to activate the `Infrared Scanner` can also be customized in the config file. Several default command aliases are included by default.

## Configuration

`ScanPlus.cfg` will be generated after runnning *Lethal Company* for the first time after installing this mod, allowing for several changes to be made:

* Switch between `Low`, `Medium`, `High`, and `Excessive` levels of detail in the upgraded scan messages. 

* Enable/Disable need for ship upgrade to use enhanced scanner.

* Adjust price of the `Infrared Scanner` ship upgrade (if enabled).

* Change terminal command(s) used to activate the `Infrared Scanner`.

## Bugs/Troubleshooting/Suggestions

Found a bug, encountered a problem, or have a suggestion? - let us know on [GitHub!](https://github.com/AidanTweedy/scanplus/issues)
