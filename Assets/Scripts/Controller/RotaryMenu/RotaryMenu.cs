using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoozyUI.Gestures;

public class RotaryMenu : MonoBehaviour {

	public DoozyUI.UIElement RotorDisplay = null;
	public float StartDelay = 2;

	public List<GameObject> Options = new List<GameObject>();
	public RectTransform RotaryTrack;
	public float MinScale;
	public float MaxScale;
	[Range(0,2)]
	public float Power;
	[Range(0.1f,2)]
	public float Sensitivity;

	private bool dragging = false;
	private float target;
	private float offset = 0;
	private float offsetStart;
	private Touch touchStart;
	private float dragScale;


	IEnumerator Start(){
		yield return new WaitForSecondsRealtime (StartDelay);
		RotorDisplay.Show (false);
	}

	void Update(){
		if (Options.Count > 0) {
//			offset = dragging ? GetDragOffset () : GetBreakOffset ();
			AdjustTarget();
			offset = Mathf.Lerp (offset, target, Mathf.Clamp01 (Time.unscaledDeltaTime * Mathf.Pow (Mathf.Abs (offset - target), -Power)));
			UpdateOptions ();
		}
	}

	void AdjustTarget(){
		if (dragging) {
			Touch touch = InputHelper.GetTouches () [0];
			if (touch.phase == TouchPhase.Moved) {
				float delta = (touch.position - touchStart.position).x / dragScale;
				target = Mathf.Round ((offsetStart + delta) * Options.Count) / Options.Count;
//				if (Mathf.Abs (target - offset) > 0.5) {
//					target += Mathf.Sign (offset - target);
//				}
			}
		}
	}
//
//	float GetBreakOffset ()
//	{
//		return Mathf.Lerp (offset, Mathf.Round (Options.Count * offset) / Options.Count, Time.unscaledDeltaTime);
//	}
//
//	float GetDragOffset(){
//		Touch touch = InputHelper.GetTouches () [0];
//		if (touch.phase == TouchPhase.Moved) {
//			float horizontalDistance = (touch.position - touchStart.position).x;
//			return Mathf.Lerp(offset, offsetStart + horizontalDistance / dragScale, Time.unscaledDeltaTime);
//		}
//		return offset;
//	}

	void AdjustTransform(GameObject option, float t){
		t = Mathf.Repeat (t, 1);
		Rect R = RotaryTrack.rect;
		float angle = t * Mathf.PI * 2 - Mathf.PI / 2;
		float Y = R.height / 2 * Mathf.Sin (angle);
		float X = R.width / 2 * Mathf.Cos (angle);
		float scale = Mathf.Lerp (MinScale, MaxScale, Mathf.InverseLerp (1, -1, Mathf.Sin (angle)));
//		Debug.Log (option.GetComponent<RectTransform> ().anchoredPosition.ToString ());
//		Debug.Log (new Vector2 (X, Y).ToString ());
		option.GetComponent<RectTransform> ().anchoredPosition = new Vector2(X, Y);
//		Debug.Log (option.GetComponent<RectTransform> ().anchoredPosition.ToString ());
		option.GetComponent<RectTransform> ().localScale = new Vector3(scale, scale, 1);
	}

	void UpdateOptions(){
		for (int i = 0; i < Options.Count; i++) {
			AdjustTransform (Options [i], offset + (float)i/Options.Count);
		}
	}

	public void BeginDrag(){
		dragging = true;
		touchStart = InputHelper.GetTouches () [0];
		offsetStart = offset;
		dragScale = GetComponent<RectTransform> ().rect.width / Sensitivity;
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);
	}

	public void EndDrag(){
		Debug.Log (System.Reflection.MethodBase.GetCurrentMethod ().Name);

		dragging = false;
	}
}
