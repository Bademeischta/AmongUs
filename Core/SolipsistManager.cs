using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class SolipsistManager
    {
        private static SolipsistManager _instance;
        public static SolipsistManager Instance => _instance ??= new SolipsistManager();

        // Dictionary mapping PlayerId -> HashSet of Censored Object NetIds
        private readonly Dictionary<byte, HashSet<uint>> _censoredObjects = new Dictionary<byte, HashSet<uint>>();
        private readonly object _lock = new object();

        private SolipsistManager() { }

        public void SetCensored(byte playerId, uint netId)
        {
            lock (_lock)
            {
                if (!_censoredObjects.ContainsKey(playerId))
                {
                    _censoredObjects[playerId] = new HashSet<uint>();
                }
                _censoredObjects[playerId].Add(netId);
            }
        }

        public bool IsCensored(byte playerId, uint netId)
        {
            lock (_lock)
            {
                if (_censoredObjects.TryGetValue(playerId, out var objects))
                {
                    return objects.Contains(netId);
                }
                return false;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _censoredObjects.Clear();
            }
        }
    }
}
