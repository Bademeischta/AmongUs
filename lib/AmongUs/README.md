# Missing Game Assemblies

This directory is a placeholder for the required Among Us game assemblies. To build this mod, you must provide these files yourself.

## Required Files

Please copy the following files from your local Among Us installation (`<YourSteamPath>/steamapps/common/Among Us/Among Us_Data/Managed/`) into this directory:

*   `Assembly-CSharp.dll`
*   `Il2Cppmscorlib.dll`
*   `Hazel.dll`
*   `UnityEngine.CoreModule.dll`
*   `UnityEngine.UI.dll`
*   `UnityEngine.TextRenderingModule.dll`

Once these files are placed here, the `build-real.sh` script will be able to compile the mod successfully.
