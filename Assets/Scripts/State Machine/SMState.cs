using UnityEngine;

public abstract class SMState : MonoBehaviour {

	public virtual void Enter(){
		SubscribeEvents ();
		SMApplication.Notify (SMEventTypes.OnStateEnter, this);
	}

	public virtual void Exit(){
		UnsubscribeEvents ();
		SMApplication.Notify (SMEventTypes.OnStateExit, this);
	}

	protected virtual void OnDestroy(){
		UnsubscribeEvents ();
	}

	protected virtual void SubscribeEvents(){
		SMApplication.Notify (SMEventTypes.OnStateSubscribed, this);
	}

	protected virtual void UnsubscribeEvents(){
		SMApplication.Notify (SMEventTypes.OnStateUnsubscribed, this);
	}
}