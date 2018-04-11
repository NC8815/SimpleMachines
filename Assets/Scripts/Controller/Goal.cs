using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : SceneObject {

	void OnCollisionEnter2D (Collision2D coll){
		SMApplication.Notify (SMEventTypes.LevelCollisionGoal,this);
	}

}
