using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysicsBall : SceneObject, IDragHandler,IPointerClickHandler {

	public void Init ()
	{
		GetComponent<BallStatePosition> ().dragZone = LinkedObject.GetComponent<SpriteRenderer> ();
		GetComponent<SMStateMachine> ().ChangeState<BallStatePosition> ();
	}

	void Start(){
		Init ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
//		print ("Clicked Ball");
		SMApplication.Notify (SMEventTypes.OnPointerClick, this, GetType());
	}

	public void OnDrag (PointerEventData eventData)
	{
		SMApplication.Notify (SMEventTypes.OnPointerDrag, this);
	}
		/*
	protected override void Init ()
	{
		sm = GetComponent<SMStateMachine> ();
		base.Init ();
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.SetupOnPointerClick, HandleSetupClick);
		SMApplication.Subscribe (SMEventTypes.SetupOnPointerDrag, HandleSetupDrag);
		base.SubscribeEvents ();
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.SetupOnPointerClick, HandleSetupClick);
		SMApplication.Unsubscribe (SMEventTypes.SetupOnPointerDrag, HandleSetupDrag);
		base.UnsubscribeEvents ();
	}

	void HandleSetupDrag(Object target, params object[] data){
	}

	void HandleSetupClick(Object target, params object[] data){
		gameObject.GetComponent<SpriteRenderer>().color = Color.red;
		SMApplication.Notify (SMEventTypes.ChangeNavigationState, this, typeof(NavigationState_Play));
	}
*/
}
