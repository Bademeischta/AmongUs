# Missing Game Assemblies

**Note:** The recommended way to build this mod is to set the `AMONG_US_GAME_PATH` environment variable. See the main `README.md` for details.

If you choose not to set the environment variable, you must manually copy the required Among Us game assemblies into this directory.

## Required Files

Please copy the following files from your local Among Us installation (`<YourSteamPath>/steamapps/common/Among Us/Among Us_Data/Managed/`) into this directory:

*   `Assembly-CSharp.dll`
*   `Il2Cppmscorlib.dll`
*   `Hazel.dll`
*   `UnityEngine.CoreModule.dll`
*   `UnityEngine.UI.dll`
*   `UnityEngine.TextRenderingModule.dll`

Once these files are placed here, the `build-real.sh` script will be able to compile the mod successfully.
