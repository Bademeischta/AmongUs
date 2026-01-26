using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class QuantumManager
    {
        private static QuantumManager _instance;
        public static QuantumManager Instance => _instance ??= new QuantumManager();

        public byte? QuantumImpostorId { get; private set; }
        public byte? ObservedTargetId { get; private set; }
        public bool IsObserved { get; private set; }
        public bool IsChanneling { get; private set; }

        private float _channelingTimer;
        private const float ChannelingDuration = 2.0f; // 2 seconds to collapse

        public void SetQuantumImpostor(byte? playerId)
        {
            QuantumImpostorId = playerId;
        }

        public void SetObservedStatus(bool isObserved)
        {
            if (IsObserved != isObserved)
            {
                IsObserved = isObserved;
                if (isObserved && IsChanneling)
                {
                    CancelCollapse();
                }
            }
        }

        public void SelectTarget(byte targetId)
        {
            if (!IsObserved && !IsChanneling)
            {
                ObservedTargetId = targetId;
            }
        }

        public void StartCollapse()
        {
            if (!IsObserved && ObservedTargetId.HasValue)
            {
                IsChanneling = true;
                _channelingTimer = ChannelingDuration;
            }
        }

        public void Update(float deltaTime)
        {
            if (IsChanneling)
            {
                _channelingTimer -= deltaTime;
                if (_channelingTimer <= 0)
                {
                    // Collapse completes
                    // The actual kill logic will be handled via RPC from the patch
                    IsChanneling = false;
                }
            }
        }

        public void CancelCollapse()
        {
            IsChanneling = false;
            ObservedTargetId = null;
            _channelingTimer = 0;
            // Maybe notify the player via a HUD message
        }

        public void Clear()
        {
            QuantumImpostorId = null;
            ObservedTargetId = null;
            IsObserved = false;
            IsChanneling = false;
            _channelingTimer = 0;
        }
    }
}
