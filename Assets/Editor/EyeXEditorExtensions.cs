//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Custom property drawer for bit mask attributes
/// </summary>
[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class EnumBitMaskPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        var typeAttr = attribute as BitMaskAttribute;
        prop.intValue = DrawBitMaskField(position, prop.intValue, typeAttr.propertyType, label);
    }

    public static int DrawBitMaskField(Rect position, int value, Type type, GUIContent label)
    {
        var itemNames = Enum.GetNames(type);
        var itemValues = Enum.GetValues(type) as int[];

        int currentMask = 0;
        for (int i = 0; i < itemValues.Length; i++)
        {
            if (itemValues[i] != 0)
            {
                if ((value & itemValues[i]) == itemValues[i])
                {
                    currentMask |= 1 << i;
                }
            }
            else if (value == 0)
            {
                currentMask |= 1 << i;
            }
        }

        int newMask = EditorGUI.MaskField(position, label, currentMask, itemNames);

        int newValue = value;
        int changes = currentMask ^ newMask;
        for (int i = 0; i < itemValues.Length; i++)
        {
            if ((changes & (1 << i)) != 0)
            {
                if ((newMask & (1 << i)) != 0)
                {
                    if (itemValues[i] == 0)
                    {
                        newValue = 0;
                        break;
                    }
                    else
                    {
                        newValue |= itemValues[i];
                    }
                }
                else
                {
                    newValue &= ~itemValues[i];
                }
            }
        }

        return newValue;
    }
}
