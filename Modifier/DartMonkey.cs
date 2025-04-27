using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerAscension.Modifier
{
    public class DartMonkey : AscensionModifier
    {
        public override string TowerId => TowerType.DartMonkey;

        public override void OnAscend(int rank, InGame inGame)
        {
            List<TowerModel> newTowers = [];

            foreach (TowerModel tm in inGame.GetGameModel().towers.Duplicate().Where(t => t.baseId == TowerId).Select(t => t.GetDefault()))
            {

                tm.IncreaseRange(6 * (1 + MathF.Log2(rank + 1)));


                foreach (var wpn in tm.GetWeapons())
                {
                    if(wpn.emission.Is<ArcEmissionModel>(out var emission))
                    {
                        emission.count += rank;
                    }
                    else
                    {
                        emission = new(wpn.emission.name, 1 + rank, 0, rank == 1 ? 5 : 30, null, false, false);
                    }

                    wpn.rate /= MathF.Pow(rank, 1.115f);

                    foreach(var proj in wpn.GetDescendants<ProjectileModel>().ToList())
                    {
                        if (proj.GetDamageModel() != null)
                        {
                            proj.GetDamageModel().damage *= 1 + (MathF.Pow(rank, 1.2f) - (rank / 1.15f));

                            if(rank > 2)
                            {
                                proj.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.None;
                            }
                        }

                        var travelModel = proj.GetBehavior<TravelStraitModel>();

                        if (travelModel != null)
                        {
                            travelModel.lifespan *= tm.range / tm.GetDefault().range;
                        }
                    }
                }

                if (rank >= 1)
                {
                    tm.GetDescendants<FilterInvisibleModel>().ForEach(mod => mod.isActive = false);
                }

                newTowers.Add(tm);
            }

            inGame.GetGameModel().UpdateTowerModels(newTowers);
        }
    }
}
