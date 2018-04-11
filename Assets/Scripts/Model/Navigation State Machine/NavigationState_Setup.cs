using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationState_Setup : NavigationState {
	#region Notification Handlers
	public override void Enter ()
	{
		PauseSimulation ();
		base.Enter ();
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.OnPointerClick, HandleClick);
		SMApplication.Subscribe (SMEventTypes.OnPointerDrag, HandleDrag);
		base.SubscribeEvents ();
	}

	protected void HandleClick(Object target, params object[] data){
//		print ("SetupReceivingClickNotification, sending personal notification");
		SMApplication.Notify (SMEventTypes.SetupOnPointerClick, target, data);
	}

	protected void HandleDrag(Object target, params object[] data){
		SMApplication.Notify (SMEventTypes.SetupOnPointerDrag, target, data);
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.OnPointerClick, HandleClick);
		SMApplication.Unsubscribe (SMEventTypes.OnPointerDrag, HandleDrag);
		base.UnsubscribeEvents ();
	}

	public override void Exit ()
	{
		UnpauseSimulation ();
		base.Exit ();
	}

	#endregion

	void PauseSimulation(){
		Time.timeScale = 0.0f;
	}

	void UnpauseSimulation(){
		Time.timeScale = 1.0f;
	}
}
