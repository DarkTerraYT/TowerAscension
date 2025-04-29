using MelonLoader;
using BTD_Mod_Helper;
using TowerAscension;
using System.Collections.Generic;
using TowerAscension.Ui;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.NKMulti;
using Il2Cpp;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Simulation.Track;
using Newtonsoft.Json;
using System.Collections;
using Il2CppAssets.Scripts.Unity;
using System.Linq;
using UnityEngine;
using System;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.Towers.Behaviors.Abilities.Behaviors;
using static Il2CppSystem.Array;
using BTD_Mod_Helper.Api.Hooks.BloonHooks;
using BTD_Mod_Helper.Api.Hooks;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;

[assembly: MelonInfo(typeof(TowerAscension.TowerAscension), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace TowerAscension;

public class TowerAscension : BloonsTD6Mod
{
    public static Dictionary<string, AscensionData> DataById = [];
    const string AscensionSaveId = "TowerAscensions";
    [HarmonyPatch(typeof(Map), nameof(Map.GetSaveData))]
    static class Map_GetSaveData
    {
        [HarmonyPostfix]
        public static void Postfix(MapSaveDataModel mapData)
        {
            var json = JsonConvert.SerializeObject(DataById);
            if (!mapData.metaData.TryAdd(AscensionSaveId, json))
            {
                mapData.metaData[AscensionSaveId] = json;
            }
        }
    }

    public static string[] BannedTowers => [TowerType.MonkeyVillage, "TimeMaster-TimeMaster_Tower"];

    public static bool IsBanned(TowerModel tm) => BannedTowers.Contains(tm.baseId) || tm.IsHero();

    static IEnumerable<AscensionData> CreateData()
    {
        foreach(var twr in Game.instance.model.towers.Where(t => t.name == t.baseId && !IsBanned(t)))
        {
            var data = AscensionModifier.GetAscensionModifier(twr.baseId).GetAscensionData();
            data.TowerId = twr.baseId;

            yield return data;
        }
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    static class InGame_StartMatch
    {
        [HarmonyPostfix]
        public static void Postfix(InGame __instance, MapSaveDataModel mapSaveData)
        {
            if (mapSaveData != null && mapSaveData.metaData.TryGetValue(AscensionSaveId, out var json))
            {
                DataById = JsonConvert.DeserializeObject<Dictionary<string, AscensionData>>(json)!;
            }
            else
            {
                DataById.Clear();

                foreach (var data in CreateData())
                {
                    DataById[data.TowerId] = data;
                }
            }
            foreach (var data in DataById.Values)
            {
                AscensionModifier.GetAscensionModifier(data.TowerId).DoAscend(data.Rank, __instance);
            }
            AscensionUiBtn.Create(InGame.instance);
        }
    }

    public override void OnAbilityCast(Ability ability)
    {
        LoggerInstance.Msg(ability.abilityModel.cooldown);
    }

    [HookTarget(typeof(BloonDamageHook), HookTargetAttribute.EHookType.Postfix)]
    [HookPriority(HookPriorityAttribute.Higher)]
    public static bool BloonDamagePostfix(Bloon @this, ref float totalAmount, Projectile projectile, ref bool distributeToChildren,
ref bool overrideDistributeBlocker, ref bool createEffect, Tower tower, BloonProperties immuneBloonProperties,
BloonProperties originalImmuneBloonProperties, ref bool canDestroyProjectile, ref bool ignoreNonTargetable, ref bool blockSpawnChildren, HookNullable<int> powerActivatedByPlayerId)
    {
        if (tower != null && !IsBanned(tower.towerModel) && !DataById[tower.towerModel.baseId].IncreasePopsOnGenerateCash)
        {
            DataById[tower.towerModel.baseId].Pops += totalAmount;
            AscensionUi.instance?.UpdateForData(DataById[tower.towerModel.baseId]);
        }

        return true;
    }

    [HarmonyPatch(typeof(Cash), nameof(Cash.Pickup))]
    public static class CashModel_Collect
    {
        public static void Postfix(Cash __instance, float __result)
        {
            Tower twr = __instance.projectile.Weapon.attack.tower;
            if (twr != null)
            {
                if (DataById[twr.towerModel.baseId].IncreasePopsOnGenerateCash) 
                {
                    DataById[twr.towerModel.baseId].Pops += __result;
                }
            }
        }
    }
}