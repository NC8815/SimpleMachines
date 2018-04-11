using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
	public List<SceneObjectData> sceneObjects;
	public Vector2 gravity = Vector2.zero;

	public void Initialize(List<SceneObjectData> objects){
		this.sceneObjects = objects;
	}
	public void Initialize(LevelData source){
		this.sceneObjects = source.sceneObjects;
	}
	public void Initialize(List<SceneObject> objects){
		this.sceneObjects = objects.Select (so => so.GetData ()).ToList ();
		for (int i = 0; i < sceneObjects.Count; i++) {
			sceneObjects [i].linkIndex = (objects [i].LinkedObject != null) ? objects.Select(so=>so.gameObject).ToList().IndexOf (objects [i].LinkedObject) : -1;
			Debug.Log (sceneObjects [i].linkIndex);
		}
	}
}
