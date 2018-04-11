using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : SMController {

	Vector3 inputVector;

	void Update () {
		inputVector = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		SMApplication.Notify (SMEventTypes.OnControllerInputAxes, this, inputVector);
	}
}
