// <copyright file="PrefabManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System;
    using System.Collections.Generic;
    using AlgernonCommons;
    using ColossalFramework.UI;
    using UnityEngine;
    using VisibilityControl.AdditiveShader;

    /// <summary>
    /// Prefab management.
    /// </summary>
    internal static class PrefabManager
    {
        /// <summary>
        /// Scans loaded prefabs for relevant shader information and performs any necessary actions.
        /// </summary>
        internal static void ScanPrefabs()
        {
            // Additive shader assets.
            List<ManagedPrefab> additiveAssets = new List<ManagedPrefab>();

            // Iterate through all loaded prefabs.
            PrefabInfo[] prefabs = Resources.FindObjectsOfTypeAll<PrefabInfo>();
            for (int i = 0; i < prefabs.Length; ++i)
            {
                try
                {
                    PrefabInfo prefab = prefabs[i];
                    if (prefab is PropInfo prop)
                    {
                        // Prop - check for additive shader.
                        if (prop.m_mesh?.name is string meshName && ShaderManager.HasShaderKey(meshName))
                        {
                            additiveAssets.Add(new ManagedPrefab(prop));
                        }
                    }
                    else if (prefab is BuildingInfo building)
                    {
                        // Building - check for additive shader.
                        if (building.m_mesh?.name is string meshName && ShaderManager.HasShaderKey(meshName))
                        {
                            additiveAssets.Add(new ManagedPrefab(building, false));
                        }

                        // Prop LODs don't support the additive shader, so the maximum prop visibility distance needs to be extended instead if the building contains additive shader props.
                        if (building.m_props != null && ShaderManager.ContainsShaderProps(building))
                        {
                            additiveAssets.Add(new ManagedPrefab(building, true));
                        }
                    }
                    else if (prefab is BuildingInfoSub buildingSub)
                    {
                        // Sub-building - check for additive shader.
                        if (buildingSub.m_mesh?.name is string meshName && ShaderManager.HasShaderKey(meshName))
                        {
                            additiveAssets.Add(new ManagedPrefab(buildingSub));
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
        }
    }
}
