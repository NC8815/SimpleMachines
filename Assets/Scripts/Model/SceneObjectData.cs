using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneObjectData{

	public string PrefabFileName;
	public SerializableTransform transform;
	public SerializableV2 size;
	public bool active;
	public int linkIndex = -1;

	public SceneObjectData(){
	}

	public SceneObjectData(SceneObject sceneObject){
		PrefabFileName = sceneObject.PrefabFileName;
		transform = new SerializableTransform (sceneObject.transform.position, sceneObject.transform.eulerAngles, sceneObject.transform.localScale);
		size = sceneObject.GetComponent<SpriteRenderer> () ? new SerializableV2(sceneObject.GetComponent<SpriteRenderer> ().size) : new SerializableV2(Vector2.zero);
		active = sceneObject.gameObject.activeInHierarchy;
	}
}

[Serializable]
public class SerializableV2{
	public float x, y;

	public SerializableV2(Vector2 vector){
		x = vector.x;
		y = vector.y;
	}

	public Vector2 GetVector(){
		return new Vector2 (x, y);
	}
}

[Serializable]
public class SerializableTransform{
	public float xp,yp,zp;
	public float xr,yr,zr;
	public float xs,ys,zs;

	public SerializableTransform(Vector3 position, Vector3 rotation, Vector3 scale){
		xp = position.x;
		yp = position.y;
		zp = position.z;

		xr = rotation.x;
		yr = rotation.y;
		zr = rotation.z;

		xs = scale.x;
		ys = scale.y;
		zs = scale.z;
	}

	public Vector3 GetPosition(){
		return new Vector3 (xp, yp, zp);
	}

	public Vector3 GetRotation(){
		return new Vector3 (xr, yr, zr);
	}

	public Vector3 GetScale(){
		return new Vector3 (xs, ys, zs);
	}
}
