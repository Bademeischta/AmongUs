#!/bin/bash

set -e

MOD_NAME=MyCustomRolesMod
VERSION=4.1
DIST_DIR=dist
ZIP_NAME="${MOD_NAME}-v${VERSION}-Distribution.zip"

echo "=== Real Build Script for ${MOD_NAME} v${VERSION} ==="
echo

# Step 1: Check for .NET SDK
echo "Step 1: Checking for .NET SDK..."
if ! command -v dotnet &> /dev/null
then
    echo "ERROR: .NET SDK is not installed!"
    echo "Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi
echo "[OK] .NET SDK found."

# Step 2: Clean up old artifacts
echo
echo "Step 2: Cleaning up old artifacts..."
rm -rf "${DIST_DIR}" "${ZIP_NAME}" bin obj
mkdir -p "${DIST_DIR}"
echo "[OK] Cleanup complete."

# Step 3: Verify Game Assemblies
echo
echo "Step 3: Verifying game assemblies..."
GAME_LIB_DIR="lib/AmongUs"
REQUIRED_ASSEMBLIES=(
    "Assembly-CSharp.dll"
    "Il2Cppmscorlib.dll"
    "Hazel.dll"
    "UnityEngine.CoreModule.dll"
    "UnityEngine.UI.dll"
    "UnityEngine.TextRenderingModule.dll"
)
ALL_FILES_FOUND=true
for file in "${REQUIRED_ASSEMBLIES[@]}"; do
    if [ ! -f "${GAME_LIB_DIR}/${file}" ]; then
        echo "ERROR: Missing required game assembly: ${GAME_LIB_DIR}/${file}"
        ALL_FILES_FOUND=false
    fi
done

if [ "$ALL_FILES_FOUND" = false ]; then
    echo
    echo "Build failed. Please copy the required files into '${GAME_LIB_DIR}'."
    echo "See '${GAME_LIB_DIR}/README.md' for more details."
    exit 1
fi
echo "[OK] All game assemblies found."

# Step 4: Verify BepInEx Files
echo
echo "Step 4: Verifying BepInEx files..."
BEPINEX_DIR="bepinex_files"
if [ ! -d "${BEPINEX_DIR}" ]; then
    echo "ERROR: The '${BEPINEX_DIR}' directory is missing."
    echo "Please download BepInEx 6.0.0-pre.1 IL2CPP from:"
    echo "https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1"
    echo "Then, extract it to a folder named '${BEPINEX_DIR}' in this directory."
    exit 1
fi
echo "[OK] BepInEx directory found."


# Step 5: Build the mod
echo
echo "Step 5: Building the mod..."
dotnet build MyCustomRolesMod.csproj --configuration Release
DLL_PATH="bin/Release/net6.0/${MOD_NAME}.dll"
if [ ! -f "${DLL_PATH}" ]; then
    echo "ERROR: DLL was not created. Build failed!"
    exit 1
fi
DLL_SIZE=$(stat -c%s "${DLL_PATH}")
echo "[OK] Mod built successfully (${DLL_SIZE} bytes)."


# Step 6: Assemble distribution
echo
echo "Step 6: Assembling distribution package..."
mkdir -p "${DIST_DIR}/BepInEx/plugins"
cp "${DLL_PATH}" "${DIST_DIR}/BepInEx/plugins/"
cp -r "${BEPINEX_DIR}/." "${DIST_DIR}/"

# Create README for distribution
echo "Creating installation README..."
cat > "${DIST_DIR}/INSTALLATION_README.txt" << EOL
MyCustomRolesMod v${VERSION} - Installation Instructions
======================================================

1. Unzip this entire folder into your Among Us game directory.
   (The folder that contains "Among Us.exe")

2. Your Among Us folder should now contain:
   - winhttp.dll
   - doorstop_config.ini
   - BepInEx/ folder

3. Start the game. The mod is now active!
EOL

echo "[OK] Distribution package assembled in '${DIST_DIR}'."

# Step 7: Create ZIP
echo
echo "Step 7: Creating distribution ZIP..."
if command -v zip &> /dev/null; then
    (cd "${DIST_DIR}" && zip -r "../${ZIP_NAME}" .)
    echo "[OK] Created ${ZIP_NAME}"
else
    echo "WARNING: 'zip' command not found. ZIP creation skipped."
fi

echo
echo "=== BUILD COMPLETE ==="
echo
echo "The final distribution is available in: ${ZIP_NAME}"
echo "The mod DLL is also available at: ${DLL_PATH}"
echo
