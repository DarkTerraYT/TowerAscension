using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TowerAscension;
using static Il2CppSystem.DateTimeParse;
namespace TowerAscension.Modifier
{
    public class TackShooter : AscensionModifier
    {
        public override string TowerId => TowerType.TackShooter;

        public override void Apply(int rank, Tower tower, TowerModel defaultTowerModel)
        {
            var wpn = defaultTowerModel.GetWeapon();
            if (wpn.emission.Is<ArcEmissionModel>(out var emission))
            {
                emission.count += rank * 2;
                wpn.emission = emission;
            }

            if (rank > 0)
            {
                wpn.projectile.GetDamageModel().damage *= 1 + MathF.Log2(rank);
            }

            if (rank >= 3 && defaultTowerModel.tiers[0] == 5)
            {
                wpn = defaultTowerModel.GetWeapon(1).Duplicate();
                wpn.rate /= rank - 2;

                wpn.emission = new ArcEmissionModel("ArcEmissionModel", 8, 0, 360, null, false, false);
                wpn.name = "FireballsAscended";

                defaultTowerModel.GetAttackModel(1).AddWeapon(wpn);
            }

            defaultTowerModel.IncreaseRange(5 * (1 + MathF.Log2(rank + 1)));

            tower.UpdateRootModel(defaultTowerModel);
        }
    }
}
