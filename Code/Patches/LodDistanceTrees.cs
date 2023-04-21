// <copyright file="LodDistanceTrees.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Harmony patches to adjust tree LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceTrees
    {
        /// <summary>
        /// Minimum permitted LOD visiblity distance.
        /// </summary>
        internal const float MinTreeDistance = 0f;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxTreeDistance = 10000f;

        /// <summary>
        /// Default tree LOD visibility distance.  Based on game settings at highest level of detail (4f * 300f).
        /// </summary>
        internal const float DefaultTreeDistance = 1200f;

        // LOD visibility distance modifiers.
        private static float s_treeDistance = DefaultTreeDistance;

        /// <summary>
        /// Gets or sets the tree LOD visibility distance.
        /// </summary>
        internal static float TreeLodDistance
        {
            get => s_treeDistance;

            set
            {
                // Enforce bounds.
                s_treeDistance = Mathf.Clamp(value, MinTreeDistance, MaxTreeDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Refreshes tree visibility.
        /// </summary>
        internal static void RefreshVisibility()
        {
            PrefabManager.RefreshLODs<TreeInfo>();
            PrefabManager.UpdateRenderGroups(TreeManager.instance.m_treeLayer);
        }

        /// <summary>
        /// Harmony postfix to <see cref="TreeInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="TreeInfo"/> instance.</param>
        [HarmonyPatch(typeof(TreeInfo), nameof(TreeInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void TreeRefreshLOD(TreeInfo __instance)
        {
            __instance.m_lodRenderDistance = PrefabManager.LodMode ? 0 : s_treeDistance;
        }

        /// <summary>
        /// Harmony postfix to <see cref="TreeManager.PopulateGroupData"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="TreeManager"/> instance.</param>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(TreeManager), nameof(TreeManager.PopulateGroupData))]
        [HarmonyPostfix]
        private static void TreePopulateGroupData(TreeManager __instance, int layer, ref float maxInstanceDistance)
        {
            // Ensure correct layer.
            if (layer == __instance.m_treeLayer)
            {
                maxInstanceDistance = PrefabManager.LodMode ? 0 : Mathf.Max(maxInstanceDistance, s_treeDistance);
            }
        }
    }
}