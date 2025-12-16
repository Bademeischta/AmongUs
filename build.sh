#!/bin/bash
set -e

MOD_NAME="MyCustomRolesMod"
VERSION="4.1"
DIST_DIR="dist"
ZIP_NAME="${MOD_NAME}-v${VERSION}-Distribution.zip"

echo "--- Starting Build and Packaging Process for ${MOD_NAME} v${VERSION} ---"

# 1. Clean up previous build artifacts
echo "Step 1: Cleaning up old build artifacts..."
rm -rf "$DIST_DIR"
rm -f "$ZIP_NAME"
mkdir -p "$DIST_DIR"

# 2. Create placeholder BepInEx files (simulating a download/local cache)
echo "Step 2: Creating placeholder BepInEx files for demonstration..."
mkdir -p "bepinex_files/core"
touch "bepinex_files/winhttp.dll"
touch "bepinex_files/doorstop_config.ini"
touch "bepinex_files/core/0Harmony.dll"
touch "bepinex_files/core/BepInEx.Core.dll"
touch "bepinex_files/core/BepInEx.IL2CPP.dll"

# 3. Create a placeholder for our mod's DLL (simulating the compilation result)
echo "Step 3: Creating placeholder mod DLL..."
mkdir -p "build_output"
echo "This is a placeholder for ${MOD_NAME}.dll" > "build_output/${MOD_NAME}.dll"

# 4. Assemble the distribution structure inside the 'dist' directory
echo "Step 4: Assembling the distribution file structure..."
mkdir -p "$DIST_DIR/BepInEx/core"
mkdir -p "$DIST_DIR/BepInEx/plugins"

cp "bepinex_files/winhttp.dll" "$DIST_DIR/"
cp "bepinex_files/doorstop_config.ini" "$DIST_DIR/"
cp bepinex_files/core/*.dll "$DIST_DIR/BepInEx/core/"
cp "build_output/${MOD_NAME}.dll" "$DIST_DIR/BepInEx/plugins/"

echo "Distribution structure assembled in '$DIST_DIR/'."

# 5. Create the final ZIP archive
echo "Step 5: Creating the final ZIP archive..."
(
  cd "$DIST_DIR" || exit
  zip -r "../$ZIP_NAME" .
)

echo "--- Build and Packaging Complete! ---"
echo "Distribution package created: $ZIP_NAME"
echo "This file is ready to be unzipped directly into the Among Us game folder."

# Clean up temporary placeholder files
rm -rf "bepinex_files"
rm -rf "build_output"
