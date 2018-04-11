using UnityEngine;

public abstract class SMController : SMMVC
{
	protected override void SubscribeEvents(){
		SMApplication.Notify (SMEventTypes.OnControllerSubscribed, this);
	}

	protected override void UnsubscribeEvents(){
		SMApplication.Notify (SMEventTypes.OnControllerUnsubscribed, this);
	}
}