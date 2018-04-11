using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using GraphicDNA;



public class PhysicsDisplay : MonoBehaviour {

	float recordLength;
	ContactPoint2D[] contacts;
	ContactPoint2D contactBuffer;
	List<object> removalBuffer = new List<object> ();

	public int numContacts;

	Rigidbody2D myRigid;
	Vector2 gravForce { get { return Physics2D.gravity * myRigid.mass; } }
	Vector2 dragForce { get { return myRigid.drag * -1 * myRigid.velocity.sqrMagnitude * myRigid.velocity.normalized; } }

	Collider2D myCollider;

	public Dictionary<GameObject,ForceRecord> impulses = new Dictionary<GameObject, ForceRecord>();
	public Dictionary<GameObject,ForceRecord> frictions = new Dictionary<GameObject, ForceRecord> ();
	public Dictionary<ConstantForces,ForceRecord> constants = new Dictionary<ConstantForces, ForceRecord>();

	public PhysicsDisplayManager Tester;

	public enum ConstantForces{
		Gravity,
		Drag
	}

	[Serializable]
	public class ForceRecord{
		Queue<Vector2> forces = new Queue<Vector2>();
		public Vector2 anchor;
		public bool ShouldRemove { get { return forces.Count <= 0; } }

		public Vector2 Direction {
			get {
				if (ShouldRemove)
					return Vector2.zero;
				else
					return forces.Aggregate (Vector2.zero, (a, b) => a + b, v => v / forces.Count);
			}
		}

		public ForceRecord(Vector2 initial, Vector2 source, float recordDuration){
			int numRecords = Mathf.Max(1, Mathf.RoundToInt(recordDuration / Time.fixedDeltaTime));
//			Debug.Log(numRecords);
			for(int i = 0; i < numRecords; i++){
				forces.Enqueue(Vector2.zero);
			}
			RegisterForce(initial, source);
			Decay();
		}

		public void RegisterForce(Vector2 force, Vector2 source){
			forces.Enqueue (force);
			anchor = source;
		}

		public void Decay(){
			forces.Dequeue ();
//			Debug.Log (forces.Count);
		}
	}

	void Awake(){
		contacts = new ContactPoint2D[SMApplication.Instance.Tuning.maxContacts];
		recordLength = SMApplication.Instance.Tuning.recordLength;
		myRigid = GetComponent<Rigidbody2D> ();
		myCollider = GetComponent<Collider2D> ();
//		RegisterProcedures ();
		SubscribeEvents();
	}

	void OnDestroy(){
		UnsubscribeEvents ();
	}

//	void RegisterProcedures(){
//		Tester.RegConstants += RegisterConstants;
//		Tester.StepTime += SimulateTimeStep;
//		Tester.RegForces += RegisterReactions;
//		Tester.UnStepTime += ResetTimeStep;
//		Tester.DecayForces += DecayForces;
//		Tester.CullForces += CullForces;
//	}

	void SubscribeEvents(){
		SMApplication.Subscribe (SMEventTypes.PDRegisterConstants, RegisterConstants);
		SMApplication.Subscribe (SMEventTypes.PDStepTime, SimulateTimeStep);
		SMApplication.Subscribe (SMEventTypes.PDRegisterReactions, RegisterReactions);
		SMApplication.Subscribe (SMEventTypes.PDResetTimeStep, ResetTimeStep);
		SMApplication.Subscribe (SMEventTypes.PDDecayForces, DecayForces);
		SMApplication.Subscribe (SMEventTypes.PDCullForces, CullForces);
	}

	void UnsubscribeEvents(){
		SMApplication.Unsubscribe (SMEventTypes.PDRegisterConstants, RegisterConstants);
		SMApplication.Unsubscribe (SMEventTypes.PDStepTime, SimulateTimeStep);
		SMApplication.Unsubscribe (SMEventTypes.PDRegisterReactions, RegisterReactions);
		SMApplication.Unsubscribe (SMEventTypes.PDResetTimeStep, ResetTimeStep);
		SMApplication.Unsubscribe (SMEventTypes.PDDecayForces, DecayForces);
		SMApplication.Unsubscribe (SMEventTypes.PDCullForces, CullForces);
	}

