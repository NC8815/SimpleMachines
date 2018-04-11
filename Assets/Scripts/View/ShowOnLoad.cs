using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnLoad : MonoBehaviour {

	void Awake(){
		SubscribeEvents ();
	}

	void OnDestroy(){
		UnsubscribeEvents ();
	}

	void SubscribeEvents(){
		SMApplication.Subscribe (SMEventTypes.LevelLoadComplete, ShowElement);
		SMApplication.Subscribe (SMEventTypes.LevelReset, HideElement);
	}

	void UnsubscribeEvents(){
		SMApplication.Unsubscribe (SMEventTypes.LevelLoadComplete, ShowElement);
		SMApplication.Unsubscribe (SMEventTypes.LevelReset, HideElement);
	}

	void ShowElement(Object target, params object[] data){
		GetComponent<DoozyUI.UIElement> ().Show (true);
	}

	void HideElement(Object target, params object[] data){
		GetComponent<DoozyUI.UIElement> ().Hide (true);
	}
}
