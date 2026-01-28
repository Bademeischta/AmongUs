using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class AnchorPatches
    {
        private static LinkButton _linkButton;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void HudManager_Update_Postfix(HudManager __instance)
        {
            var role = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId);
            if (role?.RoleType != RoleType.Anchor)
            {
                if (_linkButton != null) _linkButton.Destroy();
                return;
            }

            if (_linkButton == null)
            {
                _linkButton = new LinkButton();
            }
            _linkButton.Update();
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void MeetingHud_Update_Postfix(MeetingHud __instance)
        {
            var role = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId);
            if (role?.RoleType != RoleType.Anchor || AnchorManager.Instance.LinkRevealed)
            {
                if (_revealLinkButton != null) _revealLinkButton.Destroy();
                return;
            }

            if (_revealLinkButton == null)
            {
                _revealLinkButton = new RevealLinkButton();
            }
            _revealLinkButton.Update(__instance);
        }

        private static RevealLinkButton _revealLinkButton;

        private class RevealLinkButton
        {
            private readonly GameObject _gameObject;
            private readonly SpriteRenderer _renderer;
            private readonly Collider2D _collider;

            public RevealLinkButton()
            {
                var originalButton = MeetingHud.Instance.SkipVoteButton.gameObject;
                _gameObject = Object.Instantiate(originalButton, originalButton.transform.parent);
                _gameObject.transform.localPosition = new Vector3(0, 2.5f, 0);
                _renderer = _gameObject.GetComponent<SpriteRenderer>();
                _collider = _gameObject.GetComponent<Collider2D>();
                _renderer.color = new Color(0.1f, 0.1f, 0.4f, 0.5f);
            }

            public void Update(MeetingHud meetingHud)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (_collider.OverlapPoint(mousePosition))
                    {
                        HandleClick(meetingHud);
                    }
                }
            }

            private void HandleClick(MeetingHud meetingHud)
            {
                Networking.RpcManager.Instance.SendRpcRevealAnchorLink();
                ForceVote(meetingHud);
                Destroy();
            }

            private void ForceVote(MeetingHud meetingHud)
            {
                foreach (var player in meetingHud.playerStates)
                {
                    if (player.TargetPlayerId != AnchorManager.Instance.LinkedPlayer1 && player.TargetPlayerId != AnchorManager.Instance.LinkedPlayer2)
                    {
                        player.voteButton.enabled = false;
                    }
                }
            }

            public void Destroy()
            {
                if (_gameObject != null) Object.Destroy(_gameObject);
            }
        }

        private class LinkButton
        {
            private readonly GameObject _gameObject;
            private readonly SpriteRenderer _renderer;
            private readonly Collider2D _collider;
            private PlayerControl _selectedPlayer1;
            private PlayerControl _selectedPlayer2;

            public LinkButton()
            {
                var originalButton = HudManager.Instance.ReportButton.gameObject;
                _gameObject = Object.Instantiate(originalButton, originalButton.transform.parent);
                _gameObject.transform.localPosition = new Vector3(2.5f, 1.5f, 0);
                _renderer = _gameObject.GetComponent<SpriteRenderer>();
                _collider = _gameObject.GetComponent<Collider2D>();
                _renderer.color = new Color(0.1f, 0.1f, 0.4f, 0.5f);
            }

            public void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (_collider.OverlapPoint(mousePosition))
                    {
                        HandleClick();
                    }
                }
            }

            private void HandleClick()
            {
                if (_selectedPlayer1 == null)
                {
                    _selectedPlayer1 = FindClosestPlayer();
                    if (_selectedPlayer1 != null) _renderer.color = new Color(0.1f, 0.1f, 0.4f, 1f);
                }
                else
                {
                    _selectedPlayer2 = FindClosestPlayer();
                    if (_selectedPlayer2 != null && _selectedPlayer1.PlayerId != _selectedPlayer2.PlayerId)
                    {
                        Networking.RpcManager.Instance.SendSetAnchorLinkedPlayers(_selectedPlayer1.PlayerId, _selectedPlayer2.PlayerId);
                        Destroy();
                    }
                }
            }

            private PlayerControl FindClosestPlayer()
            {
                PlayerControl closestPlayer = null;
                float minDistance = float.MaxValue;

                foreach (var player in PlayerControl.AllPlayers)
                {
                    if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId || player.Data.IsDead) continue;

                    float distance = Vector2.Distance(PlayerControl.LocalPlayer.transform.position, player.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPlayer = player;
                    }
                }
                return closestPlayer;
            }

            public void Destroy()
            {
                if (_gameObject != null) Object.Destroy(_gameObject);
            }
        }
    }
}
