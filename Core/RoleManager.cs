using System.Collections.Generic;
using System.Linq;

namespace MyCustomRolesMod.Core
{
    public class RoleManager
    {
        private static RoleManager _instance;
        public static RoleManager Instance => _instance ??= new RoleManager();

        private readonly Dictionary<byte, BaseRole> _playerRoles = new Dictionary<byte, BaseRole>();
        private byte _jesterWinnerId = byte.MaxValue;

        // A single lock object to ensure atomicity across all state modifications.
        private readonly object _stateLock = new object();

        private RoleManager() { }

        public void SetRole(PlayerControl player, RoleType roleType)
        {
            if (player == null) return;

            lock (_stateLock)
            {
                _playerRoles.Remove(player.PlayerId);

                BaseRole newRole = roleType switch
                {
                    RoleType.Jester => new JesterRole(player),
                    RoleType.Echo => new EchoRole(player),
                    RoleType.Geist => new GeistRole(player),
                    RoleType.Witness => new WitnessRole(player),
                    RoleType.Puppeteer => new PuppeteerRole(player),
                    RoleType.Glitch => new GlitchRole(player),
                    RoleType.Auditor => new AuditorRole(player),
                    RoleType.Phantom => new PhantomRole(player),
                    _ => null
                };

                if (newRole != null)
                {
                    _playerRoles[player.PlayerId] = newRole;
                }
            }
        }

        public BaseRole GetRole(byte playerId)
        {
            lock (_stateLock)
            {
                _playerRoles.TryGetValue(playerId, out var role);
                return role;
            }
        }

        public Dictionary<byte, RoleType> GetAllRoles()
        {
            lock (_stateLock)
            {
                // The dictionary is copied here, ensuring the returned object is safe
                // from modifications to the original collection.
                return _playerRoles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.RoleType);
            }
        }

        public void ClearAllRoles()
        {
            lock (_stateLock)
            {
                _playerRoles.Clear();
                _jesterWinnerId = byte.MaxValue;
            }
        }

        public void SetJesterWinner(byte playerId)
        {
            lock (_stateLock)
            {
                if (_jesterWinnerId != byte.MaxValue) return; // Prevent winner from being changed.
                _jesterWinnerId = playerId;
            }
        }

        public bool HasJesterWinner(out byte winnerId)
        {
            lock (_stateLock)
            {
                winnerId = _jesterWinnerId;
                return _jesterWinnerId != byte.MaxValue;
            }
        }
    }
}
