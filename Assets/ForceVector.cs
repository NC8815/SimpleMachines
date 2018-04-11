using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceVector : MonoBehaviour {
	public Vector2 Anchor;
	public Vector2 Direction;
	public float Scale;
	public Color Color;
	SpriteRenderer spriteRenderer;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
		
	void Update(){
		transform.position = Anchor;
		transform.eulerAngles = new Vector3 (0, 0, Vector2.SignedAngle (Vector2.right, Direction));
		transform.localScale = new Vector3 (Direction.magnitude * Scale, 3);// * Vector3.one;
	}

	public void UpdateData(PhysicsDisplay.ForceRecord record){
		Anchor = record.anchor;
		Direction = record.Direction;
		spriteRenderer.color = Color;
	}
}