	void RegisterConstants (UnityEngine.Object target, params object[] data)
	{
		RegisterForce (constants, ConstantForces.Gravity, myRigid.worldCenterOfMass, gravForce, SMApplication.Instance.Tuning.GravityColor);
		RegisterForce (constants, ConstantForces.Drag, myRigid.worldCenterOfMass, dragForce,SMApplication.Instance.Tuning.DragColor);
	}

	private Vector2 _pos;
	void SimulateTimeStep(UnityEngine.Object target, params object[] data){
		float stepLength = (float) data [0];
		var f = Vector2.zero;
		foreach (var kvp in constants) {
			f += kvp.Value.Direction;
		}
		_pos = transform.position;
		transform.Translate(stepLength * ( myRigid.velocity + stepLength * (f / myRigid.mass))); 
	}

	void ResetTimeStep(UnityEngine.Object target, params object[] data){
		transform.position = _pos;
	}

	void RegisterReactions(UnityEngine.Object target, params object[] data){
		float stepLength = (float) data [0];
		numContacts = myRigid.GetContacts (contacts);
		for (int i = 0; i < numContacts; i++) {
			contactBuffer = contacts [i];
			RegisterForce (impulses, contactBuffer.collider.gameObject, myRigid.worldCenterOfMass, contactBuffer.normal * contactBuffer.normalImpulse / stepLength,SMApplication.Instance.Tuning.NormalColor);
			RegisterForce (frictions, contactBuffer.collider.gameObject, myRigid.worldCenterOfMass, contactBuffer.relativeVelocity.normalized * contactBuffer.tangentImpulse / stepLength,SMApplication.Instance.Tuning.FrictionColor);
		}
	}

//	void FixedUpdate(){
//		RegisterConstants ();
//		numContacts = myRigid.GetContacts (contacts);
//		Debug.Log (string.Format ("Detected Contacts (Fixed Update): {0}", numContacts));
////		drawBox = false;
////		for (int i = 0; i < numContacts; i++) {
////			contactBuffer = contacts [i];
////			RegisterForce (impulses, contactBuffer.collider.gameObject, contactBuffer.point, contactBuffer.normal * contactBuffer.normalImpulse);
////			RegisterForce (frictions, contactBuffer.collider.gameObject, contactBuffer.point, contactBuffer.relativeVelocity.normalized * contactBuffer.tangentImpulse);
//////			drawBox = true;
////		}
//		DecayForces ();
//		CullForces ();
//	}
//
//	void OnCollisionStay2D(Collision2D col){
////		var contact = col.contacts.First();
//		contacts = col.contacts;
//		Debug.Log (string.Format("Detected Contacts (OnColStay): {0}", contacts.Length));
//		GameObject key = col.collider.gameObject;
//		foreach (var contact in contacts) {
//			RegisterForce (impulses, key, contact.point, contact.normal * contact.normalImpulse * 10);
//			RegisterForce (frictions, key, contact.point, contact.relativeVelocity.normalized * contact.tangentImpulse);
//			target = contact;
//			drawBox = true;
//		}
//	}
//
//	ContactPoint2D target;
//	bool drawBox =false;
//
//	void OnGUI(){
//		if (drawBox) {
////			target = contactBuffer;
//			GUI.Box (new Rect (0, 0, Screen.width, 20), string.Format ("{0}, {1}, {2}, {3}", target.collider.gameObject.name, target.point.ToString("0.000"), target.normal.ToString("0.000"), (target.normalImpulse * 10).ToString("0.000")));
//		}
//	}

	void Update(){
		DisplayForces ();
	}

