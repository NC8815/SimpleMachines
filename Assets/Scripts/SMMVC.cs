using UnityEngine;

public abstract class SMMVC : MonoBehaviour {
	void Awake(){
		SubscribeEvents ();
	}

	void Start(){
		Init ();
	}

	void OnDestroy(){
		UnsubscribeEvents ();
	}

	protected virtual void Init (){}

	protected abstract void SubscribeEvents ();

	protected abstract void UnsubscribeEvents ();
}