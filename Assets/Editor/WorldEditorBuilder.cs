using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using ProjectFortrest.Game.Level;
using ProjectFortrest.Game.Database;
using ProjectFortrest.Game.Blocks;

public class WorldEditorBuilder : EditorWindow {

	[MenuItem("World/WorldEditor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(WorldEditorBuilder));
	}
	
	private BlockDatabase _blockDatabase;
	private BlockDatabase BlockDatabase {
		get {
			if(_blockDatabase != null) return _blockDatabase;

			foreach(string AssetPath in AssetDatabase.GetAllAssetPaths()) {
				if(!AssetPath.StartsWith("Assets")) continue;

				UnityEngine.Object LoadedAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(BlockDatabase));
				if(LoadedAsset is BlockDatabase) {
					_blockDatabase = LoadedAsset as BlockDatabase;
					break;
				}
			}

			if(_blockDatabase == null)
				throw new Exception("_blockDatabase was not found in assets");

			return _blockDatabase;
		}
	}

	void OnEnable() {
		
	}
	
	private enum IDrawTool {
		Draw,
		Erase,
		Fill
	}

	private TileLevel tileLevel;
	private IBlockGroup tileLayer;
	private IDrawTool drawTool;
	private int blockIndex;
	private bool drawEnabled;
	private Vector2 scrollPosition = Vector2.zero;
	private Dictionary<string, int> stateIndexes = new Dictionary<string, int>();

