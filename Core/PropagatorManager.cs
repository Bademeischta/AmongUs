using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class PropagatorManager
    {
        private static PropagatorManager _instance;
        public static PropagatorManager Instance => _instance ??= new PropagatorManager();

        private readonly object _stateLock = new object();

        public byte InfectedPlayerId { get; private set; } = byte.MaxValue;
        public float InfectionTime { get; private set; } = -1f;

        private const float InfectionDuration = 20f; // 20 seconds to pass it on
        private const float ProximityRadius = 1.5f; // How close you need to be to pass it

        public void Infect(byte playerId)
        {
            lock (_stateLock)
            {
                if (InfectedPlayerId == byte.MaxValue)
                {
                    InfectedPlayerId = playerId;
                    InfectionTime = Time.time;
                }
            }
        }

        public void Cure()
        {
            lock (_stateLock)
            {
                InfectedPlayerId = byte.MaxValue;
                InfectionTime = -1f;
            }
        }

        public void Update()
        {
            lock (_stateLock)
            {
                if (InfectedPlayerId == byte.MaxValue) return;

                var infectedPlayer = GameData.Instance.GetPlayerById(InfectedPlayerId)?.Object;
                if (infectedPlayer == null || infectedPlayer.Data.IsDead)
                {
                    Cure();
                    return;
                }

                if (Time.time - InfectionTime > InfectionDuration)
                {
                    infectedPlayer.RpcMurderPlayer(infectedPlayer.PlayerId);
                    Cure();
                    return;
                }

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == InfectedPlayerId || player.Data.IsDead) continue;

                    if (Vector2.Distance(infectedPlayer.GetTruePosition(), player.GetTruePosition()) < ProximityRadius)
                    {
                        InfectedPlayerId = player.PlayerId;
                        InfectionTime = Time.time;
                        break;
                    }
                }
            }
        }
    }
}
