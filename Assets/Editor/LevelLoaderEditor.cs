using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoader))]
public class LevelLoaderEditor : Editor {

	protected string GetRelativePath(string fullPath){
		if (fullPath.StartsWith (Application.dataPath)) {
			return "Assets" + fullPath.Substring (Application.dataPath.Length);
		} else {
			Debug.Log ("Invalid fullPath. String does not contain dataPath.");
			return fullPath;
		}
	}

	public override void OnInspectorGUI ()
	{
		var current = target as LevelLoader;
		base.OnInspectorGUI ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Current Level:");
		if (GUILayout.Button(current.data.name)){
			Selection.activeObject = current.data;
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button ("Save")) {
			current.InitializePrefabLibrary ();
			current.SaveData ();
			string path = EditorUtility.SaveFilePanel (
				              "Save Level Data",
				              current.Directory,
				              current.FileName,
				              "asset");
			path = GetRelativePath (path);
			AssetDatabase.CreateAsset (current.data, path);
			AssetDatabase.SaveAssets ();
		}

		if (GUILayout.Button ("Load")) {
			current.InitializePrefabLibrary ();
			string path = EditorUtility.OpenFilePanel (
				              "Choose Level Data",
				              current.Directory,
				              "asset");
			path = GetRelativePath (path);
			LevelData data = AssetDatabase.LoadAssetAtPath<LevelData> (path);
			if (data) {
				current.data = data;
				current.LoadData ();
			}
		}
	}
}
