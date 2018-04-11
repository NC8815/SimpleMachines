using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTimeScale : MonoBehaviour {

	private float _myTime = 0;
	private SpriteRenderer _renderer;

	public float TimeScale = 1;

	void Awake(){
		_renderer = GetComponent<SpriteRenderer> ();
	}

	void Update(){
		_myTime += Time.unscaledDeltaTime * TimeScale;
		_renderer.material.SetFloat ("_MyTime", _myTime);
	}
}
