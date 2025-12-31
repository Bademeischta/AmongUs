using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class PhantomManager
    {
        private static PhantomManager _instance;
        public static PhantomManager Instance => _instance ??= new PhantomManager();

        private Vector2? _imprintedLocation;

        private readonly object _stateLock = new object();

        private PhantomManager() { }

        public void SetImprintLocation(Vector2 location)
        {
            lock (_stateLock)
            {
                _imprintedLocation = location;
            }
        }

        public Vector2? GetImprintLocation()
        {
            lock (_stateLock)
            {
                return _imprintedLocation;
            }
        }

        public void Clear()
        {
            lock (_stateLock)
            {
                _imprintedLocation = null;
            }
        }
    }
}
