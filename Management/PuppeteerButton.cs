using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using UnityEngine;

namespace MyCustomRolesMod.Management
{
    public class PuppeteerButton : MonoBehaviour
    {
        public void OnClick()
        {
            var puppeteer = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId) as PuppeteerRole;
            if (puppeteer == null) return;

            if (puppeteer.IsControlling)
            {
                puppeteer.StopControl();
            }
            else if (puppeteer.IsSelectingTarget)
            {
                puppeteer.StopSelection();
            }
            else
            {
                puppeteer.StartSelection();
            }
        }
    }
}
