using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PhysicsDisplayManager : MonoBehaviour {
//	[SerializeField]
//	PhysicsRecord testee;
	// Use this for initialization

	Rigidbody2D myRigid;
	ContactPoint2D[] contacts = new ContactPoint2D[10];

	float timeStep;

	public Action RegConstants;
	public Action<float> StepTime;
	public Action<float> RegForces;
	public Action UnStepTime;
	public Action DecayForces;
	public Action CullForces;

	void Awake(){
		myRigid = GetComponent<Rigidbody2D> ();
		timeStep = Time.fixedDeltaTime;
	}

//	void TryExecute(Action action){
//		if (action != null)
//			action.Invoke ();
//	}
//
//	void TryExecute<T>(Action<T> action, T value){
//		if (action != null)
//			action.Invoke (value);
//	}

	void FixedUpdate(){
		if (timeStep != 0) {
			SMApplication.Notify (SMEventTypes.PDRegisterConstants);
			SMApplication.Notify (SMEventTypes.PDStepTime, null, timeStep);
			SMApplication.Notify (SMEventTypes.PDRegisterReactions, null, timeStep);
			SMApplication.Notify (SMEventTypes.PDResetTimeStep);
			SMApplication.Notify (SMEventTypes.PDDecayForces);
			SMApplication.Notify (SMEventTypes.PDCullForces);
//			TryExecute (RegConstants);
//			TryExecute (StepTime, timeStep);
//			TryExecute (RegForces, timeStep);
//			TryExecute (UnStepTime);
//			TryExecute (DecayForces);
//			TryExecute (CullForces);
		}
	}
}
