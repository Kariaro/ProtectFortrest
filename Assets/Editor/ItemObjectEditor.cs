using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Blocks;
using System.Linq;
using ProjectFortrest.Game.Blocks.Impl;

[CustomEditor(typeof(ItemObject), true, isFallback = true)]
public class ItemObjectEditor : Editor {
	protected virtual void OnEnable() {
		states_list = BuildReorderableList(serializedObject.FindProperty("states"), "State List");
	}

	private ReorderableList states_list;
	public override void OnInspectorGUI() {
		serializedObject.Update();
		
		SerializedProperty sprite_prop = serializedObject.FindProperty("sprite");
		SerializedProperty prefab_prop = serializedObject.FindProperty("prefab");
		SerializedProperty itemName_prop = serializedObject.FindProperty("itemName");
		SerializedProperty itemGroup_prop = serializedObject.FindProperty("itemGroup");
		SerializedProperty interactable_prop = serializedObject.FindProperty("interactable");
		SerializedProperty hasStates_prop = serializedObject.FindProperty("hasStates");
		
		int old = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		GUILayout.BeginVertical();

		Rect itemName_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		itemName_prop.stringValue = EditorGUI.TextField(itemName_rect, "Item Name", itemName_prop.stringValue);
		GUILayout.Space(1);
		
		Rect itemGroup_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		int itemGroup_index = EditorGUI.Popup(itemGroup_rect, "Item Group", itemGroup_prop.enumValueIndex, itemGroup_prop.enumDisplayNames);
		itemGroup_prop.enumValueIndex = itemGroup_index;
		GUILayout.Space(1);

		EditorGUI.BeginDisabledGroup(hasStates_prop.boolValue);
		Rect sprite_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		EditorGUI.ObjectField(sprite_rect, sprite_prop, new GUIContent("Sprite"));
		EditorGUI.EndDisabledGroup();
		GUILayout.Space(1);

		
		Rect prefab_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		EditorGUI.ObjectField(prefab_rect, prefab_prop, new GUIContent("Prefab"));
		GUILayout.Space(1);

		Rect interactable_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		interactable_prop.boolValue = EditorGUI.Toggle(interactable_rect, "Interactable", interactable_prop.boolValue);
		GUILayout.Space(1);

		Rect hasStates_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
		hasStates_prop.boolValue = EditorGUI.Toggle(hasStates_rect, "Has States", hasStates_prop.boolValue);
		GUILayout.Space(1);

		EditorGUI.BeginDisabledGroup(!hasStates_prop.boolValue);
		if(true) {
			SerializedProperty defaultState_prop = serializedObject.FindProperty("defaultState");
			SerializedProperty states_prop = serializedObject.FindProperty("states");

			// If has states draw states
			GUILayout.BeginVertical(GUI.skin.box);

			string defaultState_value = defaultState_prop.stringValue;
			int defaultState_currentIndex = 0;

			string[] defaultState_values = new string[states_prop.arraySize];
			for(int i = 0; i < states_prop.arraySize; i++) {
				string value = states_prop.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
				if(value == defaultState_value) {
					defaultState_currentIndex = i;
                }
				defaultState_values[i] = value;
            }
			
			Rect defaultState_rect = GUILayoutUtility.GetRect(64, EditorGUIUtility.singleLineHeight);
			if(defaultState_values.Length == 0) {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Popup(defaultState_rect, "Default State", defaultState_currentIndex, defaultState_values);
				EditorGUI.EndDisabledGroup();
			} else {
				int defaultState_index = EditorGUI.Popup(defaultState_rect, "Default State", defaultState_currentIndex, defaultState_values);
				defaultState_prop.stringValue = defaultState_values[defaultState_index];
            }
			
			GUILayout.Space(4);
			states_list.DoLayoutList();

			GUILayout.EndVertical();
        }
		EditorGUI.EndDisabledGroup();

		GUILayout.EndVertical();
		
		EditorGUI.indentLevel = old;
		serializedObject.ApplyModifiedProperties();
	}

	private ReorderableList BuildReorderableList(SerializedProperty property, string list_name) {
		return new ReorderableList(property.serializedObject, property, true, true, true, true) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, list_name);
			},
			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				SerializedProperty prop = property.GetArrayElementAtIndex(index);
				SerializedProperty name_prop = prop.FindPropertyRelative("name");
				SerializedProperty sprite_prop = prop.FindPropertyRelative("sprite");

				rect.height = 16;
				string newName_value = EditorGUI.TextField(rect, new GUIContent("Name"), name_prop.stringValue);
				name_prop.stringValue = EnsureUniqueName(newName_value, index, property);
				rect.y += 17;
				EditorGUI.ObjectField(rect, sprite_prop, new GUIContent("Sprite"));
			},
			elementHeightCallback = (int index) => {
				return 48;
			},
			drawNoneElementCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Add a new state", EditorStyles.centeredGreyMiniLabel);
			},
			onAddCallback = (ReorderableList list) => {
				property.arraySize++;
				int index = property.arraySize - 1;
				SerializedProperty prop = property.GetArrayElementAtIndex(index);
				SerializedProperty name_prop = prop.FindPropertyRelative("name");
				name_prop.stringValue = EnsureUniqueName("State", index, property);
            }
		};
	}

	public string EnsureUniqueName(string newName, int index, SerializedProperty prop) {
		string modified = newName;
		int collisions = 0;
		for(int i = 0, size = prop.arraySize; i < size; i++) {
			if(i == index) continue;

			string value = prop.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
			if(value == modified) {
				modified = newName + "_" + (collisions++);
				i = 0;
				continue;
            }
        }

		return modified;
    }
}
