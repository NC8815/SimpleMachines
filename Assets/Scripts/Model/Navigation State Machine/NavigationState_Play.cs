using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationState_Play : NavigationState {
	#region Notification Handlers
	public override void Enter ()
	{
		base.Enter ();
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.OnControllerInputAxes, HandleInput);
		base.SubscribeEvents ();
	}

	protected void HandleInput(Object target, params object[] data){
		SMApplication.Notify (SMEventTypes.PlayerMoveInput, this, data);
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.OnControllerInputAxes, HandleInput);
		base.UnsubscribeEvents ();
	}
	#endregion
}
