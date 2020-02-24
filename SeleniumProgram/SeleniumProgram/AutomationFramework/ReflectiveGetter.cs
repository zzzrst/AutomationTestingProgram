// <copyright file="ReflectiveGetter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class to get test action using reflection.
    /// </summary>
    public class ReflectiveGetter
    {
        /// <summary>
        /// Gets a list of all subclasses of base class T using reflection.
        /// </summary>
        /// <typeparam name="T">The generic type T to be used.</typeparam>
        /// <param name="constructorArgs">The constructorArgs<see cref="T:object[]"/>.</param>
        /// <returns><see cref="List{T}"/> found.</returns>
        public static List<T> GetEnumerableOfType<T>(params object[] constructorArgs)
            where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }

            return objects;
        }
    }
}
