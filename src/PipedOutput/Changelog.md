# Changelog

## [1.4.1.0]

### Fixed
- removed individual patches altogether and combined them in a single patch; this should fix crashes under Linux/Mac

## [1.4.0.0]

### Dev
- updated references to Harmony 2
- removed invalid patches to DoPostConfigurePreview and DoPostConfigureUnderConstruction (can patch only method that were explicitly inherited)
- added these patches to DoPostConfigureComplete instead
- added RustDeoxidizer (testing was successful)
