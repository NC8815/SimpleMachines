using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System;

public class LevelLoader : MonoBehaviour,ISubscriber {

	public GameObject background = null;
	public GameObject sceneObjects = null;

	[SerializeField]
	protected List<GameObject> prefabs;

//	[HideInInspector]
	public LevelData data;

	const string _defaultFileName = "File";
	const string _defaultFilePath = "Assets/Resources/Levels";
	public string FileName { get { return _defaultFileName; } }
	public string Directory { get { return _defaultFilePath; } }

	public void Awake(){
		InitializePrefabLibrary ();
		SubscribeEvents ();
	}

	#region ISubscriber implementation

	public void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.OnLevelLoad, SetData);
		SMApplication.Subscribe (SMEventTypes.LoadLevel, LoadData);
		SMApplication.Subscribe (SMEventTypes.LevelLoadComplete, CheckInitiateTutorial);
	}

	public void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.OnLevelLoad, SetData);
		SMApplication.Unsubscribe (SMEventTypes.LoadLevel, LoadData);
		SMApplication.Unsubscribe (SMEventTypes.LevelLoadComplete, CheckInitiateTutorial);

	}

	public void OnDestroy(){
		Debug.Log ("Poof!");
		UnsubscribeEvents ();
	}
	#endregion

	public void InitializePrefabLibrary(){
		var database = new Dictionary<string,GameObject> ();
		foreach (GameObject obj in prefabs) {
			SceneObject so = obj.GetComponent<SceneObject> ();
			if (so) {
				database.Add (so.PrefabFileName, obj);
			}
		}
		SceneObject.InitializePrefabLibrary (database);
	}

	public void ResetLevel ()
	{
		if (!Application.isEditor)
			SMApplication.Notify (SMEventTypes.LevelReset);
		foreach (SceneObject child in sceneObjects.transform.GetComponentsInChildren<SceneObject> ()) {
			if (Application.isEditor) {
				DestroyImmediate (child.gameObject);
			} else {
				Destroy (child.gameObject);
			}
		}
	}

	void SetData(UnityEngine.Object target, params object[] data){
		this.data = target as LevelData;
	}

	public void ReloadLevel(){
		SMApplication.Notify (SMEventTypes.OnLevelLoad, data);
	}

	public void SetData(LevelData data){
		this.data = data;
	}

	public void CheckInitiateTutorial(UnityEngine.Object target, params object[] data){
		SMApplication.Notify (SMEventTypes.BeginTutorial, this.data, this.data.name);
	}

	public void LoadData(UnityEngine.Object target, params object[] data){
		
		LoadData ();
	}

	public void LoadData(){
		if (data) {
			ResetLevel ();
			Physics2D.gravity = data.gravity;
			var so = new GameObject[data.sceneObjects.Count];
			for (int i = 0; i < data.sceneObjects.Count; i++){
				so [i] = SceneObject.Create (data.sceneObjects [i], sceneObjects.transform);
//				PhysicsRecordManager.Instance.models.Add (so [i].GetComponent<PhysicsRecord> ());
			}
			for (int i = 0; i < so.Length; i++) {
				int linkIndex = data.sceneObjects [i].linkIndex;
				if (0 <= linkIndex && linkIndex < so.Length) {
					so [i].GetComponent<SceneObject> ().LinkedObject = so [linkIndex];
					Debug.Log (string.Format ("Setting {0} Linked Object[{1}] to {2}", so [i].name, linkIndex, so [linkIndex].name));
				} else {
					Debug.Log (string.Format ("No Linked Object set for {0}", so [i].name));
				}
			}
		}
	}

	public void SaveData(){
		List<SceneObject> objectList = new List<SceneObject> ();
		if (sceneObjects) {
			sceneObjects.transform.GetComponentsInChildren<SceneObject> (objectList);
		}

		data = ScriptableObject.CreateInstance<LevelData> ();
		data.Initialize (objectList);//.Select (so => so.GetData ()).ToList ());
	}
		
//	public void SaveLocal(string filename){
//		BinaryFormatter bf = new BinaryFormatter ();
//		FileStream file = File.Create (Application.persistentDataPath + "/" + filename + ".dat");
//
//		List<SceneObject> kineticObjects = new List<SceneObject> ();
//		if (sceneObjects) {
//			sceneObjects.transform.GetComponentsInChildren<SceneObject> (kineticObjects);
//		}
//
//		data = ScriptableObject.CreateInstance<LevelData> ();
//		data.Initialize(kineticObjects.Select (so => so.GetData ()).ToList ());
//
//		bf.Serialize (file, data);
//		file.Close ();
//	}
//
//	public void LoadLocal(string filename){
//		if (File.Exists (Application.persistentDataPath + "/" + filename + ".dat")) {
//			BinaryFormatter bf = new BinaryFormatter ();
//			FileStream file = File.Open (Application.persistentDataPath + "/" + filename + ".dat", FileMode.Open);
//
//			data = (LevelData)bf.Deserialize (file);
//			file.Close ();
//
//			if (sceneObjects) {
//				foreach (SceneObjectData sod in data.sceneObjects) {
//					SceneObject.Create (sod, sceneObjects.transform);
//				}
//			}
//		}
//	}

}
