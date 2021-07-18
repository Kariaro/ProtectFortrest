using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Items;

[CustomEditor(typeof(ItemDatabase), true, isFallback = true)]
public class ItemDatabaseEditor : Editor {

	protected virtual void OnEnable() {
	}

	private ReorderableList BuildTaskReorderableList(List<ItemObject> database, string name) {
		return new ReorderableList(database, typeof(ItemObject), true, true, false, false) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, name);
			},
			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				ItemObject value = database[index];

				float labelWidth_old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 100;
				
				Rect color_rect = new Rect(rect) { width = 68, height = 68 };
				EditorGUI.DrawRect(color_rect, Color.black);
				Rect image_rect = new Rect(rect) { x = rect.x + 2, y = rect.y + 2, width = 64, height = 64 };
				EditorGUI.DrawTextureTransparent(image_rect, PreviewUtility.GetSpritePreview(value.states[0].sprite));
				rect.x += 70;
				rect.width -= 70;

				Rect id_rect = new Rect(rect) { height = 16 };
				value.itemName = EditorGUI.TextField(id_rect, new GUIContent("Name"), value.itemName);
				rect.y += 16;

				Rect object_rect = new Rect(rect) { height = 16 };
				value.states[0].sprite = (Sprite)EditorGUI.ObjectField(object_rect, new GUIContent("Sprite"), value.states[0].sprite, typeof(Sprite), false);
				rect.y += 16;

				Rect group_rect = new Rect(rect) { height = 16 };
				value.itemGroup = (IItemGroup)EditorGUI.EnumPopup(group_rect, new GUIContent("Group"), value.itemGroup);
				rect.y += 18;				

				Rect button_rect = new Rect(rect) { height = 18, width = 140 };
				rect.y += 16;
				if(GUI.Button(button_rect, "Open in Inspector")) {
					Selection.activeObject = value;
				}
				EditorGUIUtility.labelWidth = labelWidth_old;
			},
			elementHeightCallback = (int index) => {
				return 16*4 + 16;
			},
			drawNoneElementCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Empty", EditorStyles.centeredGreyMiniLabel);
			},
			onAddCallback = (ReorderableList a) => {
				database.Add(ItemObject.CreateInstance<ItemObject>());
			},
		};
	}

	private ReorderableList item_list;
	private Vector2 scrollPosition = Vector2.zero;
	private List<ItemObject> database_list;

	public override void OnInspectorGUI() {
		if(item_list == null) {
			database_list = ((ItemDatabase)target).elements;
			item_list = BuildTaskReorderableList(database_list, "Database");
		}

		serializedObject.Update();

		GUILayout.Space(10);
		GUILayout.BeginHorizontal();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if(GUILayout.Button("Import Items")) {
			database_list.Clear();

			foreach(string AssetPath in AssetDatabase.GetAllAssetPaths()) {
				if(!AssetPath.StartsWith("Assets")) continue;

				UnityEngine.Object LoadedAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(ItemObject));
				if(LoadedAsset is ItemObject) {
					database_list.Add(LoadedAsset as ItemObject);
				}
			}
		}

		item_list.DoLayoutList();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}