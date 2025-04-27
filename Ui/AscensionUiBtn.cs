using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowerAscension.Ui
{
    [RegisterTypeInIl2Cpp]
    public class AscensionUiBtn : MonoBehaviour
    {
        public static AscensionUiBtn instance;

        public ModHelperButton btn;

        static RectTransform UiRect;

        [HideFromIl2Cpp]
        public static void Create(InGame inGame)
        {
            if (instance != null) {

                instance.btn.Show();

                return;
            }

            UiRect = inGame.mapRect;

            var btn = inGame.mapRect.gameObject.AddModHelperComponent(ModHelperButton.Create(new("AscensionUiBtn", UiRect.rect.right - 135, UiRect.rect.bottom - 550, 200), ModContent.GetTextureGUID<TowerAscension>("Icon"), new Action(() => { AscensionUi.Create(inGame); })));
            instance = btn.AddComponent<AscensionUiBtn>();
            instance.btn = btn;
        }
    }
}
