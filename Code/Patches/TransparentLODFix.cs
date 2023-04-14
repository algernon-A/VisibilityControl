// <copyright file="TransparentLODFix.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using System.Collections.Generic;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Addresses the issue of the rotors shader (used for transparency) not working for LODs,
    /// by increasing the asset's visibility distance.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmnony")]
    internal class TransparentLODFix
    {
        /// <summary>
        /// Minimum permitted fallback distance.
        /// </summary>
        internal const float MinFallbackDistance = 500f;

        /// <summary>
        /// Maximum permitted fallback distance.
        /// </summary>
        internal const float MaxFallbackDistance = 100000f;

        /// <summary>
        /// Default fallback render distance.
        /// </summary>
        internal const float DefaultFallbackDistance = 1000f;

        /// <summary>
        /// Minimum permitted minimum distance.
        /// </summary>
        internal const float MinMinimumDistance = 1f;

        /// <summary>
        /// Maximum permitted minimum distance.
        /// </summary>
        internal const float MaxMinimumDistance = 1000f;

        /// <summary>
        /// Default minimum distance.
        /// </summary>
        internal const float DefaultMinimumDistance = 100f;

        /// <summary>
        /// Minimum permitted distance multiplier.
        /// </summary>
        internal const float MinDistanceMultiplier = 1f;

        /// <summary>
        /// Maximum permitted distance multiplier.
        /// </summary>
        internal const float MaxDistanceMultiplier = 1000f;

        /// <summary>
        /// Default distance multiplier.
        /// </summary>
        internal const float DefaultDistanceMultiplier = 100f;

        /// <summary>
        /// Minimum permitted LOD transition distance multiplier.
        /// </summary>
        internal const float MinLODTransitionMultiplier = 0.05f;

        /// <summary>
        /// Maximum permitted LOD transition distance multiplier.
        /// </summary>
        internal const float MaxLODTransitionMultiplier = 1.0f;

        /// <summary>
        /// Default LOD transition distance multiplier.
        /// </summary>
        internal const float DefaultLODTransitionMultiplier = 0.25f;

        // Maximum rendering distance for props.
        private const float PropRenderDistanceMaximum = 100000f; // Game default 1000f

        // Maximum rendering distance for props effects.
        private const float EffectRenderDistanceMaximum = 100000f; // Game default 1000f

        // Prop Control Harmony ID.
        private const string PropControlHarmonyID = "com.github.algernon-A.csl.propcontrol";

        // LOD rendering factors.
        private static float s_buildingFallbackRenderDistance = DefaultFallbackDistance;
        private static float s_buildingMinimumDistance = DefaultMinimumDistance;
        private static float s_buildingLODTransitionMultiplier = DefaultDistanceMultiplier;

        private static float s_propFallbackRenderDistance = DefaultFallbackDistance;
        private static float s_propMinimumDistance = DefaultMinimumDistance;
        private static float s_propDistanceMultiplier = DefaultDistanceMultiplier;
        private static float s_propLODTransitionMultipler = DefaultLODTransitionMultiplier;

        /// <summary>
        /// Gets the hashset of <see cref="BuildingInfo"/> prefabs using the rotors shader for the transparent LOD fix.
        /// </summary>
        internal static HashSet<BuildingInfo> ManagedBuildings { get; } = new HashSet<BuildingInfo>();

        /// <summary>
        /// Gets the hashset of <see cref="BuildingInfoSub"/> prefabs using the rotors shader for the transparent LOD fix.
        /// </summary>
        internal static HashSet<BuildingInfoSub> ManagedSubBuildings { get; } = new HashSet<BuildingInfoSub>();

        /// <summary>
        /// Gets the hashset of <see cref="PropInfo"/> prefabs using the rotors shader for the transparent LOD fix.
        /// </summary>
        internal static HashSet<PropInfo> ManagedProps { get; } = new HashSet<PropInfo>();

        /// <summary>
        /// Gets or sets the fallback building render distance.
        /// This is used when no other value can be calculated.
        /// </summary>
        internal static float BuildingFallbackRenderDistance
        {
            get => s_buildingFallbackRenderDistance;

            set
            {
                s_buildingFallbackRenderDistance = Mathf.Clamp(value, MinFallbackDistance, MaxFallbackDistance);
                RefreshBuildingPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the building minimum visibility distance.
        /// Buildings will always be visible within this distance.
        /// </summary>
        internal static float BuildingMinimumDistance
        {
            get => s_buildingMinimumDistance;

            set
            {
                s_buildingMinimumDistance = Mathf.Clamp(value, MinMinimumDistance, MaxMinimumDistance);
                RefreshBuildingPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the prop LOD transition multiplier.
        /// This determines how far away the model will transition from full mesh to LOD.
        /// </summary>
        internal static float BuildingLODTransitionMultiplier
        {
            get => s_buildingLODTransitionMultiplier;

            set
            {
                s_buildingLODTransitionMultiplier = Mathf.Clamp(value, MinLODTransitionMultiplier, MaxLODTransitionMultiplier);
                RefreshBuildingPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the prop fallback prop render distance.
        /// This is used when no other value can be calculated.
        /// </summary>
        internal static float PropFallbackRenderDistance
        {
            get => s_propFallbackRenderDistance;

            set
            {
                s_propFallbackRenderDistance = Mathf.Clamp(value, MinFallbackDistance, MaxFallbackDistance);
                RefreshPropPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the prop minimum visibility distance.
        /// Props will always be visible within this distance.
        /// </summary>
        internal static float PropMinimumDistance
        {
            get => s_propMinimumDistance;

            set
            {
                s_propMinimumDistance = Mathf.Clamp(value, MinMinimumDistance, MaxMinimumDistance);
                RefreshPropPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the prop distance multiplier.
        /// This determines how far away the prop is visible.
        /// </summary>
        internal static float PropDistanceMultiplier
        {
            get => s_propDistanceMultiplier;

            set
            {
                s_propDistanceMultiplier = Mathf.Clamp(value, MinDistanceMultiplier, MaxDistanceMultiplier);
                RefreshPropPrefabs();
            }
        }

        /// <summary>
        /// Gets or sets the prop LOD transition multiplier.
        /// This determines how far away the model will transition from full mesh to LOD.
        /// </summary>
        internal static float PropLODTransitionMultiplier
        {
            get => s_propLODTransitionMultipler;

            set
            {
                s_propLODTransitionMultipler = Mathf.Clamp(value, MinLODTransitionMultiplier, MaxLODTransitionMultiplier);
                RefreshPropPrefabs();
            }
        }

        /// <summary>
        /// Refreshes the visibility of all managed <see cref="BuildingInfo"/> prefabs.
        /// </summary>
        internal static void RefreshBuildingPrefabs()
        {
            foreach (BuildingInfo building in ManagedBuildings)
            {
                building.RefreshLevelOfDetail();
            }

            foreach (BuildingInfoSub subBuilding in ManagedSubBuildings)
            {
                subBuilding.RefreshLevelOfDetail();
            }
        }

        /// <summary>
        /// Refreshes the visibility of all managed <see cref="PropInfo"/> prefabs.
        /// </summary>
        internal static void RefreshPropPrefabs()
        {
            foreach (PropInfo prop in ManagedProps)
            {
                prop.RefreshLevelOfDetail();
            }
        }

        /// <summary>
        /// Harmony prefix to <see cref="BuildingInfo.RefreshLevelOfDetail"></see> to implement transparency LOD fix.
        /// </summary>
        /// <param name="__instance"><see cref="BuildingInfo"/> prefab.</param>
        /// <returns><c>false</c> (don't execute original method) if this prop is using the rotors shader, <c>true</c> (execute original method) otherwise.</returns>
        [HarmonyPatch(typeof(BuildingInfo), nameof(BuildingInfo.RefreshLevelOfDetail))]
        [HarmonyPrefix]
        private static bool RefreshLevelOfDetailPrefix(BuildingInfo __instance)
        {
            // We're only interested in managed prefabs.
            if (!ManagedBuildings.Contains(__instance))
            {
                // Not a managed prefab - execute original method instead.
                return true;
            }

            // Set building missing LOD flag.
            __instance.m_lodMissing = true;

            // Calculate maximum render distance.
            if (__instance.m_generatedInfo.m_triangleArea == 0f || float.IsNaN(__instance.m_generatedInfo.m_triangleArea))
            {
                // Invalid info for calculation - use fallback distance.
                __instance.m_minLodDistance = __instance.m_maxLodDistance = s_buildingFallbackRenderDistance;
            }
            else
            {
                // Calculate dynamic visibility distance.
                double distanceMultiplier = RenderManager.LevelOfDetailFactor * s_buildingLODTransitionMultiplier;
                __instance.m_minLodDistance = __instance.m_maxLodDistance = (float)((Mathf.Sqrt(__instance.m_generatedInfo.m_triangleArea) * distanceMultiplier) + s_buildingMinimumDistance);
            }

            __instance.m_minLodDistance = Mathf.Max(s_buildingFallbackRenderDistance, __instance.m_minLodDistance);
            __instance.m_maxLodDistance = Mathf.Max(s_buildingFallbackRenderDistance, __instance.m_maxLodDistance);

            // Don't execute original method if we've adjusted the visibility distances.
            return false;
        }

        /// <summary>
        /// Harmony prefix to <see cref="BuildingInfoSub.RefreshLevelOfDetail"></see> to implement transparency LOD fix.
        /// </summary>
        /// <param name="__instance"><see cref="BuildingInfoSub"/> prefab.</param>
        /// <returns><c>false</c> (don't execute original method) if this prop is using the rotors shader, <c>true</c> (execute original method) otherwise.</returns>
        [HarmonyPatch(typeof(BuildingInfoSub), nameof(BuildingInfoSub.RefreshLevelOfDetail))]
        [HarmonyPrefix]
        private static bool RefreshLevelOfDetailPrefix(BuildingInfoSub __instance)
        {
            // We're only interested in managed prefabs.
            if (!ManagedSubBuildings.Contains(__instance))
            {
                // Not a managed prefab - execute original method instead.
                return true;
            }

            // Set building missing LOD flag.
            __instance.m_hasLodData = false;

            // Calculate maximum render distance.
            if (__instance.m_generatedInfo.m_triangleArea == 0f || float.IsNaN(__instance.m_generatedInfo.m_triangleArea))
            {
                // Invalid info for calculation - use fallback distance.
                __instance.m_minLodDistance = __instance.m_maxLodDistance = s_buildingFallbackRenderDistance;
            }
            else
            {
                // Calculate dynamic visibility distance.
                double distanceMultiplier = RenderManager.LevelOfDetailFactor * s_buildingLODTransitionMultiplier;
                __instance.m_minLodDistance = __instance.m_maxLodDistance = (float)((Mathf.Sqrt(__instance.m_generatedInfo.m_triangleArea) * distanceMultiplier) + s_buildingMinimumDistance);
            }

            __instance.m_minLodDistance = Mathf.Max(s_buildingFallbackRenderDistance, __instance.m_minLodDistance);
            __instance.m_maxLodDistance = Mathf.Max(s_buildingFallbackRenderDistance, __instance.m_maxLodDistance);

            // Don't execute original method if we've adjusted the visibility distances.
            return false;
        }

        /// <summary>
        /// Harmony prefix to <see cref="PropInfo.RefreshLevelOfDetail"></see> to implement transparency LOD fix.
        /// </summary>
        /// <param name="__instance"><see cref="PropInfo"/> prefab.</param>
        /// <returns><c>false</c> (don't execute original method) if this prop is using the rotors shader, <c>true</c> (execute original method) otherwise.</returns>
        [HarmonyPatch(typeof(PropInfo), nameof(PropInfo.RefreshLevelOfDetail))]
        [HarmonyBefore(PropControlHarmonyID)]
        [HarmonyPrefix]
        private static bool RefreshLevelOfDetailPrefix(PropInfo __instance)
        {
            // We're only interested in managed prefabs.
            if (!ManagedProps.Contains(__instance))
            {
                // Not a managed prefab - execute original method instead.
                return true;
            }

            // Calculate maximum render distance.
            if (__instance.m_generatedInfo.m_triangleArea == 0.0f || float.IsNaN(__instance.m_generatedInfo.m_triangleArea))
            {
                // Invalid info for calculation - use fallback distance.
                __instance.m_maxRenderDistance = s_propFallbackRenderDistance;
            }
            else
            {
                // Calculate dynamic visibility distance.
                double lodFactor = RenderManager.LevelOfDetailFactor * s_propDistanceMultiplier;
                __instance.m_maxRenderDistance = Mathf.Min(PropRenderDistanceMaximum, (float)((Mathf.Sqrt(__instance.m_generatedInfo.m_triangleArea) * lodFactor) + s_propMinimumDistance));
            }

            // Calculate LOD render distance.
            if (__instance.m_isDecal | __instance.m_isMarker)
            {
                // Decals and markers have 0 LOD render distance.
                __instance.m_lodRenderDistance = 0f;
            }
            else
            {
                // Does this prop have a LOD mesh?
                if (__instance.m_lodMesh == null)
                {
                    // No LOD mesh - LOD render distance is the same as maximum render distance (so never render a LOD).
                    __instance.m_lodRenderDistance = __instance.m_maxRenderDistance;
                }
                else
                {
                    // LOD mesh presence - LOD render distance the maximum render distance multiplied by the LOD distance transition multipler.
                    __instance.m_lodRenderDistance = __instance.m_maxRenderDistance * s_propLODTransitionMultipler;
                }
            }

            // Update effect distances.
            if (__instance.m_effects != null)
            {
                for (int i = 0; i < __instance.m_effects.Length; ++i)
                {
                    if (__instance.m_effects[i].m_effect != null)
                    {
                        __instance.m_maxRenderDistance = Mathf.Max(__instance.m_maxRenderDistance, __instance.m_effects[i].m_effect.RenderDistance());
                    }
                }

                __instance.m_maxRenderDistance = Mathf.Min(EffectRenderDistanceMaximum, __instance.m_maxRenderDistance);
            }

            // Don't execute original method if we've adjusted the visibility distances.
            return false;
        }
    }
}
