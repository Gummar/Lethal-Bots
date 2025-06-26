using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using UnityEngine;

namespace LethalBots.Patches.MapHazardsPatches
{
    /// <summary>
    /// Patch for <c>SpikeRoofTrap</c>
    /// </summary>
    [HarmonyPatch(typeof(SpikeRoofTrap))]
    public class SpikeRoofTrapPatch
    {
        // TODO: This needs to be changed to use a transpiler since we use the player collider
        // NEEDTOVALIDATE: Should I use a transpiler or do my own logic here.......
        // It may be better to use postfixes and prefixes since I can allow players to toggle 
        // whether they want the bots to take damage from traps!
        [HarmonyPatch("OnTriggerStay")]
        [HarmonyPostfix]
        static void OnTriggerStay_PostFix(SpikeRoofTrap __instance, Collider other)
        {
            if (!__instance.trapActive || !__instance.slammingDown || (Time.realtimeSinceStartup - __instance.timeSinceMovingUp) < 0.75f)
            {
                return;
            }
            PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
            if (component != null && LethalBotManager.Instance.IsPlayerLethalBotOwnerLocal(component) && !component.isPlayerDead)
            {
                component.KillPlayer(Vector3.down * 17f, spawnBody: true, CauseOfDeath.Crushing);
                return;
            }
        }
    }
}
