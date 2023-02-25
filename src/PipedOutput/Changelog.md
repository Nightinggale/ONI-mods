﻿# Changelog

## [1.4.4.2]
- removed changes to Electrolyzer max pressure; if you need this get CustomizedBuildings

## [1.4.4.1]
- recompiled

## [1.4.4.0]
- increased Oxygen Diffuser and RustDeoxidizer max pressure from to 4kg
- added code to force manual delivery to RustDeoxidizer regardless of state

## [1.4.3.1]
- removed logging

## [1.4.3.0]
- added option to exclude buildings found here: %userprofile%\documents\Klei\OxygenNotIncluded\mods\PipedOutput.json

## [1.4.2.0]
- increased Electrolyzer max pressure from 1.8kg to 4kg

## [1.4.1.0]
- removed individual patches altogether and combined them in a single patch; this should fix crashes under Linux/Mac

## [1.4.0.0]
- updated references to Harmony 2
- removed invalid patches to DoPostConfigurePreview and DoPostConfigureUnderConstruction (can patch only method that were explicitly inherited)
- added these patches to DoPostConfigureComplete instead
- added RustDeoxidizer (testing was successful)