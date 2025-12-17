using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class GlitchManager
    {
        private static GlitchManager _instance;
        public static GlitchManager Instance => _instance ??= new GlitchManager();

        private readonly HashSet<int> _corruptedSystems = new HashSet<int>();
        private readonly object _stateLock = new object();

        private GlitchManager() { }

        public void CorruptSystem(int systemId)
        {
            lock (_stateLock)
            {
                _corruptedSystems.Add(systemId);
            }
        }

        public bool IsSystemCorrupted(int systemId)
        {
            lock (_stateLock)
            {
                return _corruptedSystems.Contains(systemId);
            }
        }

        public void ClearAllCorruptedSystems()
        {
            lock (_stateLock)
            {
                _corruptedSystems.Clear();
            }
        }
    }
}
