using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationStateMachine : SMStateMachine {

	protected override void Init ()
	{
		print ("changing state to setup");
		//ChangeState<NavigationState_Setup> ();
		SMApplication.Notify(SMEventTypes.ChangeNavigationState, this, typeof(NavigationState_Setup));
		base.Init ();
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.ChangeNavigationState, ChangeState);
		base.SubscribeEvents ();
	}

	void ChangeState(Object target, params object[] data){
		if (data.Length > 0) {
			System.Type type = data [0] as System.Type;
			if (type != null){
				if (type.BaseType == typeof(NavigationState)) {
					ChangeState (type);
				}
			} else {
				print ("NavigationState is null, not changing state.");
			}
		}else {
			print (string.Format ("State of {0} not changed. No state parameter supplied.", name));
		}
	}
}
