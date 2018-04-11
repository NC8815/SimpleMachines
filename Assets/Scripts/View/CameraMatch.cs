using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMatch : MonoBehaviour {
	void Start () {
		Camera camera = Camera.main;
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		//renderer.drawMode = SpriteDrawMode.Tiled;
		renderer.size = new Vector2 (camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2);
		//print (renderer.size.x);
		renderer.material.SetVector ("_Tiling", new Vector2 (renderer.size.x/4, 0.1f));
	}
}