	void OnGUI() {
		/* Display the current scene */ {
			Scene scene = EditorSceneManager.GetActiveScene();
			EditorGUILayout.LabelField("Current scene opened: " + scene == null ? "":scene.path, EditorStyles.centeredGreyMiniLabel);
		}

		if(tileLevel == null) {
			tileLevel = GameObject.Find("Level(0)")?.GetComponent<TileLevel>();
		}

		float old_labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 130;

		// TileLevel picker
		tileLevel = (TileLevel)EditorGUILayout.ObjectField("Tile Level", tileLevel, typeof(TileLevel), tileLevel);

		// TileLevel layer
		tileLayer = (IBlockGroup)EditorGUILayout.EnumPopup("Layer", tileLayer);

		GUILayout.BeginHorizontal();
		{
			Rect rect = GUILayoutUtility.GetRect(64, 20);
			rect.x += 3;
			rect.width -= 6;

			float step = rect.width / 3.0f;
			float w = rect.width / 3.0f;

			Rect rect_0 = new Rect(rect) { x = rect.x + step * 0, width = w };
			if(GUI.Toggle(rect_0, drawTool == IDrawTool.Draw, "Draw", EditorStyles.miniButtonLeft)) {
				drawTool = IDrawTool.Draw;

			}

			Rect rect_1 = new Rect(rect) { x = rect.x + step * 1, width = w };
			if(GUI.Toggle(rect_1, drawTool == IDrawTool.Erase, "Erase", EditorStyles.miniButtonMid)) {
				drawTool = IDrawTool.Erase;

			}

			Rect rect_2 = new Rect(rect) { x = rect.x + step * 2, width = w };
			if(GUI.Toggle(rect_2, drawTool == IDrawTool.Fill, "Fill", EditorStyles.miniButtonRight)) {
				drawTool = IDrawTool.Fill;

			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical(GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		EditorGUI.BeginDisabledGroup(tileLevel == null);
		if(tileLevel == null) {
			Rect rect = GUILayoutUtility.GetRect(0, 0);
			DrawBlockPicker();
			rect.width = Screen.width - rect.x;
			rect.height = Screen.height - rect.y;
			GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false, 1, new Color(0, 0, 0, 0.6f), 0, 0);
		} else {
			DrawBlockPicker();
		}
		EditorGUI.EndDisabledGroup();

		GUILayout.EndScrollView();
		GUILayout.EndVertical();

		EditorGUIUtility.labelWidth = old_labelWidth;
	}

	void DrawBlockPicker() {
		int index = 0;
		
		for(int i = 0, len = BlockDatabase.elements.Count; i < len; i++) {
			BlockObject block = BlockDatabase.elements[i];
			if(block == null) {
				BlockDatabase.elements.RemoveAt(i);
				i -= 1;
				len -= 1;
			}
		}
		
		foreach(BlockObject block in BlockDatabase.elements) {
			Rect rect = GUILayoutUtility.GetRect(64 + 4, 64 + 4 + 8);
			
			Rect color_rect = new Rect(rect.x, rect.y, 64 + 4, 64 + 4);
			if(GUI.Button(color_rect, "", GUI.skin.box)) {
				blockIndex = index;
				tileLayer = block.blockGroup;
			}

			Rect label_rect = new Rect(rect.x + 68, rect.y, 60, 32);
			GUI.Label(label_rect, "Name:\nGroup:");

			Rect text_rect = new Rect(rect.x + 68 + 60, rect.y, rect.width, 32);
			GUI.Label(text_rect, $"{block.blockName}\n{block.blockGroup}");
			
			Texture2D texture;
			if(block.HasStates()) {
				int state_index = 0;
				if(stateIndexes.TryGetValue(block.blockName, out int _stateIndex)) {
					state_index = _stateIndex;
				}

				Rect stateLabel_rect = new Rect(rect.x + 68, rect.y + 32, 60, 16);
				GUI.Label(stateLabel_rect, "State:");

				Rect state_rect = new Rect(rect.x + 68 + 60, rect.y + 32, rect.width, 16);
				state_index = EditorGUI.Popup(state_rect, state_index, block.GetStateNames());
				stateIndexes[block.blockName] = state_index;
				texture = PreviewUtility.GetTilePreview(block.states[state_index].tile);
			} else {
				texture = PreviewUtility.GetTilePreview(block.states[0].tile);
			}
			texture.filterMode = FilterMode.Point;

			if(index == blockIndex) {
				EditorGUI.DrawRect(color_rect, Color.white);
			} else {
				EditorGUI.DrawRect(color_rect, Color.black);
			}
			
			Rect image_rect = new Rect(rect.x + 2, rect.y + 2, 64, 64);
			EditorGUI.DrawTextureTransparent(image_rect, texture);

			index++;
		}
	}

	private Texture GetTestPreview() {
		BlockObject block = BlockDatabase.elements[blockIndex];
		int stateIndex = 0;
		if(stateIndexes.TryGetValue(block.blockName, out int _stateIndex)) {
			stateIndex = _stateIndex;
		}

		return PreviewUtility.GetTilePreview(block.GetState(stateIndex).tile);
	}
	
	void OnSceneGUI(SceneView sceneView) {
		if(tileLevel == null) return;

		{
			Handles.BeginGUI();
			Rect rect = new Rect(8, 8, 160, 24);
			drawEnabled = GUI.Toggle(rect, drawEnabled, "Toggle Custom Draw", GUI.skin.button);
			Handles.EndGUI();
		}

		if(drawEnabled) {
			if(Event.current.type == EventType.Layout) {
				HandleUtility.AddDefaultControl(0);
			}

			Event evt = Event.current;

			Vector3 mousePosition = evt.mousePosition;
			mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
			mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
			mousePosition.z = 0;

			int mx = Mathf.FloorToInt(mousePosition.x);
			int my = Mathf.FloorToInt(mousePosition.y);
			Rect selectionBox = new Rect(mx, my, 1, 1);

			{
				Handles.BeginGUI();

				Vector3 a = sceneView.camera.WorldToScreenPoint(new Vector2(mx, my + 1));
				Vector3 b = sceneView.camera.WorldToScreenPoint(new Vector2(mx + 1, my + 2));
				GUI.DrawTexture(new Rect(a.x, sceneView.camera.pixelHeight - a.y, b.x - a.x, (b.y - a.y)), GetTestPreview());
				Handles.EndGUI();
			}
			Handles.color = Color.black;
			Handles.DrawSolidRectangleWithOutline(selectionBox, new Color(0, 0, 0, 0.5f), Color.black);

			switch(drawTool) {
				case IDrawTool.Draw: {
					if((evt.type == EventType.MouseDown
					|| evt.type == EventType.MouseDrag)
					&& evt.button == 0) {
						// Place block at position
						
						BlockObject block = BlockDatabase.elements[blockIndex];
						int stateIndex = 0;
						if(stateIndexes.TryGetValue(block.blockName, out int _stateIndex)) {
							stateIndex = _stateIndex;
						}

						if((evt.modifiers & EventModifiers.Shift) == 0) {
							tileLevel.PutBlock(mx, my, block, stateIndex);
						} else {
							tileLevel.RemoveBlock(mx, my, (int)block.blockGroup);
						}
						evt.Use();
					}
					break;
				}
				case IDrawTool.Erase: {
					if((evt.type == EventType.MouseDown
					|| evt.type == EventType.MouseDrag)
					&& evt.button == 0) {
						tileLevel.RemoveBlock(mx, my, (int)tileLayer);
						evt.Use();
					}
					break;
				}
			}
			

			SceneView.RepaintAll();
		}
	}

	void OnFocus() {
		SceneView.duringSceneGui -= this.OnSceneGUI;
		SceneView.duringSceneGui += this.OnSceneGUI;
	}

	void OnDestroy() {
		SceneView.duringSceneGui -= this.OnSceneGUI;
	}
}