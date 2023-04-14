// <copyright file="LodDistanceBuildings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patches to adjust building LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceBuildings
    {
        /// <summary>
        /// Minimum permitted building LOD visiblity distance.
        /// This matches the minimum bounds check of 1000f for buildings in the game's code.
        /// </summary>
        internal const float MinBuildingDistance = 1000f;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxBuildingDistance = 10000f;

        /// <summary>
        /// Default minimum building LOD distance.
        /// </summary>
        internal const float DefaultBuildingMinDistance = 1000f;

        /// <summary>
        /// Default building LOD distance multiplier.
        /// </summary>
        internal const float DefaultBuildingMult = 1f;

        /// <summary>
        /// Minimum permitted building LOD distance multiplier.
        /// </summary>
        internal const float MinBuildingMult = 0.5f;

        /// <summary>
        /// Maximum permitted building LOD distance multiplier.
        /// </summary>
        internal const float MaxBuildingMult = 10f;

        // LOD visibility distance modifiers.
        private static float s_buildingMinDistance = DefaultBuildingMinDistance;
        private static float s_buildingMult = DefaultBuildingMult;

        /// <summary>
        /// Gets or sets the minimum building LOD visibility distance.
        /// Building distances are calculated dynamically based on size, so this merely enforces the minimum (game default is 1000f).
        /// </summary>
        internal static float BuildingMinDistance
        {
            get => s_buildingMinDistance;

            set
            {
                // Enforce bounds.
                s_buildingMinDistance = Mathf.Clamp(value, MinBuildingDistance, MaxBuildingDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    PrefabManager.RefreshLODs<BuildingInfo>();
                    PrefabManager.RefreshLODs<BuildingInfoSub>();
                    PrefabManager.UpdateRenderGroups(LayerMask.NameToLayer("Buildings"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the building LOD distance multiplier.
        /// Building distances are calculated dynamically based on size; this multiplier is applied to the game distance calculation.
        /// </summary>
        internal static float BuildingMultiplier
        {
            get => s_buildingMult;

            set
            {
                // Enforce bounds.
                s_buildingMult = Mathf.Clamp(value, MinBuildingMult, MaxBuildingMult);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    PrefabManager.RefreshLODs<BuildingInfo>();
                    PrefabManager.RefreshLODs<BuildingInfoSub>();
                    PrefabManager.UpdateRenderGroups(LayerMask.NameToLayer("Buildings"));
                }
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="BuildingInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="BuildingInfo"/> instance.</param>
        [HarmonyPatch(typeof(BuildingInfo), nameof(BuildingInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        public static void BuildingInfoRefreshLOD(BuildingInfo __instance)
        {
            // Only applies to instances with LODs.
            if (__instance.m_lodMesh != null)
            {
                __instance.m_minLodDistance = Mathf.Max(s_buildingMinDistance, __instance.m_minLodDistance * s_buildingMult);
                __instance.m_maxLodDistance = Mathf.Max(s_buildingMinDistance, __instance.m_maxLodDistance * s_buildingMult);
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="BuildingInfoSub.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="BuildingInfoSub"/> instance.</param>
        [HarmonyPatch(typeof(BuildingInfoSub), nameof(BuildingInfoSub.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        public static void BuildingInfoSubRefreshLOD(BuildingInfoSub __instance)
        {
            // Only applies to instances with LODs.
            if (__instance.m_lodMesh != null)
            {
                __instance.m_minLodDistance = Mathf.Max(s_buildingMinDistance, __instance.m_minLodDistance * s_buildingMult);
                __instance.m_maxLodDistance = Mathf.Max(s_buildingMinDistance, __instance.m_maxLodDistance * s_buildingMult);
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="BuildingAI.PopulateGroupData"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="BuildingAI"/> instance.</param>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(BuildingAI), nameof(BuildingAI.PopulateGroupData))]
        [HarmonyPostfix]
        public static void BuildingPopulateGroupData(BuildingAI __instance, int layer, ref float maxInstanceDistance)
        {
            // Ensure correct layer.
            if (__instance.m_info.m_prefabDataLayer == layer)
            {
                maxInstanceDistance = Mathf.Max(s_buildingMinDistance, maxInstanceDistance * s_buildingMult);
            }
        }
    }
}