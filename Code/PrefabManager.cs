// <copyright file="PrefabManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System;
    using System.Collections.Generic;
    using AlgernonCommons;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;
    using VisibilityControl.AdditiveShader;
    using VisibilityControl.Patches;

    /// <summary>
    /// Prefab management.
    /// </summary>
    internal static class PrefabManager
    {
        // Active detail override mode.
        private static OverrideMode s_currentMode = OverrideMode.None;

        /// <summary>
        /// Detail override modes.
        /// </summary>
        internal enum OverrideMode : int
        {
            /// <summary>
            /// No override (use player settings).
            /// </summary>
            None = 0,

            /// <summary>
            /// Vanilla mode (ignore mod settings).
            /// </summary>
            Vanilla,

            /// <summary>
            /// Screenshot mode override (use maximum detail).
            /// </summary>
            Screenshot,

            /// <summary>
            /// LOD mode override (LODs only).
            /// </summary>
            LOD,
        }

        /// <summary>
        /// Gets the current visibility distance override (-1f if none).
        /// </summary>
        internal static float OverrideDistance
        {
            get
            {
                switch (CurrentMode)
                {
                    case OverrideMode.LOD:
                        return 0f;

                    case OverrideMode.Screenshot:
                        return 100000f;

                    case OverrideMode.None:
                    default:
                        return -1f;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether LOD mod is active.
        /// </summary>
        internal static OverrideMode CurrentMode
        {
            get => s_currentMode;

            set
            {
                // Don't do anything if no change.
                if (s_currentMode != value)
                {
                    s_currentMode = value;

                    // Refresh all visibility settings.
                    LodDistanceBuildings.RefreshVisibility();
                    LodDistanceNets.RefreshVisibility();
                    LodDistanceTrees.RefreshVisibility();
                    LodDistanceProps.RefreshVisibility();
                    RefreshLODs<VehicleInfo>();
                }
            }
        }

        /// <summary>
        /// Scans loaded prefabs for relevant shader information and performs any necessary actions.
        /// </summary>
        internal static void ScanPrefabs()
        {
            // Additive shader assets.
            List<ManagedPrefab> additiveAssets = new List<ManagedPrefab>();

            // Clear transparent LOD fix records.
            TransparentLODFix.ManagedBuildings.Clear();
            TransparentLODFix.ManagedProps.Clear();

            // Iterate through all loaded prefabs.
            PrefabInfo[] prefabs = Resources.FindObjectsOfTypeAll<PrefabInfo>();
            for (int i = 0; i < prefabs.Length; ++i)
            {
                try
                {
                    PrefabInfo prefab = prefabs[i];
                    if (prefab is PropInfo prop)
                    {
                        // Prop - check for valid mesh.
                        if (prop.m_mesh is Mesh mesh)
                        {
                            // Check for additive shader.
                            if (mesh.name is string meshName && ShaderManager.HasShaderKey(meshName))
                            {
                                additiveAssets.Add(new ManagedPrefab(prop));
                            }

                            // Check for rotors shader for transparent LOD fix.
                            if (HasRotorsShader(prop.m_material))
                            {
                                Logging.Message("adding rotors shader LOD fix for prop ", prop.name);
                                TransparentLODFix.ManagedProps.Add(prop);
                            }
                        }
                    }
                    else if (prefab is BuildingInfo building)
                    {
                        // Building - check for valid mesh.
                        if (building.m_mesh is Mesh mesh)
                        {
                            // Check for additive shader.
                            if (mesh.name is string meshName && ShaderManager.HasShaderKey(meshName))
                            {
                                additiveAssets.Add(new ManagedPrefab(building, false));
                            }

                            // Check for rotors shader for transparent LOD fix.
                            if (HasRotorsShader(building.m_material))
                            {
                                Logging.Message("adding rotors shader LOD fix for building ", building.name);
                                TransparentLODFix.ManagedBuildings.Add(building);
                            }
                        }

                        // Prop LODs don't support the additive shader, so the maximum prop visibility distance needs to be extended instead if the building contains additive shader props.
                        // Mesh isn't relevant here, e.g. empty container building.
                        if (building.m_props != null && ShaderManager.ContainsShaderProps(building))
                        {
                            additiveAssets.Add(new ManagedPrefab(building, true));
                        }
                    }
                    else if (prefab is BuildingInfoSub buildingSub)
                    {
                        // Sub-building - check for valid mesh.
                        if (buildingSub.m_mesh is Mesh mesh)
                        {
                            // Sub-building - check for additive shader.
                            if (mesh.name is string meshName && ShaderManager.HasShaderKey(meshName))
                            {
                                additiveAssets.Add(new ManagedPrefab(buildingSub));
                            }

                            // Check for rotors shader for transparent LOD fix.
                            if (HasRotorsShader(buildingSub.m_material))
                            {
                                Logging.Message("adding rotors shader LOD fix for sub-building ", buildingSub.name);
                                TransparentLODFix.ManagedSubBuildings.Add(buildingSub);
                            }
                        }
                    }
                    else if (prefab is VehicleInfoSub vehicleSub)
                    {
                        // Sub-vehicle - check for additive shader.
                        if (vehicleSub.m_mesh?.name is string meshName && ShaderManager.HasShaderKey(meshName))
                        {
                            additiveAssets.Add(new ManagedPrefab(vehicleSub));
                        }
                    }
                }
                catch (Exception e)
                {
                    // Don't let a single failure stop us.
                    Logging.LogException(e, "exception parsing prefab data");
                }
            }

            // Did we find any prefabs using the additive shader?
            if (additiveAssets.Count > 0)
            {
                // Populate manages assets.
                ManagedPrefab[] managedAssets = additiveAssets.ToArray();
                ShaderManager.ManagedAssets = managedAssets;

                // Set initial visibility.
                for (int i = 0; i < managedAssets.Length; ++i)
                {
                    if (additiveAssets[i].IsContainer || additiveAssets[i].Profile.IsStatic)
                    {
                        // Container and static assets start on.
                        additiveAssets[i].SetVisibility(true);
                    }
                    else
                    {
                        // Other assets start off.
                        additiveAssets[i].SetVisibility(false);
                    }
                }

                // Start additive shader coroutine.
                UIView.GetAView().StartCoroutine(ShaderManager.ShaderCoroutine());
            }

            // Refresh LODs for any transparent lod fix prefabs.
            TransparentLODFix.RefreshBuildingPrefabs();
            TransparentLODFix.RefreshPropPrefabs();
        }

        /// <summary>
        /// Refresh the LODs for all prefabs of the specified type.
        /// </summary>
        /// <typeparam name="TPrefab">Prefab type.</typeparam>
        internal static void RefreshLODs<TPrefab>()
            where TPrefab : PrefabInfo
        {
            // Iterate through all loaded prefabs of the specified type.
            uint prefabCount = (uint)PrefabCollection<TPrefab>.LoadedCount();
            for (uint i = 0; i < prefabCount; ++i)
            {
                // Refresh LODs for all valid prefabs.
                PrefabInfo prefab = PrefabCollection<TPrefab>.GetLoaded(i);
                if (prefab)
                {
                    prefab.RefreshLevelOfDetail();
                }
            }

            // Also refresh any edit prefab.
            if (ToolsModifierControl.toolController && ToolsModifierControl.toolController.m_editPrefabInfo is TPrefab)
            {
                ToolsModifierControl.toolController.m_editPrefabInfo.RefreshLevelOfDetail();
            }
        }

        /// <summary>
        /// Update all <see cref="RenderGroup"/> for the given layer.
        /// </summary>
        /// <param name="layer">Render layer index.</param>
        internal static void UpdateRenderGroups(int layer)
        {
            // Local reference.
            RenderManager renderManager = Singleton<RenderManager>.instance;

            // Iterate throuh all render groups.
            foreach (RenderGroup renderGroup in renderManager.m_groups)
            {
                // Null check.
                if (renderGroup != null)
                {
                    // Refresh this group.
                    renderGroup.SetLayerDataDirty(layer);
                    renderGroup.UpdateMeshData();
                }
            }
        }

        /// <summary>
        /// Checks if the given <see cref="Material"/> uses the rotors shader.
        /// </summary>
        /// <param name="material"><see cref="Material"/> to check.</param>
        /// <returns><c>true</c> if the given <see cref="Material"/> uses the rotors shader, <c>false</c> otherwise.</returns>
        private static bool HasRotorsShader(Material material)
        {
            if (material?.shader is Shader shader && shader)
            {
                return shader.name.Equals("Custom/Vehicles/Vehicle/Rotors");
            }

            return false;
        }
    }
}
