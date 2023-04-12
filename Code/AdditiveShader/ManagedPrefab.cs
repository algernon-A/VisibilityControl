// <copyright file="ManagedPrefab.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard), Quistar (Simon Ueng), aubergine10, Ronyx69. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.AdditiveShader
{
    using System;
    using AlgernonCommons;
    using UnityEngine;

    /// <summary>
    /// Asset record for prefabs using the additive shader.
    /// </summary>
    public readonly struct ManagedPrefab
    {
        /// <summary>
        /// Expanded visibility distance used for props using the additive shader.
        /// Workaround for LODs not being able to use the shader.
        /// </summary>
        internal const float ExpandedPropVisibilityDistance = 25000f;

        // Shader property names.
        private const string ShaderFadeProperty = "_InvFade";
        private const string ShaderIntensityProperty = "_Intensity";

        /// <summary>
        /// Mesh name used for container assets.
        /// </summary>
        private const string ContainerName = "AdditiveShader Container 0 0 container-building";

        // Original prefab data.
        private readonly PrefabInfo _prefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPrefab"/> struct for a <see cref="PropInfo"/> prefab.
        /// </summary>
        /// <param name="prop">Prop prefab using the additive shader.</param>
        public ManagedPrefab(PropInfo prop)
        {
            Logging.Message("creating additive shader for prop ", prop.name);

            // Set basic data.
            TypeOfAsset = AssetType.Prop;
            _prefab = prop;
            IsContainer = false;
            CachedRenderDistance = RenderDistance(prop.m_generatedInfo.m_size);
            Profile = new ShaderData(prop.m_mesh.name);

            // Set shader prefab values.
            prop.m_lodHasDifferentShader = false;
            prop.m_material.SetFloat(ShaderFadeProperty, Profile.m_fade);
            prop.m_lodRenderDistance = Mathf.Max(prop.m_lodRenderDistance, CachedRenderDistance);
            prop.m_maxRenderDistance = Mathf.Max(prop.m_maxRenderDistance, CachedRenderDistance);

            // Set initial additive shader visibility.
            SetVisibility(Profile.IsAlwaysOn);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPrefab"/> struct for a <see cref="BuildingInfo"/> prefab.
        /// </summary>
        /// <param name="building">Building prefab using the additive shader.</param>
        /// <param name="isContainer"><c>true</c> if this is a container asset (doesn't use the shader itself, but contains components that do), <c>false</c> otherwise.</param>
        public ManagedPrefab(BuildingInfo building, bool isContainer)
        {
            Logging.Message("creating additive shader for building ", building.name);

            // Set basic data.
            _prefab = building;
            IsContainer = isContainer;
            if (isContainer)
            {
                // Container buildings (not using the shader in themselves, only in child components).
                TypeOfAsset = AssetType.Container;
                CachedRenderDistance = ExpandedPropVisibilityDistance;
                Profile = new ShaderData(ContainerName);

                // Set shader prefab values.
                building.m_maxPropDistance = Mathf.Max(building.m_maxPropDistance, CachedRenderDistance);
            }
            else
            {
                // Buildings directly using the shader.
                TypeOfAsset = AssetType.Building;
                CachedRenderDistance = RenderDistance(building.m_generatedInfo.m_max);
                Profile = new ShaderData(building.m_mesh.name);

                // Set shader prefab values.
                building.m_lodHasDifferentShader = false;
                building.m_lodMissing = true;
                building.m_material.SetFloat(ShaderFadeProperty, Profile.m_fade);
                building.m_mesh.colors = CreateColorArray(building.m_mesh.vertices.Length);
                building.m_maxLodDistance = Mathf.Max(building.m_maxLodDistance, CachedRenderDistance);
                building.m_minLodDistance = Mathf.Max(building.m_minLodDistance, CachedRenderDistance);

                // Set initial additive shader visibility.
                SetVisibility(Profile.IsAlwaysOn);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPrefab"/> struct for a <see cref="BuildingInfoSub"/> prefab.
        /// </summary>
        /// <param name="subBuilding">Sub-building prefab using the additive shader.</param>
        public ManagedPrefab(BuildingInfoSub subBuilding)
        {
            Logging.Message("creating additive shader for sub-building ", subBuilding.name);

            // Set basic data.
            TypeOfAsset = AssetType.SubBuilding;
            _prefab = subBuilding;
            IsContainer = false;
            CachedRenderDistance = RenderDistance(subBuilding.m_generatedInfo.m_max);
            Profile = new ShaderData(subBuilding.m_mesh.name);

            // Set shader prefab values.
            subBuilding.m_lodHasDifferentShader = false;
            subBuilding.m_material.SetFloat(ShaderFadeProperty, Profile.m_fade);
            subBuilding.m_mesh.colors = CreateColorArray(subBuilding.m_mesh.vertices.Length);
            subBuilding.m_maxLodDistance = Mathf.Max(subBuilding.m_maxLodDistance, CachedRenderDistance);
            subBuilding.m_minLodDistance = Mathf.Max(subBuilding.m_minLodDistance, CachedRenderDistance);

            // Set initial additive shader visibility.
            SetVisibility(Profile.IsAlwaysOn);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPrefab"/> struct for a <see cref="VehicleInfoSub"/> prefab.
        /// </summary>
        /// <param name="subVehicle">Vehicle prefab using the additive shader.</param>
        public ManagedPrefab(VehicleInfoSub subVehicle)
        {
            Logging.Message("creating additive shader for sub-vehicle ", subVehicle.name);

            // Set basic data.
            TypeOfAsset = AssetType.Vehicle;
            _prefab = subVehicle;
            IsContainer = false;
            CachedRenderDistance = RenderDistance(subVehicle.m_generatedInfo.m_size);

            try
            {
                Profile = new ShaderData(subVehicle.m_mesh.name);
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating additive shader profile for vehicle ", subVehicle.name, " for mesh ", subVehicle.m_mesh.name);

                // Use static profile by default.
                Profile = new ShaderData(ShaderData.Profiles.Static);
            }

            // Set shader prefab values.
            subVehicle.m_material.SetFloat(ShaderFadeProperty, Profile.m_fade);
            subVehicle.m_mesh.colors = CreateColorArray(subVehicle.m_mesh.vertices.Length);
            subVehicle.m_lodRenderDistance = Mathf.Max(subVehicle.m_lodRenderDistance, CachedRenderDistance);
            subVehicle.m_maxRenderDistance = Mathf.Max(subVehicle.m_maxRenderDistance, CachedRenderDistance);

            // Set initial additive shader visibility.
            SetVisibility(Profile.IsAlwaysOn);
        }

        /// <summary>
        /// Asset type enum.
        /// </summary>
        public enum AssetType : int
        {
            /// <summary>
            /// Null type.
            /// </summary>
            None,

            /// <summary>
            /// Prop.
            /// </summary>
            Prop,

            /// <summary>
            /// Building.
            /// </summary>
            Building,

            /// <summary>
            /// Sub-building.
            /// </summary>
            SubBuilding,

            /// <summary>
            /// Vehicle.
            /// </summary>
            Vehicle,

            /// <summary>
            /// Container asset (contains other assets).
            /// </summary>
            Container,
        }

        /// <summary>
        /// Gets the shader profile for this asset.
        /// </summary>
        public ShaderData Profile { get; }

        /// <summary>
        /// Gets a value indicating whether this asset is just a container for another shader-using asset.
        /// </summary>
        public bool IsContainer { get; }

        /// <summary>
        /// Gets a cached render distance for this asset.
        /// </summary>
        public float CachedRenderDistance { get; }

        /// <summary>
        /// <para>Gets the asset type for this instance.</para>
        /// </summary>
        public AssetType TypeOfAsset { get; }

        /// <summary>
        /// Show or hide the additive shader for this asset based on game world time.
        /// </summary>
        /// <param name="time">The game time of day.</param>
        public void SetVisiblityByTime(float time)
        {
            if (Profile is ShaderData profile)
            {
                // Set visibility according to 'overlaps midnight' status.
                if (profile.OverlapsMidnight)
                {
                    // Active period overlaps midnight.
                    SetVisibility(time < profile.OffTime || profile.OnTime <= time);
                }
                else
                {
                    // Active period doesn't overlap midnight.
                    SetVisibility(profile.OnTime <= time && time < profile.OffTime);
                }
            }
            else
            {
                // No eligible shader profile found; set visibility to false.
                SetVisibility(false);
            }
        }

        /// <summary>
        /// Show or hide the additive shader for this asset based on daytime or nighttime status.
        /// </summary>
        /// <param name="currentlyNightTime">True if the game is currently in nighttime, false otherwise.</param>
        public void SetVisibliltyByTwilight(bool currentlyNightTime) => SetVisibility(currentlyNightTime == Profile.IsNightTimeOnly);

        /// <summary>
        /// Show or hide the additive shader for this asset.
        /// </summary>
        /// <param name="visible">Shader visibility status to set.</param>
        public void SetVisibility(bool visible)
        {
            switch (TypeOfAsset)
            {
                case AssetType.Prop:
                    PropInfo propInfo = _prefab as PropInfo;
                    propInfo.m_lodRenderDistance = propInfo.m_maxRenderDistance = CachedRenderDistance;
                    propInfo.m_material.SetFloat(ShaderIntensityProperty, visible ? Profile.m_intensity : 0f);
                    break;

                case AssetType.Building:
                    BuildingInfo building = _prefab as BuildingInfo;
                    building.m_maxLodDistance = building.m_minLodDistance = CachedRenderDistance;
                    building.m_material.SetFloat(ShaderIntensityProperty, visible ? Profile.m_intensity : 0f);
                    break;

                case AssetType.SubBuilding:
                    BuildingInfoSub subBuilding = _prefab as BuildingInfoSub;
                    subBuilding.m_maxLodDistance = subBuilding.m_minLodDistance = CachedRenderDistance;
                    subBuilding.m_material.SetFloat(ShaderIntensityProperty, visible ? Profile.m_intensity : 0f);
                    break;

                case AssetType.Vehicle:
                    (_prefab as VehicleInfoSub).m_material.SetFloat(ShaderIntensityProperty, visible ? Profile.m_intensity : 0f);
                    break;
            }
        }

        /// <summary>
        /// Calculate an increased visibility distance based on the given size, to compensate for the additive shader not working on LODs.
        /// <param name="size">Asset mesh size.</param>
        /// </summary>
        /// <returns>Calulated render didstance.</returns>
        private static float RenderDistance(Vector3 size) => (size.x + 30f) * (size.y + 30f) * (size.z + 30f) * 0.1f;

        /// <summary>
        /// Creates a new initialized color array.
        /// </summary>
        /// <param name="size">Array size to create.</param>
        /// <returns>New initialized color array of the given size.</returns>
        private Color[] CreateColorArray(int size)
        {
            // Create array.
            Color[] colors = new Color[size];

            // Initialize array values to white.
            for (int i = 0; i < size; ++i)
            {
                colors[i] = Color.white;
            }

            return colors;
        }
    }
}
