using GameNetcodeStuff;
using HarmonyLib;
using LethalBots.AI;
using LethalBots.Managers;
using LethalBots.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

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

        // A function we use to find that dang TerminalAccessibleObject
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start_PostFix(SpikeRoofTrap __instance)
        {
            // Ok, so I already know that GetComponent, GetComponentInChildren, and GetComponentInParent on the trap itself doesnt work, could it be on the animator or something?
            // LIST OF FIELDS TESTED:
            // Instance: FAILED
            // spikeTrapAnimator: FAILED
            // laserEye: FAILED
            // stickingPointsContainer: FAILED
            // spikeTrapAudio: FAILED
            HUDManager.Instance?.DisplayTip("Spike Roof Trap Spawned", "Check the logs!!!!!!");
            //TerminalAccessibleObject terminalAccessibleObject = __instance.spikeTrapAudio.GetComponent<TerminalAccessibleObject>();
            //if (terminalAccessibleObject == null)
            //{
            //    terminalAccessibleObject = __instance.spikeTrapAudio.GetComponentInParent<TerminalAccessibleObject>();
            //    if (terminalAccessibleObject == null)
            //    {
            //        terminalAccessibleObject = __instance.spikeTrapAudio.GetComponentInChildren<TerminalAccessibleObject>();
            //        if (terminalAccessibleObject == null)
            //        {
            //            Plugin.LogDebug("Failed to find TerminalAccessibleObject in SpikeRoofTrap! :(");
            //        }
            //        else
            //        {
            //            Plugin.LogDebug("SpikeRoofTrap had TerminalAccessibleObject in animator under GetComponentInChildren.");
            //        }
            //    }
            //    else
            //    {
            //        Plugin.LogDebug("SpikeRoofTrap had TerminalAccessibleObject in animator under GetComponentInParent.");
            //    }
            //}
            //else
            //{
            //    Plugin.LogDebug("SpikeRoofTrap had TerminalAccessibleObject in animator under GetComponent.");
            //}
            // I FOUND IT, YAY! Its attacted to the Containter Transform object.
            // Related logs:
            //[Debug  :LethalBots] Container (UnityEngine.Transform): 
            //[Debug: LethalBots]
            //[Debug: LethalBots] List of components:
            //[Debug: LethalBots] Container(UnityEngine.Transform)
            //[Debug: LethalBots] Child components :
            //[Debug: LethalBots] Container(UnityEngine.Transform)
            //[Debug: LethalBots] AnimContainer(UnityEngine.Transform)
            //[Debug: LethalBots] AnimContainer(UnityEngine.Animator)
            //[Debug: LethalBots] AnimContainer(PlayAudioAnimationEvent)
            //[Debug: LethalBots] SpikeAudio(UnityEngine.Transform)
            //[Debug: LethalBots] SpikeAudio(UnityEngine.AudioSource)
            //[Debug: LethalBots] SpikeAudio(OccludeAudio)
            //[Debug: LethalBots] SpikeAudio(UnityEngine.AudioLowPassFilter)
            //[Debug: LethalBots] SpikeAudio(UnityEngine.AudioReverbFilter)
            //[Debug: LethalBots] BaseSupport(UnityEngine.Transform)
            //[Debug: LethalBots] BaseSupport(UnityEngine.MeshFilter)
            //[Debug: LethalBots] BaseSupport(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] SpikeRoof(UnityEngine.Transform)
            //[Debug: LethalBots] SpikeRoof(UnityEngine.MeshFilter)
            //[Debug: LethalBots] SpikeRoof(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] MovingBar(UnityEngine.Transform)
            //[Debug: LethalBots] MovingBar(UnityEngine.MeshFilter)
            //[Debug: LethalBots] MovingBar(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] BodyStickyPoints(UnityEngine.Transform)
            //[Debug: LethalBots] Spot Light(UnityEngine.Transform)
            //[Debug: LethalBots] Spot Light(UnityEngine.Light)
            //[Debug: LethalBots] Spot Light(UnityEngine.Rendering.HighDefinition.HDAdditionalLightData)
            //[Debug: LethalBots] Trigger(UnityEngine.Transform)
            //[Debug: LethalBots] Trigger(UnityEngine.MeshFilter)
            //[Debug: LethalBots] Trigger(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] Trigger(UnityEngine.BoxCollider)
            //[Debug: LethalBots] Trigger(SpikeRoofTrap)
            //[Debug: LethalBots] Cube(UnityEngine.Transform)
            //[Debug: LethalBots] Cube(UnityEngine.MeshFilter)
            //[Debug: LethalBots] Cube(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] Cube(UnityEngine.BoxCollider)
            //[Debug: LethalBots] Colliders(UnityEngine.Transform)
            //[Debug: LethalBots] Cube(UnityEngine.Transform)
            //[Debug: LethalBots] Cube(UnityEngine.MeshFilter)
            //[Debug: LethalBots] Cube(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] Cube(UnityEngine.BoxCollider)
            //[Debug: LethalBots] Cube(2)(UnityEngine.Transform)
            //[Debug: LethalBots] Cube(2)(UnityEngine.MeshFilter)
            //[Debug: LethalBots] Cube(2)(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] Cube(2)(UnityEngine.BoxCollider)
            //[Debug: LethalBots] Cube(1)(UnityEngine.Transform)
            //[Debug: LethalBots] Cube(1)(UnityEngine.MeshFilter)
            //[Debug: LethalBots] Cube(1)(UnityEngine.MeshRenderer)
            //[Debug: LethalBots] Cube(1)(UnityEngine.BoxCollider)
            //[Debug: LethalBots] LaserEye(UnityEngine.Transform)
            //[Debug: LethalBots] TerminalObjectContainer(UnityEngine.Transform)
            //[Debug: LethalBots] TerminalObjectContainer(TerminalAccessibleObject)
            //Plugin.LogDebug("List of components :");
            //Component[] components = __instance.gameObject.GetComponents(typeof(Component));
            //foreach (Component component in components)
            //{
            //    if (component == null) continue;
            //    Plugin.LogDebug(component.ToString() + ": ");
            //    ComponentUtil.ListAllComponents(component.gameObject);
            //}

            //Plugin.LogDebug("Child components :");
            //components = __instance.gameObject.GetComponentsInChildren(typeof(Component));
            //foreach (Component component in components)
            //{
            //    if (component == null) continue;
            //    Plugin.LogDebug(component.ToString() + ": ");
            //    ComponentUtil.ListAllComponents(component.gameObject);
            //}

            //Plugin.LogDebug("Parent components :");
            //components = __instance.gameObject.GetComponentsInParent(typeof(Component));
            //foreach (Component component in components)
            //{
            //    if (component == null) continue;
            //    Plugin.LogDebug(component.ToString() + ": ");
            //    ComponentUtil.ListAllComponents(component.gameObject);
            //}

            // Ok, based on the log file, it appears to be in Parent side, so lets see if this works....
            Plugin.LogDebug($"Attempting to find TerminalAccessableObject in {__instance}");
            Component[] components = __instance.gameObject.GetComponentsInParent<Component>();
            foreach (Component component in components)
            {
                if (component == null) continue;
                Plugin.LogDebug($"Checking if {component} has TerminalAccessableObject");
                TerminalAccessibleObject terminalAccessible = component.GetComponent<TerminalAccessibleObject>();
                Plugin.LogDebug($"{component} {(terminalAccessible != null ? "did" : "did not")} have a terminal accessable object!");
            }

            //Plugin.LogDebug($"Listing all components in {__instance}");
            //ComponentUtil.ListAllComponents(__instance.gameObject);
        }
    }
}
