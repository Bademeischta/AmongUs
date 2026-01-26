using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class ChroniclerManager
    {
        private static ChroniclerManager _instance;
        public static ChroniclerManager Instance => _instance ??= new ChroniclerManager();

        private readonly List<string> _facts = new List<string>();
        private readonly object _lock = new object();

        public void AddFact(string fact)
        {
            lock (_lock)
            {
                _facts.Add(fact.ToLower());
            }
        }

        public bool VerifyFact(string statement)
        {
            lock (_lock)
            {
                return _facts.Contains(statement.ToLower());
            }
        }

        public void ClearFacts()
        {
            lock (_lock)
            {
                _facts.Clear();
            }
        }
    }
}