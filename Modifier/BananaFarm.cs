using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.DateTimeParse;

namespace TowerAscension.Modifier
{
    public class BananaFarm : AscensionModifier
    {
        public override string TowerId => TowerType.BananaFarm;

        public override bool IncreasePopsOnGenerateCash => true;

        public override float PopsReqMultiplier => .4f;

        public override void Apply(int rank, Tower tower, TowerModel defaultTowerModel)
        {
            foreach (var cashModel in defaultTowerModel.GetDescendants<CashModel>().ToList())
            {
                cashModel.bonusMultiplier += 1 * rank;
            }

        }
    }
}
