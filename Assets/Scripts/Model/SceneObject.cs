using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour {
	const string PrefabFolder = "/Prefabs/";
	public string PrefabFileName = string.Empty;
	public SceneObjectData ObjectData;

	static Dictionary<string,GameObject> prefabs = new Dictionary<string, GameObject>();
	public GameObject LinkedObject;
	public LineRenderer LineRenderer;


	public static void InitializePrefabLibrary(Dictionary<string,GameObject> source){
		if (Application.isEditor) {
			Debug.Log ("Initializing Prefab Library");
		}
		prefabs = source;
	}

	public static GameObject Create(SceneObjectData data, Transform parent){
		GameObject prefab;//			Resources.Load<GameObject>(PrefabFolder + PrefabFileName);
//		if (prefab) {
		Debug.Log("Creating");
		Debug.Log (prefabs.Count);
		if (prefabs.TryGetValue (data.PrefabFileName, out prefab)) {

			GameObject go = Instantiate (prefab);
			go.transform.position = data.transform.GetPosition();
			go.transform.eulerAngles = data.transform.GetRotation();
			go.transform.localScale = data.transform.GetScale();
			go.transform.SetParent (parent);

			SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
			if (sr) {
				sr.size = data.size.GetVector();
			}

			SceneObject so = go.GetComponent<SceneObject> ();
			if (so) {
				so.PrefabFileName = data.PrefabFileName;
			}

			go.SetActive (data.active);
			return go;
		}
		Debug.Log ("Not Found: " + data.PrefabFileName);
		return null;
	}

	public SceneObjectData GetData(){
		return new SceneObjectData (this);
	}
}
