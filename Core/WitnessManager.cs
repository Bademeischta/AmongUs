using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class WitnessManager
    {
        private static WitnessManager _instance;
        public static WitnessManager Instance => _instance ??= new WitnessManager();

        private readonly Dictionary<byte, string> _testimonies = new Dictionary<byte, string>();
        private readonly object _stateLock = new object();

        private WitnessManager() { }

        public void SetTestimony(byte playerId, string testimony)
        {
            lock (_stateLock)
            {
                _testimonies[playerId] = testimony;
            }
        }

        public string GetTestimony(byte playerId)
        {
            lock (_stateLock)
            {
                _testimonies.TryGetValue(playerId, out var testimony);
                return testimony;
            }
        }

        public void ClearAllTestimonies()
        {
            lock (_stateLock)
            {
                _testimonies.Clear();
            }
        }
    }
}
