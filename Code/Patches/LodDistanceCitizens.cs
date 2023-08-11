// <copyright file="LodDistanceCitizens.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using static PrefabManager;

    /// <summary>
    /// Harmony patches to adjust citizen LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch(typeof(CitizenInfo))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceCitizens
    {
        /// <summary>
        /// Minimum permitted citizen LOD transition distance.
        /// The game has a minimum bounds check of 150f for citizens in the game's code at the lowest detail level.
        /// </summary>
        internal const float MinCitizenLodDistance = 1;

        /// <summary>
        /// Maximum permitted LOD transition distance.
        /// </summary>
        internal const float MaxCitizenLodDistance = 10000f;

        /// <summary>
        /// Default citizen LOD transition distance.
        /// This matches the value of 4f * 150f for citizens in the game's code at the highest detail level.
        /// </summary>
        internal const float DefaultCitizenLodDistance = 600f;

        /// <summary>
        /// Minimum permitted citizen visiblity distance.
        /// The game has a minimum bounds check of 400f for citizens in the game's code at the lowest detail level, so this is half that.
        /// </summary>
        internal const float MinCitizenMaxDistance = 200f;

        /// <summary>
        /// Maximum permitted citizen visibility distance.
        /// </summary>
        internal const float MaxCitizenMaxDistance = 10000f;

        /// <summary>
        /// Default citizen visibility distance.
        /// This matches the value of 4f * 400f for citizens in the game's code at the highest detail level.
        /// </summary>
        internal const float DefaultCitizenMaxDistance = 1600f;

        // LOD visibility distance modifiers.
        private static float s_citizenLodDistance = DefaultCitizenLodDistance;
        private static float s_citizenMaxDistance = DefaultCitizenMaxDistance;

        /// <summary>
        /// Gets or sets the citizen LOD transition distance.
        /// </summary>
        internal static float CitizenLodDistance
        {
            get => s_citizenLodDistance;

            set
            {
                // Enforce bounds.
                s_citizenLodDistance = Mathf.Clamp(value, MinCitizenLodDistance, MaxCitizenLodDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshLODs<CitizenInfo>();
                }
            }
        }

        /// <summary>
        /// Gets or sets the citizen maximum visibility distance.
        /// </summary>
        internal static float CitizenMaxDistance
        {
            get => s_citizenMaxDistance;

            set
            {
                // Enforce bounds.
                s_citizenMaxDistance = Mathf.Clamp(value, MinCitizenMaxDistance, MaxCitizenMaxDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshLODs<CitizenInfo>();
                }
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="CitizenInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifiers.
        /// </summary>
        /// <param name="__instance"><see cref="CitizenInfo"/> instance.</param>
        [HarmonyPatch(nameof(CitizenInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void CitizenRefreshLOD(CitizenInfo __instance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Only applies to instances with LODs.
            if (__instance.m_lodMesh != null)
            {
                // Get current override distance.
                float overrideDistance = OverrideDistance;

                __instance.m_lodRenderDistance = overrideDistance < 0f ? s_citizenLodDistance : overrideDistance;
                __instance.m_maxRenderDistance = s_citizenMaxDistance;
            }
        }
    }
}