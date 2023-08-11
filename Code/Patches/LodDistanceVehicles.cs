// <copyright file="LodDistanceVehicles.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using static PrefabManager;

    /// <summary>
    /// Harmony patches to adjust vehicle LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceVehicles
    {
        /// <summary>
        /// Minimum permitted vehicle LOD visibility distance.
        /// </summary>
        internal const float MinVehicleDistance = 1;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxVehicleDistance = 10000f;

        /// <summary>
        /// Default minimum vehicle LOD distance.
        /// This matches the minimum bounds check of 100f for vehicle in the game's code.
        /// </summary>
        internal const float DefaultVehicleMinDistance = 100f;

        /// <summary>
        /// Default vehicle LOD distance multiplier.
        /// </summary>
        internal const float DefaultVehicleMult = 1f;

        /// <summary>
        /// Minimum permitted vehicle LOD distance multiplier.
        /// </summary>
        internal const float MinVehicleMult = 0.1f;

        /// <summary>
        /// Maximum permitted vehicle LOD distance multiplier.
        /// </summary>
        internal const float MaxVehicleMult = 10f;

        // LOD visibility distance modifiers.
        private static float s_vehicleMinDistance = DefaultVehicleMinDistance;
        private static float s_vehicleMult = DefaultVehicleMult;

        /// <summary>
        /// Gets or sets the minimum vehicle LOD visibility distance.
        /// Vehicle distances are calculated dynamically based on size, so this merely enforces the minimum (game default is 100f).
        /// </summary>
        internal static float VehicleMinDistance
        {
            get => s_vehicleMinDistance;

            set
            {
                // Enforce bounds.
                s_vehicleMinDistance = Mathf.Clamp(value, MinVehicleDistance, MaxVehicleDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshLODs<VehicleInfo>();
                }
            }
        }

        /// <summary>
        /// Gets or sets the vehicle LOD distance multiplier.
        /// Vehicle distances are calculated dynamically based on size; this multiplier is applied to the game distance calculation.
        /// </summary>
        internal static float VehicleMultiplier
        {
            get => s_vehicleMult;

            set
            {
                // Enforce bounds.
                s_vehicleMult = Mathf.Clamp(value, MinVehicleMult, MaxVehicleMult);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshLODs<VehicleInfo>();
                }
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="VehicleInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifiers.
        /// </summary>
        /// <param name="__instance"><see cref="VehicleInfo"/> instance.</param>
        [HarmonyPatch(typeof(VehicleInfo), nameof(VehicleInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void VehicleRefreshLOD(VehicleInfo __instance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Get current override distance.
            float overrideDistance = OverrideDistance;

            __instance.m_lodRenderDistance = overrideDistance < 0f ? Mathf.Max(s_vehicleMinDistance, __instance.m_lodRenderDistance * s_vehicleMult) : overrideDistance;

            // Exclude vehicle max render distance from LOD effects.
            __instance.m_maxRenderDistance = Mathf.Max(s_vehicleMinDistance, __instance.m_maxRenderDistance * s_vehicleMult, overrideDistance);
        }

        /// <summary>
        /// Harmony postfix to <see cref="VehicleInfoBase.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifiers.
        /// </summary>
        /// <param name="__instance"><see cref="VehicleInfoBase"/> instance.</param>
        [HarmonyPatch(typeof(VehicleInfoBase), nameof(VehicleInfoBase.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void VehicleSubRefreshLOD(VehicleInfoBase __instance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Get current override distance.
            float overrideDistance = OverrideDistance;

            __instance.m_lodRenderDistance = overrideDistance < 0f ? Mathf.Max(s_vehicleMinDistance, __instance.m_lodRenderDistance * s_vehicleMult) : overrideDistance;

            // Exclude vehicle max render distance from LOD effects.
            __instance.m_maxRenderDistance = Mathf.Max(s_vehicleMinDistance, __instance.m_maxRenderDistance * s_vehicleMult, overrideDistance);
        }

        /// <summary>
        /// Harmony postfix to <see cref="VehicleInfoSub.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifiers.
        /// </summary>
        /// <param name="__instance"><see cref="VehicleInfoSub"/> instance.</param>
        [HarmonyPatch(typeof(VehicleInfoSub), nameof(VehicleInfoSub.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void VehicleSubRefreshLOD(VehicleInfoSub __instance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Get current override distance.
            float overrideDistance = OverrideDistance;

            __instance.m_lodRenderDistance = overrideDistance < 0f ? Mathf.Max(s_vehicleMinDistance, __instance.m_lodRenderDistance * s_vehicleMult) : overrideDistance;

            // Exclude vehicle max render distance from LOD effects.
            __instance.m_maxRenderDistance = Mathf.Max(s_vehicleMinDistance, __instance.m_maxRenderDistance * s_vehicleMult, overrideDistance);
        }
    }
}