using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerAscension.Ui;
using UnityEngine.InputSystem;

namespace TowerAscension
{
    public class AscensionData
    {
        public string TowerId;

        public bool IncreasePopsOnGenerateCash;

        public float Pops = 0;
        public float PopsRequired;

        public int Rank;
        public bool CanAscend()
        {
            return Pops >= PopsRequired || InGame.instance.bridge.IsSandboxMode();
        }

        public static string AscensionConfirmPopupBody => ModContent.Localize<TowerAscension>(nameof(AscensionConfirmPopupBody), "Ascending {Tower} will grant this tower with great buffs, however, all {Tower}s you have placed down will be sold.");
        public static string ConfirmText => ModContent.Localize<TowerAscension>(nameof(ConfirmText), "Ascend.");

        public bool TryAscend()
        {
            if (CanAscend())
            {
                PopupScreen.instance.SafelyQueue(popup => popup.ShowPopup(PopupScreen.Placement.inGameCenter, "Ascend Tower", $"Ascending {TowerId.GetBtd6Localization()} will grant this tower with great buffs.", new Action(() => { Ascend(); }), "Ascend", null, "Cancel", Popup.TransitionAnim.Scale));
                return true;
            }
            return false;
        }

        public void Ascend() 
        { 
            Rank++;
            Pops = 0;
            PopsRequired *= 1.25f;

            AscensionModifier.GetAscensionModifier(TowerId).DoAscend(Rank, InGame.instance);

            AscensionUi.instance?.UpdateForData(this);
        }
    }
}
