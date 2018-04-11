using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifier : MonoBehaviour {

	public Object target;
	public SMEventTypes type;

	public void Notify(){
		SMApplication.Notify (type,target);
	}

}
