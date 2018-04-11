using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStateVelocity : SMState {

//	[SerializeField]
	LineRenderer velocityRenderer = null;
	public GameObject rendererPrefab;
	Rigidbody2D rb;
	new Camera camera;

	public override void Enter ()
	{
		if (velocityRenderer == null) {
			GameObject lr = Instantiate (rendererPrefab,transform,true);
			velocityRenderer = lr.GetComponent<LineRenderer> ();
		}
		GetComponent<SpriteRenderer> ().color = Color.yellow;
		velocityRenderer.enabled = true;
		velocityRenderer.positionCount = 2;
		velocityRenderer.SetPositions (new Vector3[]{ transform.position, transform.position });
		camera = Camera.main;
		rb = GetComponent<Rigidbody2D> ();
		base.Enter ();
	}

	public override void Exit ()
	{
		velocityRenderer.enabled = false;
		base.Exit ();
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
		SMApplication.Unsubscribe (SMEventTypes.ChangeNavigationState, HandleSetupClick);
		base.UnsubscribeEvents ();
	}

	void HandleSetupDrag(Object target, params object[] data){
		if (target = gameObject) {
			Vector3 touchPoint = camera.ScreenToWorldPoint (Input.mousePosition);
			velocityRenderer.SetPositions (new Vector3[]{ transform.position, touchPoint});
			rb.velocity = touchPoint - transform.position;
		}
	}

	void HandleSetupClick(Object target, params object[] data){
		if (target = gameObject) {
			GetComponent<SMStateMachine> ().ChangeState<BallStatePosition> ();
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
