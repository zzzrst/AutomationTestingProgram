// <copyright file="ResourceHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directives should be placed correctly
using System.Globalization;
using AxeAccessibilityDriver.Properties;
#pragma warning restore SA1200 // Using directives should be placed correctly

namespace AxeAccessibilityDriver
{
    /// <summary>
    /// Helper for string resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Gets the string value for the resource name.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The string value for the resource name.</returns>
        public static string GetString(string resourceName)
        {
            return Resources.ResourceManager.GetString(resourceName, CultureInfo.CurrentCulture);
        }
    }
}
