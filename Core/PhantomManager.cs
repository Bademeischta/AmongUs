using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class PhantomManager
    {
        private static PhantomManager _instance;
        public static PhantomManager Instance => _instance ??= new PhantomManager();

        private readonly object _stateLock = new object();
        private readonly HashSet<byte> _phantoms = new HashSet<byte>();

        private PhantomManager() { }

        public void SetPhantom(byte playerId, bool isPhantom)
        {
            lock (_stateLock)
            {
                if (isPhantom)
                {
                    _phantoms.Add(playerId);
                }
                else
                {
                    _phantoms.Remove(playerId);
                }
            }
        }

        public bool IsPhantom(byte playerId)
        {
            lock (_stateLock)
            {
                return _phantoms.Contains(playerId);
            }
        }

        public void Clear()
        {
            lock (_stateLock)
            {
                _phantoms.Clear();
            }
        }
    }
}
