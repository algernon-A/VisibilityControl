// <copyright file="LodDistanceRecord.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System.Xml.Serialization;
    using static VisibilityControl.Patches.LodDistanceBuildings;
    using static VisibilityControl.Patches.LodDistanceNets;
    using static VisibilityControl.Patches.LodDistanceProps;
    using static VisibilityControl.Patches.LodDistanceTrees;
    using static VisibilityControl.Patches.LodDistanceVehicles;

    /// <summary>
    /// Class to handle ULOD settings.
    /// </summary>
    public class LodDistanceRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LodDistanceRecord"/> class.
        /// </summary>
        public LodDistanceRecord()
        {
        }

        /// <summary>
        /// Gets or sets the minimum building LOD visibility distance.
        /// </summary>
        [XmlElement("BuildingMinDistance")]
        public float XMLBuildingMinDistance { get => BuildingMinDistance; set => BuildingMinDistance = value; }

        /// <summary>
        /// Gets or sets the building LOD distance multiplier.
        /// </summary>
        [XmlElement("BuildingMultiplier")]
        public float XMLBuildingMultiplier { get => BuildingMultiplier; set => BuildingMultiplier = value; }

        /// <summary>
        /// Gets or sets the minimum network LOD visibility distance.
        /// </summary>
        [XmlElement("NetworkMinDistance")]
        public float XMLNetMinDistance { get => NetMinDistance; set => NetMinDistance = value; }

        /// <summary>
        /// Gets or sets the network LOD distance multiplier.
        /// </summary>
        [XmlElement("NetworkMultiplier")]
        public float XMLNetMultiplier { get => NetMultiplier; set => NetMultiplier = value; }

        /// <summary>
        /// Gets or sets the minimum vehicle LOD visibility distance.
        /// </summary>
        [XmlElement("VehicleMinDistance")]
        public float XMLVehicleMinDistance { get => VehicleMinDistance; set => VehicleMinDistance = value; }

        /// <summary>
        /// Gets or sets the vehicle LOD distance multiplier.
        /// </summary>
        [XmlElement("VehicleMultiplier")]
        public float XMLVehicleMultiplier { get => VehicleMultiplier; set => VehicleMultiplier = value; }

        /// <summary>
        /// Gets or sets the minimum prop LOD visibility distance.
        /// </summary>
        [XmlElement("PropMinDistance")]
        public float XMLPropMinDistance { get => PropMinDistance; set => PropMinDistance = value; }

        /// <summary>
        /// Gets or sets the prop LOD distance multiplier.
        /// </summary>
        [XmlElement("PropMultiplier")]
        public float XMLPropMultiplier { get => PropMultiplier; set => PropMultiplier = value; }

        /// <summary>
        /// Gets or sets the tree LOD visibility distance.
        /// </summary>
        [XmlElement("TreeLodDistance")]
        public float XMLTreeLodDistance { get => TreeLodDistance; set => TreeLodDistance = value; }
    }
}
