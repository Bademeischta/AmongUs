using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class ScribeManager
    {
        private static ScribeManager _instance;
        public static ScribeManager Instance => _instance ??= new ScribeManager();

        private readonly object _stateLock = new object();
        private string _hiddenTruth;
        private bool _truthRevealed;

        private ScribeManager() { }

        public void SetHiddenTruth(string truth)
        {
            lock (_stateLock)
            {
                _hiddenTruth = truth;
                _truthRevealed = false;
            }
        }

        public string GetHiddenTruth()
        {
            lock (_stateLock)
            {
                return _hiddenTruth;
            }
        }

        public void RevealTruth()
        {
            lock (_stateLock)
            {
                _truthRevealed = true;
            }
        }

        public bool IsTruthRevealed(out string truth)
        {
            lock (_stateLock)
            {
                truth = _hiddenTruth;
                return _truthRevealed;
            }
        }

        public void Clear()
        {
            lock (_stateLock)
            {
                _hiddenTruth = null;
                _truthRevealed = false;
            }
        }
    }
}
