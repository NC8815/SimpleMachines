using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateDisplay : SMView {

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.ChangeNavigationState, HandleStateChange);
		print ("subscribed to state changes");
		base.SubscribeEvents ();
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.ChangeNavigationState, HandleStateChange);
		base.UnsubscribeEvents ();
	}

	void HandleStateChange(Object target, params object[] data){
		if (data.Length > 0) {
			System.Type type = data [0] as System.Type;
			if (type != null){
				if (type.BaseType == typeof(NavigationState)) {
					GetComponent<Text> ().text = type.ToString ();
				}
			} else {
				print ("NavigationState is null, not changing state.");
			}
		}
		else {
			print (string.Format ("State of {0} not changed. No state parameter supplied.", name));
		}
	}
}
