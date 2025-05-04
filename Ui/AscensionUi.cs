using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TowerAscension.Ui
{
    [RegisterTypeInIl2Cpp]
    public class AscensionUi : MonoBehaviour
    {
        public const int AscensionPanelHeight = 650;
        public const int AscensionPanelWidth = 450;

        public static AscensionUi instance;

        public ModHelperPanel mainPanel;

        public ModHelperScrollPanel scrollPanel;

        List<string> addedTowers = [];
        public Dictionary<string, ModHelperPanel> PanelByTower = [];

        public static string AscensionTitleText => ModContent.Localize<TowerAscension>(nameof(AscensionTitleText), "Tower Ascension");
        public static string AscendText => ModContent.Localize<TowerAscension>(nameof(AscendText), "Ascend");

        public void UpdateForData(AscensionData data)
        {
            try
            {
                if (addedTowers.Contains(data.TowerId))
                {
                    var panel = PanelByTower[data.TowerId];
                    panel.Background.SetSprite(GetSpriteForRank(data.Rank));

                    var rankText = panel.transform.Find("Rank").GetComponent<ModHelperText>();
                    rankText.Text.text = data.Rank.ToString("#,###");

                    var popsBar = panel.transform.FindChild("PopsBar");
                    var popsFill = popsBar.FindChild("Fill").Cast<RectTransform>();
                    var popsText = popsBar.FindChild("PopsText").GetComponent<ModHelperText>();
                    popsFill.sizeDelta = new(Mathf.Clamp((float)(data.Pops / data.PopsRequired) * 400, 0, 400), 80);

                    popsText.SetText(data.IncreasePopsOnGenerateCash ? $"{data.Pops:#,###}/{data.PopsRequired:#,###} Cash" : $"{data.Pops:#,###}/{data.PopsRequired:#,###} Pops");

                    var ascendBtn = panel.transform.FindChild("Ascend").GetComponent<ModHelperButton>();
                    ascendBtn.Button.interactable = data.CanAscend();
                }
            }
            catch
            {

            }
        }

        [HideFromIl2Cpp]
        public static void Create(InGame inGame)
        {
            if (instance != null)
            {
                instance.mainPanel.Show();
            }
            else
            {
                var mainPanel = inGame.mapRect.gameObject.AddModHelperPanel(new("Ascension", AscensionPanelWidth * 5, AscensionPanelHeight + 225), VanillaSprites.MainBgPanel);
                instance = mainPanel.AddComponent<AscensionUi>();
                instance.mainPanel = mainPanel;

                MelonCoroutines.Start(instance.FinishCreation(Game.instance.model.towers.Where(t => t.IsBaseTower && !t.isSubTower && !TowerAscension.IsBanned(t))));
            }
        }

        [HideFromIl2Cpp]
        IEnumerator FinishCreation(IEnumerable<TowerModel> twrs)
        {
            var title = mainPanel.AddText(new("Title", 0, AscensionPanelHeight / 2 + 37.5f, AscensionPanelWidth * 2.5f, 100), AscensionTitleText);

            title.Text.enableAutoSizing = true;
            title.Text.fontSizeMax = 150;

            scrollPanel = mainPanel.AddScrollPanel(new("ScrollPanel", 0, -82.5f, AscensionPanelWidth * 4.75f, AscensionPanelHeight + 50), RectTransform.Axis.Horizontal, null, 50);
            scrollPanel.RemoveComponent<Mask>();
            scrollPanel.AddComponent<RectMask2D>();
            /*scrollPanel.ScrollContent.RemoveComponent<HorizontalLayoutGroup>();
            yield return null;
            scrollPanel.ScrollContent.AddComponent<GridLayoutGroup>();*/

            mainPanel.AddButton(new("CloseBtn", AscensionPanelWidth * 2.5f, AscensionPanelHeight / 2 + 112.5f, 200), VanillaSprites.CloseBtn, new Action(mainPanel.Hide));

            addedTowers.Clear();

            foreach (var twr in twrs)
            {
                if(scrollPanel == null || mainPanel == null)
                {
                    yield break;
                }

                scrollPanel.AddScrollContent(CreateAscensionDataPanel(twr));
                addedTowers.Add(twr.baseId);
                yield return null;
            }


            yield return null;
        }

        string GetSpriteForRank(int rank)
        {
            switch (rank)
            {
                case 0:
                    return VanillaSprites.BrownInsertPanel;
                case 1:
                    return VanillaSprites.MainBGPanelBronze;
                case 2:
                    return VanillaSprites.MainBGPanelBronze;
                case 3:
                    return VanillaSprites.MainBGPanelSilver;
                case 4:
                    return VanillaSprites.MainBGPanelSilver;
                case 5:
                    return VanillaSprites.MainBGPanelGold;
                case 6:
                    return VanillaSprites.MainBGPanelGold;
                case 7:
                    return VanillaSprites.MainBgPanelHematite;
            }

            return rank < 7 ? VanillaSprites.BrownInsertPanel : VanillaSprites.MainBgPanelHematite;
        }
        ModHelperPanel CreateAscensionDataPanel(TowerModel twr)
        {
            string towerId = twr.name;

            var panel = ModHelperPanel.Create(new("AscensionData_" + twr.name, AscensionPanelWidth, AscensionPanelHeight), GetSpriteForRank(TowerAscension.DataById[towerId].Rank));

            var icon = panel.AddImage(new("Icon", 0, 100, 400), Game.instance.model.GetTowerFromId(towerId).icon.GetGUID());
            var rankText = panel.AddText(new("Rank", 0, -150, 400, 100), TowerAscension.DataById[towerId].Rank.ToString("#,###"));
            rankText.Text.fontSizeMax = 60;
            rankText.Text.enableAutoSizing = true;
            rankText.Text.color = new Color32(192, 143, 18, 255);
            var nameText = panel.AddText(new("Name", 0, AscensionPanelHeight / 2 - 50, AscensionPanelWidth, 100), towerId.Localize());
            nameText.Text.enableAutoSizing = true;

            var popsProgressBar = panel.AddImage(new("PopsBar", 0, -150, 400, 80), VanillaSprites.BrownInsertPanelDark);
            var popsProgressBarFill = popsProgressBar.AddImage(new("Fill", -200, 0, 400 * Mathf.Clamp((float)(TowerAscension.DataById[towerId].Pops / TowerAscension.DataById[towerId].PopsRequired) * 400, 0, 400), 80) { Pivot = new(0, 0.5f) }, VanillaSprites.GreenFillSmall);
            var popsProgressText = popsProgressBar.AddText(new("PopsText", 400, 80), TowerAscension.DataById[towerId].IncreasePopsOnGenerateCash ? $"{TowerAscension.DataById[towerId].Pops:#,###}/{TowerAscension.DataById[towerId].PopsRequired:#,###} Cash" : $"{TowerAscension.DataById[towerId].Pops:#,###}/{TowerAscension.DataById[towerId].PopsRequired:#,###} Pops");
            popsProgressText.Text.enableAutoSizing = true;

            var modifier = AscensionModifier.GetAscensionModifier(towerId);

            var ascendBtn = panel.AddButton(new("Ascend", 0, -256, 100 * ModHelperButton.LongBtnRatio, 100), VanillaSprites.GreenBtnLong, new Action(() => TowerAscension.DataById[towerId].TryAscend()));
            ascendBtn.Button.interactable = TowerAscension.DataById[towerId].CanAscend();

            var ascendText = ascendBtn.AddText(new("Text", 65 * ModHelperButton.LongBtnRatio, 65), AscendText);
            ascendText.Text.enableAutoSizing = true;

            PanelByTower.TryAdd(towerId, panel);
            return panel;
        }
    }
}
