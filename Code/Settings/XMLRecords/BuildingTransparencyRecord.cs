// <copyright file="BuildingTransparencyRecord.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System.Xml.Serialization;
    using static VisibilityControl.Patches.TransparentLODFix;

    /// <summary>
    /// Class to handle building transparency visibility settings.
    /// </summary>
    public class BuildingTransparencyRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingTransparencyRecord"/> class.
        /// </summary>
        public BuildingTransparencyRecord()
        {
        }

        /// <summary>
        /// Gets or sets the fallback building render distance.
        /// This is used when no other value can be calculated.
        /// </summary>
        [XmlElement("BuildingFallbackRenderDistance")]
        public float XMLBuildingFallbackRenderDistance { get => BuildingFallbackRenderDistance; set => BuildingFallbackRenderDistance = value; }

        /// <summary>
        /// Gets or sets the building minimum visibility distance.
        /// Buildings will always be visible within this distance.
        /// </summary>
        [XmlElement("BuildingMinimumDistance")]

        public float XMLBuildingMinimumDistance { get => BuildingMinimumDistance; set => BuildingMinimumDistance = value; }

        /// <summary>
        /// Gets or sets the building LOD transition multiplier.
        /// This determines how far away the model will transition from full mesh to LOD.
        /// </summary>
        [XmlElement("BuildingLODTransitionMultiplier")]

        public float XMLBuildingLODTransitionMultiplier { get => BuildingLODTransitionMultiplier; set => BuildingLODTransitionMultiplier = value; }
    }
}
