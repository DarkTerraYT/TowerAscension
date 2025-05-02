using BTD_Mod_Helper;
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
            return Game.instance.model.GetTowerFromId(model.name).Duplicate();
        }

        public static void UpdateTowerModels(this InGame inGame, IEnumerable<TowerModel> updatedTowers)
        {
            var gameModel = inGame.GetGameModel();

            var list = gameModel.towers.ToList();
            foreach (var tower in updatedTowers)
            {
                int i = list.FindIndex(t => t.name == tower.name);

                list.RemoveAt(i);
                list.Insert(i, tower);

                foreach(var t in inGame.GetTowers(tower.name))
                {
                    t.UpdateRootModel(tower);
                    ModHelper.Log<TowerAscension>(t.towerModel.name);
                }
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
