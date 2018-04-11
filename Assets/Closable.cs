using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoozyUI;

public class Closable : MonoBehaviour {

	// Use this for initialization
	void Awake(){
		SubscribeEvents ();
	}

	void SubscribeEvents (){
		SMApplication.Subscribe (SMEventTypes.OnLevelLoad, Close);
		SMApplication.Subscribe (SMEventTypes.CloseTutorial, Close);
		Debug.Log ("Subscribed");
	}

	void Close(Object target, params object[] data){
		Debug.Log ("Closing");
		Destroy (gameObject);
	}

	void OnDestroy(){
		UnsubscribeEvents ();
	}

	void UnsubscribeEvents(){
		SMApplication.Unsubscribe(SMEventTypes.OnLevelLoad,Close);
		SMApplication.Unsubscribe (SMEventTypes.CloseTutorial, Close);
	}
}
