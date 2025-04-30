using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerAscension.Modifier;

namespace TowerAscension
{
    public abstract class AscensionModifier : ModContent
    {
        protected string RealTower = "";

        public static Dictionary<string, AscensionModifier> ModifierByTowerId { get => m_ModifierByTowerId; }
        static Dictionary<string, AscensionModifier> m_ModifierByTowerId = [];

        public static AscensionModifier GetAscensionModifier(string tower)
        {
            if (!ModifierByTowerId.TryGetValue(tower, out var modifier))
            {
                var newModifier = new DefaultAscensionModifier(tower);
                newModifier.RealTower = tower;
                return newModifier;
            }

            return modifier;
        }

        public virtual AscensionData GetAscensionData()
        {
            AscensionData data = new()
            {
                TowerId = TowerId,
                Rank = 0,
                Pops = 0,
                PopsRequired = 50000 * PopsReqMultiplier,
                IncreasePopsOnGenerateCash = IncreasePopsOnGenerateCash
            };

            return data;
        }

        public override void Register()
        {
            if(m_ModifierByTowerId.ContainsKey(TowerId) && !Replace)
            {
                ModHelper.Warning<TowerAscension>($"Ascenion Modifier for tower {TowerId} already exists. If you're trying to replace, override \"AscensionModifier.Replace\"");
                return;
            }

            m_ModifierByTowerId[TowerId] = this;
        }

        public virtual bool Replace => false;

        public abstract string TowerId { get; }

        public virtual float PopsReqMultiplier => 1;

        public virtual bool IncreasePopsOnGenerateCash => false;

        public virtual bool BaseTower => true;

        public abstract void Apply(int rank, Tower tower, TowerModel defaultTowerModel);

        public virtual void Apply(int rank, InGame inGame)
        {
            foreach(var twr in inGame.GetTowersOfType(TowerId))
            {
                Apply(rank, twr, twr.GetDefaultModel());
            }
        }

        protected TowerModel GetDefaultTowerModel(string id)
        {
            return Game.instance.model.GetTowerFromId(id);
        }

        public virtual void OnEnterMatch(InGame inGame)
        {

        }
    }


    public delegate void OnAscend_Delagate(int rank, InGame inGame);
}
