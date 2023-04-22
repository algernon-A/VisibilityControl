// <copyright file="LodDistanceProps.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.Patches
{
    using System;
    using HarmonyLib;
    using UnityEngine;
    using static PrefabManager;

    /// <summary>
    /// Harmony patches to adjust prop LOD visibility distance ranges.
    /// </summary>
    [HarmonyPatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony")]
    internal static class LodDistanceProps
    {
        /// <summary>
        /// Minimum permitted prop LOD visiblity distance.
        /// This matches the minimum bounds check of 100f for props in the game's code.
        /// </summary>
        internal const float MinPropDistance = 100f;

        /// <summary>
        /// Maximum permitted LOD visibility distance.
        /// </summary>
        internal const float MaxPropDistance = 10000f;

        /// <summary>
        /// Default minimum prop LOD distance.
        /// </summary>
        internal const float DefaultPropMinDistance = 100f;

        /// <summary>
        /// Default prop LOD distance multiplier.
        /// </summary>
        internal const float DefaultPropMult = 1f;

        /// <summary>
        /// Minimum permitted prop LOD distance multiplier.
        /// </summary>
        internal const float MinPropMult = 0.5f;

        /// <summary>
        /// Maximum permitted prop LOD distance multiplier.
        /// </summary>
        internal const float MaxPropMult = 10f;

        /// <summary>
        /// Default decal fade distance (game default).
        /// </summary>
        internal const float DefaultDecalDistance = 1000f;

        // LOD visibility distance modifiers.
        private static float s_propMinDistance = DefaultPropMinDistance;
        private static float s_propMult = DefaultPropMult;
        private static float s_decalDistance = DefaultDecalDistance;

        /// <summary>
        /// Gets or sets the minimum prop LOD visibility distance.
        /// Prop distances are calculated dynamically based on size, so this merely enforces the minimum (game default is 100f).
        /// </summary>
        internal static float PropMinDistance
        {
            get => s_propMinDistance;

            set
            {
                // Enforce bounds.
                s_propMinDistance = Mathf.Clamp(value, MinPropDistance, MaxPropDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Gets or sets the prop LOD distance multiplier.
        /// Prop distances are calculated dynamically based on size; this multiplier is applied to the game distance calculation.
        /// </summary>
        internal static float PropMultiplier
        {
            get => s_propMult;

            set
            {
                // Enforce bounds.
                s_propMult = Mathf.Clamp(value, MinPropMult, MaxPropMult);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Gets or sets the decal fade distance.
        /// </summary>
        internal static float DecalFadeDistance
        {
            get => s_decalDistance;

            set
            {
                // Enforce bounds.
                s_decalDistance = Mathf.Clamp(value, MinPropDistance, MaxPropDistance);

                // Refresh prefabs if game is loaded.
                if (Loading.IsLoaded)
                {
                    RefreshVisibility();
                }
            }
        }

        /// <summary>
        /// Refreshes prop visibility.
        /// </summary>
        internal static void RefreshVisibility()
        {
            RefreshLODs<PropInfo>();
            UpdateRenderGroups(LayerMask.NameToLayer("Props"));
        }

        /// <summary>
        /// Harmony postfix to <see cref="PropInfo.RefreshLevelOfDetail"/> to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="__instance"><see cref="PropInfo"/> instance.</param>
        [HarmonyPatch(typeof(PropInfo), nameof(PropInfo.RefreshLevelOfDetail))]
        [HarmonyPostfix]
        private static void PropRefreshLOD(PropInfo __instance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Get current override distance.
            float overrideDistance = OverrideDistance;

            // Decal or prop?
            if (__instance.m_isDecal && __instance.m_material && __instance.m_material.shader.name.Equals("Custom/Props/Decal/Blend"))
            {
                // Decal.
                // Calculate override distance as max (otherwise decals won't be visibible at all in LOD mode).
                float convertedDistance = Mathf.Max(s_decalDistance, overrideDistance) * 1.1f;
                double decalFadeFactor = 1d / (convertedDistance * convertedDistance);

                // Apply visibility.
                __instance.m_lodRenderDistance = overrideDistance < 0 ? __instance.m_maxRenderDistance = convertedDistance : overrideDistance;
                __instance.m_material.SetFloat("_FadeDistanceFactor", (float)decalFadeFactor);
            }
            else
            {
                // Non-decal prop.
                __instance.m_lodRenderDistance = overrideDistance < 0 ? Mathf.Max(s_propMinDistance, __instance.m_lodRenderDistance * s_propMult) : overrideDistance;

                // Apply override distance as max, otherwise props won't be visible at all in LOD mode.
                __instance.m_maxRenderDistance = Mathf.Max(s_propMinDistance, __instance.m_maxRenderDistance * s_propMult, overrideDistance);
            }
        }

        /// <summary>
        /// Harmony postfix to <see cref="PropInstance.PopulateGroupData(PropInfo, int, InstanceID, Vector3, float, float, Color, ref int, ref int, Vector3, RenderGroup.MeshData, ref Vector3, ref Vector3, ref float, ref float)"/>
        /// to apply custom LOD visibility distance modifers.
        /// </summary>
        /// <param name="info">Instance <see cref="PropInfo"/> prefab.</param>
        /// <param name="layer">Render group layer.</param>
        /// <param name="maxInstanceDistance">Maximum instance visibility distance.</param>
        [HarmonyPatch(typeof(PropInstance), nameof(PropInstance.PopulateGroupData))]

        [HarmonyPatch(
            new Type[]
            {
                typeof(PropInfo),
                typeof(int),
                typeof(InstanceID),
                typeof(Vector3),
                typeof(float),
                typeof(float),
                typeof(Color),
                typeof(int),
                typeof(int),
                typeof(Vector3),
                typeof(RenderGroup.MeshData),
                typeof(Vector3),
                typeof(Vector3),
                typeof(float),
                typeof(float),
            },
            new ArgumentType[]
            {
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Ref,
                ArgumentType.Ref,
                ArgumentType.Normal,
                ArgumentType.Normal,
                ArgumentType.Ref,
                ArgumentType.Ref,
                ArgumentType.Ref,
                ArgumentType.Ref,
            })]
        [HarmonyPostfix]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Long Harmony annotation")]
        private static void PropPopulateGroupData(PropInfo info, int layer, ref float maxInstanceDistance)
        {
            // Don't do anything if using vanilla settings.
            if (CurrentMode == OverrideMode.Vanilla)
            {
                return;
            }

            // Ensure correct layer.
            if (info.m_prefabDataLayer == layer)
            {
                maxInstanceDistance = Mathf.Max(PropMinDistance, maxInstanceDistance * s_propMult);
            }
        }
    }
}