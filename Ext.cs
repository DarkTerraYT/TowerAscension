using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppSystem.Dynamic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerAscension
{
    public static class Ext
    {
        public static TowerModel GetDefault(this TowerModel model)
        {
            return Game.instance.model.GetTowerFromId(model.name);
        }

        public static void UpdateTowerModels(this GameModel gameModel, IEnumerable<TowerModel> updatedTowers)
        {
            var list = gameModel.towers.ToList();
            foreach (var tower in updatedTowers)
            {
                int i = list.FindIndex(t =>  t.name == tower.name);

                list.RemoveAt(i);
                list.Insert(i, tower);
            }

            gameModel.towers = list.ToIl2CppReferenceArray();
        }

        public static IEnumerable<Tower> GetTowersOfType(this InGame inGame, string baseId)
        {
            return string.IsNullOrEmpty(baseId) ? inGame.GetTowers() : inGame.GetTowers().Where(t => t.towerModel.baseId == baseId);
        }

        public static TowerModel GetDefaultModel(this Tower tower)
        {
            return tower.towerModel.GetDefault();
        }

        public static void SellTowers(this InGame inGame, string baseId = "", float newWorth = -1)
        {
            foreach(var twr in inGame.GetTowersOfType(baseId))
            {
                if(newWorth >= 0)
                {
                    twr.worth = newWorth;
                }

                twr.SellTower();
            }
        }

        public static bool Is<T>(this Model model, out T t) where T : Model
        {
            if(model.Is<T>())
            {
                t = model.Cast<T>();
                return true;
            }

            t = null;

            return false;
        }
    }
}
