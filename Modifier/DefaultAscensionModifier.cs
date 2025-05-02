using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.DateTimeParse;

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
        }

        public override void OnAscend(int rank, InGame inGame)
        {
            List<TowerModel> newTowers = [];

            foreach (var defaultTowerModel in inGame.GetGameModel().towers.Where(tm => tm.baseId == RealTower).Select(tm => tm.GetDefault()))
            {
                defaultTowerModel.range += rank * 5;

                foreach (var atk in defaultTowerModel.GetAttackModels())
                {
                    atk.range += rank * 5;

                    if (rank > 5)
                    {
                        atk.attackThroughWalls = true;
                    }

                    foreach (var wpn in atk.weapons)
                    {
                        if (rank > 0)
                        {
                            wpn.rate *= rank != 0 ? Mathf.Pow(6 * rank / (5 * rank), 8 * rank / 9) : 1;
                            foreach (var dmgModel in wpn.projectile.GetDescendants<DamageModel>().ToList())
                            {
                                dmgModel.damage *= rank != 0 ? Mathf.Pow(6 * rank / (5 * rank), 8 * rank / 9) : 1;
                            }
                        }

                        float lifespanMultiplier = 1 + (rank * 0.15f);

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
                newTowers.Add(defaultTowerModel);
            }
            inGame.UpdateTowerModels(newTowers);
        }
    }
}
