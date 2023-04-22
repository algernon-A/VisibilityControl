// <copyright file="LodDistanceNets.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using static PrefabManager;

    /// <summary>
    /// Harmony patches to adjust network LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceNets
    {
        /// <summary>
        /// Minimum permitted network LOD visiblity distance.
        /// This matches the minimum bounds check of 100f for networks in the game's code.
        /// </summary>
        internal const float MinNetDistance = 100f;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxNetDistance = 10000f;

        /// <summary>
        /// Default minimum network LOD distance.
        /// </summary>
        internal const float DefaultNetMinDistance = 100f;

        /// <summary>
        /// Default network LOD distance multiplier.
        /// </summary>
        internal const float DefaultNetMult = 1f;

        /// <summary>
        /// Minimum permitted network LOD distance multiplier.
        /// </summary>
        internal const float MinNetMult = 0.5f;

        /// <summary>
        /// Maximum permitted network LOD distance multiplier.
        /// </summary>
        internal const float MaxNetMult = 10f;

        // LOD visibility distance modifiers.
        private static float s_netMinDistance = DefaultNetMinDistance;
        private static float s_netMult = DefaultNetMult;

        /// <summary>
        /// Gets or sets the minimum network LOD visibility distance.
        /// Network distances are calculated dynamically based on size, so this merely enforces the minimum (game default is 100f).
        /// </summary>
        internal static float NetMinDistance
        {
            get => s_netMinDistance;

            set
            {
                // Enforce bounds.
                s_netMinDistance = Mathf.Clamp(value, MinNetDistance, MaxNetDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Gets or sets the network LOD distance multiplier.
        /// Network distances are calculated dynamically based on size; this multiplier is applied to the game distance calculation.
        /// </summary>
        internal static float NetMultiplier
        {
            get => s_netMult;

            set
            {
                // Enforce bounds.
                s_netMult = Mathf.Clamp(value, MinNetMult, MaxNetMult);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Refreshes network visibility.
        /// </summary>
        internal static void RefreshVisibility()
        {
            RefreshLODs<NetInfo>();
            UpdateRenderGroups(LayerMask.NameToLayer("Road"));
        }

        /// <summary>
        /// Harmony postfix to <see cref="NetInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="NetInfo"/> instance.</param>
        [HarmonyPatch(typeof(NetInfo), nameof(NetInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void NetRefreshLOD(NetInfo __instance)
        {
            // Get current override distance.
            float overrideDistance = OverrideDistance;

            // Iterate through all segments in net.
            NetInfo.Segment[] segments = __instance.m_segments;
            if (segments != null)
            {
                for (int i = 0; i < segments.Length; ++i)
                {
                    // Only applies to segments with LODs.
                    if (segments[i].m_lodMesh != null)
                    {
                        segments[i].m_lodRenderDistance = overrideDistance < 0f ? Mathf.Max(s_netMinDistance, segments[i].m_lodRenderDistance * s_netMult) : overrideDistance;
                    }
                }
            }

            // Iterate through all nodes in net.
            NetInfo.Node[] nodes = __instance.m_nodes;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length; ++i)
                {
                    // Only applies to segments with LODs.
                    if (nodes[i].m_lodMesh != null)
                    {
                        nodes[i].m_lodRenderDistance = overrideDistance < 0f ? Mathf.Max(s_netMinDistance, nodes[i].m_lodRenderDistance * s_netMult) : overrideDistance;
                    }
                }
            }

            // Set network prop distance multiplier.
            __instance.m_maxPropDistance = overrideDistance < 0f ? Mathf.Max(s_netMinDistance, __instance.m_maxPropDistance * s_netMult) : overrideDistance;
        }

        /// <summary>
        /// Harmony postfix to <see cref="NetManager.PopulateGroupData"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(NetManager), nameof(NetManager.PopulateGroupData))]
        [HarmonyPostfix]
        private static void NetPopulateGroupData(int layer, ref float maxInstanceDistance)
        {
            // Ensure correct layer.
            if (layer == LayerMask.NameToLayer("Road"))
            {
                // Get current override distance.
                float overrideDistance = OverrideDistance;

                // Set maximum visibility distance for this item.
                maxInstanceDistance = overrideDistance < 0f ? Mathf.Max(s_netMinDistance, maxInstanceDistance * s_netMult) : overrideDistance;
            }
        }
    }
}