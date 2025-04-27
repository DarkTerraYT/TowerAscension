using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TowerAscension.Modifier
{
    public class DefaultAscensionModifier : AscensionModifier
    {
        // Ensure always loads first
        protected override int Order => -999;
        public override string TowerId => "Default";

        public DefaultAscensionModifier(string tower)
        {
            RealTower = tower;
            Ascend += OnAscend;
        }

        public override void OnAscend(int rank, InGame inGame)
        {
            List<TowerModel> newTowers = [];

            ModHelper.Log<TowerAscension>(RealTower);


            foreach (TowerModel tm in inGame.GetGameModel().towers.Duplicate().Where(t => t.baseId == RealTower).Select(t => t.GetDefault()))
            {
                ModHelper.Log<TowerAscension>(tm.name);

                tm.range += rank * 5;

                foreach (var atk in tm.GetAttackModels())
                {
                    atk.range += rank * 5;

                    if (rank > 5)
                    {
                        atk.attackThroughWalls = true;
                    }

                    foreach (var wpn in atk.weapons)
                    {
                        wpn.rate *= rank != 0 ? Mathf.Pow(6 * rank / (5 * rank), 8 * rank / 9) : 1;
                        foreach (var dmgModel in wpn.projectile.GetDescendants<DamageModel>().ToList())
                        {
                            dmgModel.damage *= rank != 0 ? Mathf.Pow(6 * rank / (5 * rank), 8 * rank / 9) : 1;
                        }

                        float lifespanMultiplier = atk.range / atk.range - (rank * 5);
                        ModHelper.Log<TowerAscension>(lifespanMultiplier);

                        if (wpn.projectile.HasBehavior<TravelStraitModel>())
                        {
                            wpn.projectile.GetBehavior<TravelStraitModel>().lifespan *= lifespanMultiplier;
                        }
                        else if (wpn.projectile.HasBehavior<TravelAlongPathModel>())
                        {
                            wpn.projectile.GetBehavior<TravelAlongPathModel>().lifespan *= lifespanMultiplier;
                        }
                        else if (wpn.projectile.HasBehavior<TravelCurvyModel>())
                        {
                            wpn.projectile.GetBehavior<TravelCurvyModel>().lifespan *= lifespanMultiplier;
                        }
                        else if (wpn.projectile.HasBehavior<TravelStraitSlowdownModel>())
                        {
                            wpn.projectile.GetBehavior<TravelStraitSlowdownModel>().lifespan *= lifespanMultiplier;
                        }
                        else if (wpn.projectile.HasBehavior<TravelTowardsEmitTowerModel>())
                        {
                            wpn.projectile.GetBehavior<TravelTowardsEmitTowerModel>().lifespan *= lifespanMultiplier;
                        }
                        else if (wpn.projectile.HasBehavior<AgeModel>())
                        {
                            wpn.projectile.GetBehavior<AgeModel>().lifespan *= lifespanMultiplier;
                            wpn.projectile.GetBehavior<AgeModel>().rounds = (int)(wpn.projectile.GetBehavior<AgeModel>().rounds + lifespanMultiplier);
                        }
                    }
                }

                newTowers.Add(tm);
            }

            inGame.GetGameModel().UpdateTowerModels(newTowers);
        }
    }
}
