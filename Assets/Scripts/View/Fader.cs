using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Fader : SMMVC {


	[SerializeField]
	DoozyUI.UIElement fader;

	public void FadeComplete(){
		SMApplication.Notify (SMEventTypes.FadeComplete);
	}

	IEnumerator CompleteFade(float waitTime){
		Debug.Log ("begin Fade");
		yield return new WaitForSecondsRealtime (waitTime);
		SMApplication.Notify (SMEventTypes.FadeComplete);
	}

	void FadeHide(Object target, params object[] data){
		float D;
		if (data.Length > 0 && float.TryParse((string)data[0],out D)) {
			float duration = D;
			fader.outAnimations.fade.duration = duration;
		}
		fader.Hide (false);
		StartCoroutine ("CompleteFade", fader.outAnimations.fade.duration);
	}

	void FadeShow(Object target, params object[] data){
		float D;
		if (data.Length > 0 && float.TryParse((string)data[0],out D)) {
			float duration = D;
			fader.inAnimations.fade.duration = duration;
		}
		StartCoroutine ("CompleteFade", fader.inAnimations.fade.duration);
		fader.Show (false);
	}

	protected override void SubscribeEvents ()
	{
		SMApplication.Subscribe (SMEventTypes.FadeToBlack, FadeShow);
		SMApplication.Subscribe (SMEventTypes.FadeFromBlack, FadeHide);
	}

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Unsubscribe (SMEventTypes.FadeToBlack, FadeShow);
		SMApplication.Unsubscribe (SMEventTypes.FadeFromBlack, FadeHide);
	}
}
