namespace MyCustomRolesMod.Core
{
    public class AnchorManager
    {
        private static AnchorManager _instance;
        public static AnchorManager Instance => _instance ??= new AnchorManager();

        public byte LinkedPlayer1 { get; private set; }
        public byte LinkedPlayer2 { get; private set; }
        public bool Link Revealed { get; private set; }

        private AnchorManager() { }

        public void SetLinkedPlayers(byte player1, byte player2)
        {
            LinkedPlayer1 = player1;
            LinkedPlayer2 = player2;
            LinkRevealed = false;
        }

        public void RevealLink()
        {
            LinkRevealed = true;
        }

        public void Clear()
        {
            LinkedPlayer1 = 0;
            LinkedPlayer2 = 0;
            LinkRevealed = false;
        }
    }
}
