// <copyright file="LodDistance.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patches to adjust LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistance
    {
        /// <summary>
        /// Minimum permitted LOD visiblity distance.
        /// </summary>
        internal const float MinLodDistance = 0f;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxLodDistance = 10000f;

        /// <summary>
        /// Minimum permitted building visiblity distance.
        /// This matches the minimum bounds check of 1000f for buildings in the game's code.
        /// </summary>
        internal const float MinBuildingDistance = 1000f;

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

        /// <summary>
        /// Default tree LOD visibility distance.  Based on game settings at highest level of detail (4f * 300f).
        /// </summary>
        internal const float DefaultTreeDistance = 1200f;

        // LOD visibility distance modifiers.
        private static float s_buildingMinDistance = DefaultBuildingMinDistance;
        private static float s_buildingMult = DefaultBuildingMult;
        private static float s_treeDistance = DefaultTreeDistance;

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
                s_buildingMinDistance = Mathf.Clamp(value, MinBuildingDistance, MaxLodDistance);

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
        /// Gets or sets the tree LOD visibility distance.
        /// </summary>
        internal static float TreeLodDistance
        {
            get => s_treeDistance;
            set
            {
                // Enforce bounds.
                s_treeDistance = Mathf.Clamp(value, MinLodDistance, MaxLodDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    PrefabManager.RefreshLODs<TreeInfo>();
                    PrefabManager.UpdateRenderGroups(TreeManager.instance.m_treeLayer);
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

        /// <summary>
        /// Harmony postfix to <see cref="TreeInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="TreeInfo"/> instance.</param>
        [HarmonyPatch(typeof(TreeInfo), nameof(TreeInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        public static void TreeRefreshLOD(TreeInfo __instance)
        {
            __instance.m_lodRenderDistance = TreeLodDistance;
        }

        /// <summary>
        /// Harmony postfix to <see cref="TreeManager.PopulateGroupData"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="TreeManager"/> instance.</param>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(TreeManager), nameof(TreeManager.PopulateGroupData))]
        [HarmonyPostfix]
        public static void TreePopulateGroupData(TreeManager __instance, int layer, ref float maxInstanceDistance)
        {
            // Ensure correct layer.
            if (layer == __instance.m_treeLayer)
            {
                maxInstanceDistance = Mathf.Max(maxInstanceDistance, TreeLodDistance);
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="NetManager.PopulateGroupData"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(NetManager), nameof(NetManager.PopulateGroupData))]
        public static void NetPopulateGroupData(int layer, ref float maxInstanceDistance)
        {
            if (layer == LayerMask.NameToLayer("Road"))
            {
                maxInstanceDistance = Mathf.Max(maxInstanceDistance, NetDistance);
            }
        }

        [HarmonyPatch(typeof(NetInfo), "RefreshLevelOfDetail")]
        public static class NetInfoRefreshLevelOfDetailPatch
        {
            public static void Postfix(NetInfo __instance)
            {
                var lodTogglerFactor = TrueLodTogglerMod.ActiveConfig.NetworkLodDistance / 1000f;
                if (__instance.m_segments != null)
                {
                    for (int i = 0; i < __instance.m_segments.Length; i++)
                    {
                        if (__instance.m_segments[i].m_lodMesh != null)
                        {
                            __instance.m_segments[i].m_lodRenderDistance *= lodTogglerFactor;
                        }
                    }
                }
                if (__instance.m_nodes != null)
                {
                    for (int j = 0; j < __instance.m_nodes.Length; j++)
                    {
                        if (__instance.m_nodes[j].m_lodMesh != null)
                        {
                            __instance.m_nodes[j].m_lodRenderDistance *= lodTogglerFactor;
                        }
                    }
                }
                __instance.m_maxPropDistance *= lodTogglerFactor;
            }
        }
    }
}