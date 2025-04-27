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
namespace TowerAscension.Modifier
{
    public class TackShooter : AscensionModifier
    {
        public override string TowerId => TowerType.TackShooter;

        public override void OnAscend(int rank, InGame inGame)
        {
            List<TowerModel> newTowers = [];

            foreach (TowerModel tm in inGame.GetGameModel().towers.Duplicate().Where(t => t.baseId == TowerId).Select(t => t.GetDefault()))
            {
                var wpn = tm.GetWeapon();
                if (wpn.emission.Is<ArcEmissionModel>(out var emission))
                {
                    emission.count += rank * 2;
                    wpn.projectile.GetDamageModel().damage *= 1 + MathF.Log2(rank);
                }

                if(rank >= 3 && tm.tiers[0] == 5)
                {
                    wpn = tm.GetWeapon(1).Duplicate();

                    wpn.rate *= 2;
                    wpn.rate /= rank - 2;

                    wpn.emission = new ArcEmissionModel("ArcEmissionModel", 8, 0, 360, null, false, false);

                    wpn.name = "FireballsAscended";

                    tm.GetAttackModel(1).AddWeapon(wpn);
                }

                tm.IncreaseRange(5 * (1 + MathF.Log2(rank + 1)));

                newTowers.Add(tm);
            }

            inGame.GetGameModel().UpdateTowerModels(newTowers);
        }
    }
}
