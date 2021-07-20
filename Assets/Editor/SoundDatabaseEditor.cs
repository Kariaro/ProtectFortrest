using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Items;
using ProjectFortrest.Game.Sound;

[CustomEditor(typeof(SoundDatabase), true, isFallback = true)]
public class SoundDatabaseEditor : Editor {

	protected virtual void OnEnable() {
		
	}

	private ReorderableList BuildReorderableList(List<SoundObject> database, string name) {
		return new ReorderableList(database, typeof(SoundObject), true, true, false, false) {
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, name);
			},
			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				SoundObject value = database[index];

				float labelWidth_old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 100;

				Rect id_rect = new Rect(rect) { height = 16 };
				value.soundName = EditorGUI.TextField(id_rect, new GUIContent("Name"), value.soundName);
				rect.y += 16;

				Rect clip_rect = new Rect(rect) { height = 16 };
				value.clip = (AudioClip)EditorGUI.ObjectField(clip_rect, new GUIContent("Clip"), value.clip, typeof(AudioClip), false);
				rect.y += 16;

				Rect button_rect = new Rect(rect) { height = 18, width = 140 };
				rect.y += 16;
				if(GUI.Button(button_rect, "Open in Inspector")) {
					Selection.activeObject = value;
				}

				EditorGUIUtility.labelWidth = labelWidth_old;
			},
			elementHeightCallback = (int index) => {
				return 48;
			},
			drawNoneElementCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Empty", EditorStyles.centeredGreyMiniLabel);
			},
			onAddCallback = (ReorderableList a) => {
				database.Add(SoundObject.CreateInstance<SoundObject>());
			},
		};
	}
	
	private Vector2 scrollPosition = Vector2.zero;
	private List<SoundObject> database_list;
	private ReorderableList sound_list;

	public override void OnInspectorGUI() {
		if(sound_list == null) {
			database_list = ((SoundDatabase)target).elements;
			sound_list = BuildReorderableList(database_list, "Database");
		}

		serializedObject.Update();

		GUILayout.Space(10);
		GUILayout.BeginHorizontal();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if(GUILayout.Button("Import Sounds")) {
			database_list.Clear();

			foreach(string AssetPath in AssetDatabase.GetAllAssetPaths()) {
				if(!AssetPath.StartsWith("Assets")) continue;

				UnityEngine.Object LoadedAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(SoundObject));
				if(LoadedAsset is SoundObject) {
					database_list.Add(LoadedAsset as SoundObject);
				}
			}

			SerializedProperty elements_prop = serializedObject.FindProperty("elements");
			if(elements_prop.arraySize != database_list.Count) {
				elements_prop.arraySize = database_list.Count;
			}

			for(int i = 0; i < database_list.Count; i++) {
				SerializedProperty a = elements_prop.GetArrayElementAtIndex(i);
				a.objectReferenceValue = database_list[i];
			}
		}

		sound_list.DoLayoutList();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}