	void DecayForces(UnityEngine.Object target, params object[] data){
		DecayForce (constants);
		DecayForce (impulses);
		DecayForce (frictions);
	}

	void CullForces(UnityEngine.Object target, params object[] data){
		CullForce (constants);
		CullForce (impulses);
		CullForce (frictions);
	}

	void DisplayForces(){
		foreach (var force in constants.Values) {
			Debug.DrawRay (force.anchor, force.Direction,Color.green);
			vectorDisplay [force].UpdateData (force);
		}
		foreach(var force in impulses.Values){
			Debug.DrawRay (force.anchor, force.Direction,Color.blue);
			vectorDisplay [force].UpdateData (force);
		}
		foreach(var force in frictions.Values){
			Debug.DrawRay (force.anchor, force.Direction,Color.red);
			vectorDisplay [force].UpdateData (force);
		}
	}
//		IEnumerable<Vector2> NetForce = constants.Values.Select(fr=>fr.Direction);
//		NetForce = NetForce.Concat (impulses.Values.Select(fr=>fr.Direction));
//		NetForce = NetForce.Concat (frictions.Values.Select(fr=>fr.Direction));
//		var tail = myRigid.worldCenterOfMass;
//		Vector2 head;
//		foreach (var force in NetForce) {
//			head = tail + force;
//			Debug.DrawLine (tail, head, Color.yellow);
//			tail = head;
//		}

//	void OnGUI(){
//		screenRect = Camera.main.rect;
//
//		Drawing2D.SetParentBounds (screenRect);
//		foreach (var force in constants.Values) {
//			var p1 = Camera.main.WorldToScreenPoint (force.anchor);
//			var p2 = Camera.main.WorldToScreenPoint (force.anchor + force.Direction);
//			Drawing2D.DrawArrow (p1, p2, Color.red, 3, 8, 10);
//		}
//		foreach(var force in impulses.Values){
//			var p1 = Camera.main.WorldToScreenPoint (force.anchor);
//			var p2 = Camera.main.WorldToScreenPoint (force.anchor + force.Direction);
//			Drawing2D.DrawArrow (p1, p2, Color.green, 3, 8, 10);
//		}
//		foreach(var force in frictions.Values){
//			var p1 = Camera.main.WorldToScreenPoint (force.anchor);
//			var p2 = Camera.main.WorldToScreenPoint (force.anchor + force.Direction);
//			Drawing2D.DrawArrow (p1, p2, Color.blue, 3, 8, 10);
//		}
//		Drawing2D.ClearParentBounds ();
//	}

	void DecayForce<T>(Dictionary<T,ForceRecord> records){
		foreach (var force in records.Values) {
			force.Decay ();
		}
	}

	void CullForce<T>(Dictionary<T,ForceRecord> records){
		removalBuffer.Clear ();
		foreach (var kvp in records) {
			if (kvp.Value.ShouldRemove)
				removalBuffer.Add (kvp.Key);
		}
		foreach (var key in removalBuffer) {
			Destroy (vectorDisplay [records [(T)key]].gameObject);
			vectorDisplay.Remove (records[(T)key]);
			records.Remove ((T)key);
		}
	}

	void RegisterForce<T>(Dictionary<T,ForceRecord> records, T key, Vector2 anchor, Vector2 force, Color color){
		if (!records.ContainsKey (key)) {
			records.Add (key, new ForceRecord (force, anchor, recordLength));
			SpawnForceVector (records [key], color);
		} else {
			records [key].RegisterForce (force, anchor);
		}
	}

	[SerializeField]
	GameObject forceVectorPrefab = null;
	Dictionary<ForceRecord,ForceVector> vectorDisplay = new Dictionary<ForceRecord, ForceVector>();
	void SpawnForceVector(ForceRecord record, Color color){
		var go = Instantiate (forceVectorPrefab,transform);
		var fv = go.GetComponent<ForceVector> ();
		fv.Color = color;
		vectorDisplay.Add (record, fv);
	}
}
