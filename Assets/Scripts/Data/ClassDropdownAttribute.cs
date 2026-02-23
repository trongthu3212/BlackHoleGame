using System;
using UnityEngine;

namespace BlackHole.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ClassDropdownAttribute : PropertyAttribute
    {
        public Type BaseType { get; }
        
        public ClassDropdownAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}