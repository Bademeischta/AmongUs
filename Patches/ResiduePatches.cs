using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class ResiduePatches
    {
        // This patch will modify the kill logic for the Residue player.
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        [HarmonyPrefix]
        public static bool MurderPlayer_Prefix(PlayerControl __instance, PlayerControl target)
        {
            var role = RoleManager.Instance.GetRole(__instance.PlayerId);
            if (role?.RoleType == RoleType.Residue)
            {
                Networking.RpcManager.Instance.SendCmdResidueKill(target.PlayerId);
                return false; // Prevent the original kill logic from running
            }
            return true;
        }

        // This patch will handle the afterimage's movement and eventual collapse into a body.
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void FixedUpdate_Postfix(PlayerControl __instance)
        {
            var afterimage = __instance.gameObject.GetComponent<ResidueAfterimage>();
            if (ResidueManager.Instance.IsResidue(__instance.PlayerId))
            {
                if (afterimage == null)
                {
                    __instance.gameObject.AddComponent<ResidueAfterimage>();
                }
            }
            else
            {
                if (afterimage != null)
                {
                    Object.Destroy(afterimage);
                }
            }
        }

        private class ResidueAfterimage : MonoBehaviour
        {
            private PlayerControl _player;

            private void Awake()
            {
                _player = GetComponent<PlayerControl>();

                // Make the player look ghostly
                var renderer = _player.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    var color = renderer.color;
                    color.a = 0.5f;
                    renderer.color = color;
                }
            }

            private void OnDestroy()
            {
                // Restore the player's appearance
                if (_player != null)
                {
                    var renderer = _player.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        var color = renderer.color;
                        color.a = 1f;
                        renderer.color = color;
                    }
                }
            }
        }
    }
}
