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

        // Auditor
        CmdAuditPlayer = 145,
        RpcAuditPlayer = 146,

        // Phantom
        CmdSetImprint = 147,
        RpcSetImprint = 148,
        CmdPhantomKill = 149,
        RpcPhantomKill = 150,
    }
}
