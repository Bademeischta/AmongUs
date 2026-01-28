namespace MyCustomRolesMod.Networking.Packets
{
    public enum RpcType : byte
    {
        Acknowledge = 128,
        VersionCheck = 129,
        VersionResponse = 130,
        SetRole = 131,
        SyncAllRoles = 132,
        SyncOptions = 133,
        SetInfectedWord = 134,
        SyncInfectedWord = 135,
        CmdPlayerUsedInfectedWord = 136,
        RpcPlayerUsedInfectedWord = 137,
        MarkPlayer = 138,
        SyncMarkedPlayer = 139,
        SetFakeTimeOfDeath = 140,
        Disconnect = 141,

        SetWitnessTestimony = 142,
        SetPuppeteerForcedMessage = 143,
        SetGlitchCorruptedSystem = 144,

        SetAnchorLinkedPlayers = 145,
        RpcRevealAnchorLink = 146,
        CmdResidueKill = 147,
        RpcSpawnResidueBody = 148,
        RpcSetResidueState = 149,
    }
}
