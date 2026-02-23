using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BlackHole.Editor
{
    [CustomPropertyDrawer(typeof(ClassDropdownAttribute))]
    public class ClassDropdownDrawer : PropertyDrawer
    {
        private bool didInitialize;
        private static List<Type> types;
        private int selectionIndex;
        private string[] selections;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Do a one-time setup if we haven't yet
            if (!didInitialize)
            {
                Initialize(property);
            }

            // Make sure the selection index is always in sync with the serialized property
            UpdateSelectionIndex(property);

            // Draw the dropdown (Popup) showing all possible subclasses
            Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            selectionIndex = EditorGUI.Popup(dropdownRect, property.displayName, selectionIndex, selections);
            if (EditorGUI.EndChangeCheck())
            {
                // If the user picks a new subclass, create an instance of it
                property.managedReferenceValue = Activator.CreateInstance(types[selectionIndex]);
                property.serializedObject.ApplyModifiedProperties();
            }

            // Draw the selected subclass's serialized fields below the dropdown
            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;

                // Starting position for child fields
                float yOffset = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // We'll iterate over the child properties
                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();

                bool enterChildren = true;
                while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
                {
                    // Skip the default 'm_Script' reference (if it appears)
                    if (copy.name.Equals("m_Script", StringComparison.Ordinal))
                    {
                        enterChildren = false;
                        continue;
                    }

                    // Calculate height for this child property
                    float childHeight = EditorGUI.GetPropertyHeight(copy, true);
                    Rect childRect = new Rect(position.x, yOffset, position.width, childHeight);

                    // Draw the child field
                    EditorGUI.PropertyField(childRect, copy, true);
                    
                    // Move down for the next property
                    yOffset += childHeight + EditorGUIUtility.standardVerticalSpacing;
                    enterChildren = false;
                }

                EditorGUI.indentLevel--;
            }
        }

        // We must give Unity the total height needed (dropdown + child fields).
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Base height for the dropdown
            float height = EditorGUIUtility.singleLineHeight;

            // If there's a selected type, add heights for its child fields
            if (property.managedReferenceValue != null)
            {
                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();

                bool enterChildren = true;
                while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
                {
                    if (copy.name.Equals("m_Script", StringComparison.Ordinal))
                    {
                        enterChildren = false;
                        continue;
                    }

                    height += EditorGUI.GetPropertyHeight(copy, true) + EditorGUIUtility.standardVerticalSpacing;
                    enterChildren = false;
                }
            }

            return height;
        }
        
        private void Initialize(SerializedProperty property)
        {
            // Extract the base type from the attribute
            Type baseType = (attribute as ClassDropdownAttribute)?.BaseType;

            // Prepare a temporary list of display names
            List<string> selectionsList = new List<string>();

            // Initialize the global list of valid subclasses
            types = new List<Type>();

            // Loop through all assemblies, then all types in each assembly
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // We only want types that inherit from 'baseType' and are concrete (not abstract)
                    if (!type.IsSubclassOf(baseType) || type.IsAbstract)
                    {
                        continue;
                    }
                    
                    Debug.Log("Hello from ClassDropdownDrawer! Found type: " + type.FullName);

                    // Store the type and its display name
                    types.Add(type);
                    string displayName = type.Name;
                    selectionsList.Add(displayName);
                }
            }

            // Convert the accumulated list of names into a string array
            selections = selectionsList.ToArray();

            // Sync the current selection index with the existing value in the property
            UpdateSelectionIndex(property);

            // Mark that we've completed our initialization pass
            didInitialize = true;
        }
        
        private void UpdateSelectionIndex(SerializedProperty property)
        {
            // The currently assigned object (subclass instance) in the property
            object value = property.managedReferenceValue;

            if (value == null)
            {
                // If there is no object, pick the first type by default
                selectionIndex = 0;
                property.managedReferenceValue = Activator.CreateInstance(types[selectionIndex]);
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            // Otherwise, find which type the property is currently assigned to
            for (int i = 0; i < types.Count; i++)
            {
                if (value.GetType() == types[i])
                {
                    selectionIndex = i;
                    break;
                }
            }
        }
    }
}