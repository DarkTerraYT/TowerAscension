using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
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
    public class SpikeFactory : AscensionModifier
    {
        public override string TowerId => TowerType.SpikeFactory;

        public override void Apply(int rank, Tower tower, TowerModel defaultTowerModel)
        {
            foreach (var wpn in defaultTowerModel.GetWeapons())
            {
                if (rank > 0)
                {
                    wpn.rate /= MathF.Pow(rank, 1.115f);
                }
                foreach (var proj in wpn.GetDescendants<ProjectileModel>().ToList())
                {
                    if (proj.GetDamageModel() != null && rank > 2)
                    {
                        proj.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.None;
                    }

                    proj.pierce += 5 * rank;
                    proj.GetBehavior<AgeModel>().rounds += Math.Clamp(rank - 1, 0, 10);
                    proj.GetBehavior<AgeModel>().lifespan += Math.Clamp(rank - 1, 0, 10) * 15;
                }
            }

            tower.UpdateRootModel(defaultTowerModel);
        }
    }
}
