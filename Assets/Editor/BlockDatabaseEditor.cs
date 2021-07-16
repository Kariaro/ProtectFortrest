using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditorInternal;
using System;
using UnityEngine.Tilemaps;
using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Blocks;

[CustomEditor(typeof(BlockDatabase), true, isFallback = true)]
public class BlockDatabaseEditor : Editor {

	protected virtual void OnEnable() {
	}

	private ReorderableList BuildTaskReorderableList(List<BlockObject> database, string name) {
		return new ReorderableList(database, typeof(BlockObject), true, true, false, false) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, name);
			},
			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				BlockObject value = database[index];

				float labelWidth_old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 100;
				
				Rect color_rect = new Rect(rect) { width = 68, height = 68 };
				EditorGUI.DrawRect(color_rect, Color.black);
				Rect image_rect = new Rect(rect) { x = rect.x + 2, y = rect.y + 2, width = 64, height = 64 };
				EditorGUI.DrawTextureTransparent(image_rect, PreviewUtility.GetTilePreview(value.GetDefaultTile()));
				rect.x += 70;
				rect.width -= 70;

				Rect id_rect = new Rect(rect) { height = 16 };
				value.blockName = EditorGUI.TextField(id_rect, new GUIContent("Name"), value.blockName);
				rect.y += 16;

				Rect object_rect = new Rect(rect) { height = 16 };
				value.tile = (TileBase)EditorGUI.ObjectField(object_rect, new GUIContent("Tile"), value.tile, typeof(TileBase), false);
				rect.y += 16;

				Rect group_rect = new Rect(rect) { height = 16 };
				value.blockGroup = (IBlockGroup)EditorGUI.EnumPopup(group_rect, new GUIContent("Group"), value.blockGroup);
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
			onSelectCallback = (ReorderableList a) => {

			},
			onAddCallback = (ReorderableList a) => {
				database.Add(BlockObject.CreateInstance<BlockObject>());
			},
		};
	}

	private ReorderableList block_list;
	private Vector2 scrollPosition = Vector2.zero;
	private List<BlockObject> database_list;

	public override void OnInspectorGUI() {
		if(block_list == null) {
			database_list = ((BlockDatabase)target).elements;
			block_list = BuildTaskReorderableList(database_list, "Database");
        }

		serializedObject.Update();

		GUILayout.Space(10);
		GUILayout.BeginHorizontal();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if(GUILayout.Button("Import Blocks")) {
            database_list.Clear();

			foreach(string AssetPath in AssetDatabase.GetAllAssetPaths()) {
				if(!AssetPath.StartsWith("Assets")) continue;

				UnityEngine.Object LoadedAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(BlockObject));
				if(LoadedAsset is BlockObject) {
					database_list.Add(LoadedAsset as BlockObject);
				}
			}
        }

		block_list.DoLayoutList();
		GUILayout.EndScrollView();

		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}