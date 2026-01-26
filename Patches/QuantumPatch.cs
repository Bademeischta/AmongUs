using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class QuantumPatch
    {
        private static GameObject decoherenceButton;
        private static TMPro.TextMeshPro buttonText;
        private static float observationCheckCooldown = 0f;

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void FixedUpdate(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            var role = RoleManager.Instance.GetRole(__instance.PlayerId);
            if (role?.RoleType != RoleType.Quantum) return;

            observationCheckCooldown -= Time.fixedDeltaTime;
            if (observationCheckCooldown <= 0f)
            {
                observationCheckCooldown = 0.1f; // Check every 100ms
                bool isObserved = false;
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.PlayerId == __instance.PlayerId || p.Data.IsDead || p.Data.IsImpostor) continue;
                    if (p.CanSeePlayer(__instance))
                    {
                        isObserved = true;
                        break;
                    }
                }
                bool previousIsObserved = QuantumManager.Instance.IsObserved;
                if (previousIsObserved != isObserved)
                {
                    var writer = Hazel.MessageWriter.Get(Hazel.SendOption.Reliable);
                    writer.StartMessage((byte)MyCustomRolesMod.Networking.Packets.RpcType.CmdQuantumStateChange);
                    writer.Write(isObserved);
                    writer.EndMessage();
                    AmongUsClient.Instance.SendOrDisconnect(writer);
                    writer.Recycle();
                }
            }

            if (QuantumManager.Instance.IsChanneling)
            {
                QuantumManager.Instance.Update(Time.fixedDeltaTime);
                if (!QuantumManager.Instance.IsChanneling) // Channeling just finished
                {
                    var writer = Hazel.MessageWriter.Get(Hazel.SendOption.Reliable);
                    writer.StartMessage((byte)MyCustomRolesMod.Networking.Packets.RpcType.CmdQuantumCollapse);
                    writer.Write(__instance.PlayerId);
                    writer.Write(QuantumManager.Instance.ObservedTargetId.Value);
                    writer.EndMessage();
                    AmongUsClient.Instance.SendOrDisconnect(writer);
                    writer.Recycle();
                    QuantumManager.Instance.CancelCollapse();
                }
            }

            UpdateDecoherenceButtonState();
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void HudUpdate(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;
            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);

            if (role?.RoleType == RoleType.Quantum && !localPlayer.Data.IsDead)
            {
                if (decoherenceButton == null && __instance.ReportButton != null)
                {
                    CreateDecoherenceButton(__instance);
                }
                if (decoherenceButton != null) decoherenceButton.SetActive(true);
            }
            else
            {
                if (decoherenceButton != null) decoherenceButton.SetActive(false);
            }
        }

        private static void CreateDecoherenceButton(HudManager hud)
        {
            decoherenceButton = Object.Instantiate(hud.ReportButton.gameObject, hud.ReportButton.transform.parent);
            decoherenceButton.name = "DecoherenceButton";
            decoherenceButton.transform.localPosition = new Vector3(2.5f, 0.8f, 0);

            var passiveButton = decoherenceButton.GetComponent<PassiveButton>();
            passiveButton.OnClick.RemoveAllListeners();
            passiveButton.OnClick.AddListener(OnDecoherenceButtonClick);

            buttonText = decoherenceButton.GetComponentInChildren<TMPro.TextMeshPro>();
            if (buttonText != null) buttonText.text = "Select";
        }

        private static void OnDecoherenceButtonClick()
        {
            var manager = QuantumManager.Instance;
            if (manager.IsObserved || manager.IsChanneling) return;

            var localPlayer = PlayerControl.LocalPlayer;
            PlayerControl closestPlayer = null;
            float minDistance = float.MaxValue;

            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p.PlayerId == localPlayer.PlayerId || p.Data.IsDead || p.Data.IsImpostor) continue;
                float dist = Vector2.Distance(localPlayer.GetTruePosition(), p.GetTruePosition());
                if (dist < minDistance && dist <= GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                {
                    minDistance = dist;
                    closestPlayer = p;
                }
            }

            if (closestPlayer != null)
            {
                manager.SelectTarget(closestPlayer.PlayerId);
            }
        }

        private static void UpdateDecoherenceButtonState()
        {
            if (decoherenceButton == null) return;

            var manager = QuantumManager.Instance;
            var hudButton = decoherenceButton.GetComponent<HudButton>();

            hudButton.SetDisabled(manager.IsObserved || manager.IsChanneling);

            if (manager.IsChanneling)
            {
                buttonText.text = "Collapsing...";
            }
            else if (manager.ObservedTargetId.HasValue)
            {
                var target = GameData.Instance.GetPlayerById(manager.ObservedTargetId.Value);
                buttonText.text = $"Target: {target?.PlayerName ?? "???"}";
            }
            else
            {
                buttonText.text = "Select";
            }
        }

        [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
        [HarmonyPrefix]
        public static bool OnPerformKill()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role?.RoleType != RoleType.Quantum) return true;

            var manager = QuantumManager.Instance;
            if (manager.ObservedTargetId.HasValue && !manager.IsObserved)
            {
                manager.StartCollapse();
            }
            return false; // Prevent normal kill
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        [HarmonyPostfix]
        public static void OnRoundEnd()
        {
            QuantumManager.Instance.Clear();
            if (decoherenceButton != null)
            {
                Object.Destroy(decoherenceButton);
                decoherenceButton = null;
            }
        }
    }
}
