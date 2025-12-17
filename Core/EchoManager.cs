namespace MyCustomRolesMod.Core
{
    public class EchoManager
    {
        private static EchoManager _instance;
        public static EchoManager Instance => _instance ??= new EchoManager();

        public string InfectedWord { get; private set; }

        private EchoManager() { }

        // This method is now only called on the host by the RpcManager
        // or on the client by the SyncInfectedWord RPC.
        public void SetInfectedWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                InfectedWord = null;
                return;
            }

            InfectedWord = word.Trim().ToLower();
        }

        public void Clear()
        {
            InfectedWord = null;
        }
    }
}
