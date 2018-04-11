using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : SMModel {

	Vector3 moveDirection = Vector3.zero;
	public float speed = 1;

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe(SMEventTypes.PlayerMoveInput,UpdateMovementTarget);
		base.SubscribeEvents ();
	} 

	protected override void UnsubscribeEvents ()
	{
		base.UnsubscribeEvents ();
	}

	void UpdateMovementTarget(Object target, params object[] data){
		if (data.Length > 0){
			var moveDir = data [0] as Vector3?;
			if (moveDir.HasValue) {
				moveDirection = moveDir.Value;
			}
		}
	}
	
	// Update is called once per frame
}
