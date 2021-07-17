using ProjectFortrest.Game.Blocks;
using ProjectFortrest.Game.Level;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(BlockMap))]
public class BlockMapPropertyDrawer : PropertyDrawer {
	private BlockMap blockMap;
	private bool _Foldout;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		CheckInitalize(property, label);

		if(_Foldout)
			return (blockMap.Count + 1) * 17f + 2;

		return 19f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		CheckInitalize(property, label);

		_Foldout = EditorGUI.Foldout(new Rect(position) { height = 16 }, _Foldout, label.text);
		position.y += 17;

		if(_Foldout) {
			IEnumerator<Vector3Int> _keys = blockMap.Keys.GetEnumerator();
			IEnumerator<BlockMapEntry> _values = blockMap.Values.GetEnumerator();

			EditorGUI.BeginDisabledGroup(true);

			for(int i = 0; i < blockMap.Count; i++) {
				if(!_keys.MoveNext() || !_values.MoveNext()) break;
				var pos = _keys.Current;
				var ent = _values.Current;
				if(ent == null || ent.block == null) continue;

				Rect ent_rect = new Rect(position) { x = position.x, y = position.y + i * 17, width = 120, height = 15 };
				if(ent.block?.hasStates ?? false) {
					EditorGUI.TextField(ent_rect, GUIContent.none, ent.block.blockName + ":" + ent.state);
				} else {
					EditorGUI.TextField(ent_rect, GUIContent.none, ent.block.blockName);
				}

				Rect pos_rect = new Rect(position) { x = position.x + 120, y = position.y + i * 17, width = position.width - 120, height = 15 };
				EditorGUI.Vector3IntField(pos_rect, GUIContent.none, pos);
			}

			EditorGUI.EndDisabledGroup();
		}
	}

	private void CheckInitalize(SerializedProperty property, GUIContent label) {
		if(blockMap == null) {
			var target = property.serializedObject.targetObject;
			blockMap = fieldInfo.GetValue(target) as BlockMap;
			if(blockMap == null) {
				blockMap = new BlockMap();
				fieldInfo.SetValue(target, blockMap);
			}

			_Foldout = EditorPrefs.GetBool(label.text);
		}
	}
}
