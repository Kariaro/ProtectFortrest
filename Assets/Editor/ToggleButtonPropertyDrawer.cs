using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
public class ToggleButtonPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		ToggleButtonAttribute toggleButton = attribute as ToggleButtonAttribute;

		if(property.propertyType == SerializedPropertyType.Boolean) {
			Vector2 size = GUI.skin.label.CalcSize(new GUIContent(toggleButton.name)) + new Vector2(8, 1);
			property.boolValue = GUI.Toggle(new Rect(position.position, size), property.boolValue, toggleButton.name, GUI.skin.button);
		} else {
			EditorGUI.LabelField(position, label.text, "Use ToggleButton for boolean fields.");
		}
	}
}
