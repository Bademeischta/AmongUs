using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class ResidueChecker : MonoBehaviour
    {
        void FixedUpdate()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                ResidueManager.Instance.CheckResidues();
            }
        }
    }
}
