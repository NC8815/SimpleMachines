using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStatePosition : SMState {

//	[SerializeField]
	public SpriteRenderer dragZone = null;
	new Camera camera;
	Rect placeableZone;
	Bounds placeable;

	public override void Enter ()
	{
		GetComponent<SpriteRenderer> ().color = Color.white;
		Vector3 extents = GetComponent<Collider2D> ().bounds.extents;
		Bounds bounds = GetComponent<Collider2D> ().bounds;
		//print ("Extents: " + extents.ToString ());
//		print ("DragZone: " + dragZone.bounds.ToString ()); 
		//placeableZone = new Rect (dragZone.bounds.min + bounds.extents, dragZone.bounds.size - bounds.size);
		//placeable = new Bounds (placeableZone.center, placeableZone.size);
		placeable = new Bounds (dragZone.bounds.center, dragZone.bounds.size - bounds.size);
//		print ("Placeable: " + placeable.ToString ());
		camera = Camera.main;
		base.Enter ();
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.SetupOnPointerClick, HandleSetupClick);
		SMApplication.Subscribe (SMEventTypes.SetupOnPointerDrag, HandleSetupDrag);
		SMApplication.Subscribe (SMEventTypes.ChangeNavigationState, HandleNavStateChange);
		base.SubscribeEvents ();
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.SetupOnPointerClick, HandleSetupClick);
		SMApplication.Unsubscribe (SMEventTypes.SetupOnPointerDrag, HandleSetupDrag);
		SMApplication.Unsubscribe (SMEventTypes.ChangeNavigationState, HandleNavStateChange);
		base.UnsubscribeEvents ();
	}

	void HandleSetupDrag(Object target, params object[] data){
		if (target = gameObject) {
			//print ("Input Position: " + Input.mousePosition.ToString ());
			Vector2 closest = placeable.ClosestPoint (camera.ScreenToWorldPoint( Input.mousePosition));
			gameObject.transform.position = closest;
		}
	}

	void HandleSetupClick(Object target, params object[] data){
		if (target = gameObject) {
			GetComponent<SMStateMachine> ().ChangeState<BallStateVelocity> ();
		} else {
			print ("Click target " + target.ToString () + " and " + gameObject.ToString () + " are not equal.");
		}
	}

	void HandleNavStateChange(Object target, params object[] data){
		if (data.Length > 0) {
			System.Type type = data [0] as System.Type;
			if (type != null && type == typeof(NavigationState_Play)){
				GetComponent<SMStateMachine> ().ChangeState<BallStatePlay> ();
			} else {
				print ("NavigationState is null, not changing state.");
			}
		}
		else {
			print (string.Format ("State of {0} not changed. No state parameter supplied.", name));
		}
	}
}
