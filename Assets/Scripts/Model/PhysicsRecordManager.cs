using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhysicsRecordManager : Singleton<PhysicsRecordManager> {

	public int RecordsPerUpdate = 5;
	public List<PhysicsRecord> models = new List<PhysicsRecord> ();

	public void ClearModels(UnityEngine.Object target, params object[] data){
		models.Clear ();
	}

	void Awake(){
		SubscribeEvents ();
	}

	void SubscribeEvents(){
		SMApplication.Subscribe (SMEventTypes.LevelReset, ClearModels);
	}

//
//	public Rigidbody2D Test;
//	public Rigidbody2D Result;


//	public List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
//	public List<PhysicsModel> models = new List<PhysicsModel>();


	void FixedUpdate(){
		if (models.Count > 0) {
			SMApplication.Notify (SMEventTypes.LoadModelData);
			for (int i = 0; i < RecordsPerUpdate; i++) {
				SMApplication.Notify (SMEventTypes.CalculateForces);
				SMApplication.Notify (SMEventTypes.ApplyForces);
			}
			SMApplication.Notify (SMEventTypes.DisplayModelData);
		}
	}

//	void Start(){
//		LoadModels ();
//	}
//
//	void LoadModels(){
//		foreach (Rigidbody2D R in rigidbodies) {
//			Rigidbody2D model = new Rigidbody2D ();
//		}
//	}
//
//	void UpdateModels(){
//		for (int i = 0; i < RecordsPerUpdate; i++) {
//			foreach (var model in models) {
//				model.UpdateModel ();
//			}
//		}
//	}
//
//	void DisplayModels(){
//	}
}

//[Serializable]
//public class PhysicsModel{
//	Vector2 position;
//	Vector2 velocity;
//	float rotation;
//
//	Vector2 centerOfMass;
//	float drag;
//	float angularDrag;
//	float inertia;
//	float mass;
//
//	public void UpdateModel(List<PhysicsRecordTask> tasks){
//		foreach (var task in tasks) {
//			task.Visualize (this);
//		}
//	}
//}

