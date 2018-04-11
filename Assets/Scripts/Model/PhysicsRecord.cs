using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public enum ConstantForces{
	Gravity,
	Drag
}

public class PhysicsRecord : SMMVC {

	public Rigidbody2D myRigidbody;
	public Collider2D myCollider;

	public Vector2 velocity;
	public float angularVelocity;
	public Vector2 position;

	List<Vector2> persistentForces = new List<Vector2>();
	Vector2 GravForce { get { return myRigidbody.mass * Physics2D.gravity; } }
	Vector2 DragForce { get { return myRigidbody.drag * -1 * _velocity.sqrMagnitude * _velocity.normalized; } }

	List<float> persistentTorques = new List<float> ();
	float AngularDragForce { get { return myRigidbody.angularDrag * -1 * Mathf.Pow (angularVelocity, 2) * Mathf.Sign(angularVelocity); } }

	ContactPoint2D[] contacts;
	int maxContacts = 6;

	Dictionary<PhysicsRecord,Vector2> normalForces = new Dictionary<PhysicsRecord, Vector2>();
	Dictionary<PhysicsRecord,Vector2> frictionForces = new Dictionary<PhysicsRecord, Vector2>();
	Dictionary<PhysicsRecord,float> Torques = new Dictionary<PhysicsRecord, float>();

	private Dictionary<Collider2D,Queue<Vector2>> impulseRecords = new Dictionary<Collider2D, Queue<Vector2>> ();
	private Dictionary<Collider2D,Queue<Vector2>> frictionRecords = new Dictionary<Collider2D, Queue<Vector2>> ();
	private Dictionary<Collider2D, Queue<float>> torqueRecords = new Dictionary<Collider2D, Queue<float>>();
	private Dictionary<ConstantForces,Queue<Vector2>> constantRecords = new Dictionary<ConstantForces, Queue<Vector2>>();

	private float _timeScale;

	private Vector2 _position;
	private Quaternion _rotation;
	private Vector2 _velocity;

	void ChangeTimeScale(Object target, params object[] data){
		if (data.Length > 0 && data [0] is float) {
			_timeScale = ((float)data [0]);
		}
	}

	public void SimulateTimeStep(Object target, params object[] data){
		_position = transform.position;
		_rotation = transform.rotation;

		Vector2 forces = Vector2.zero;
		foreach (var kvp in constantRecords) {
			if (kvp.Value.Count > 0) {
				forces += kvp.Value.Last ();
			}
		}



//		Vector2 forces = constantRecords.Select (record => record.Value.Last ()).Aggregate (Vector2.zero, (a, b) => a + b);
		_velocity = myRigidbody.velocity + forces / myRigidbody.mass;

		transform.Translate (_velocity * _timeScale);
		transform.RotateAround (myRigidbody.centerOfMass, Vector3.forward, myRigidbody.angularVelocity * _timeScale);
	}

	public void ModelPhysics(Object target, params object[] data){
		int numContacts = myCollider.GetContacts (SMApplication.Instance.Tuning.filter, contacts);
		Debug.Log (numContacts);
//		int numCollisions = Physics2D.OverlapCollider (myCollider, SMApplication.Instance.Tuning.filter, collisions);
		ModelConstants ();
		for (int i = 0; i < numContacts; i++) {
			ModelImpulse (contacts[i]);
			ModelFriction (contacts [i]);
			ModelTorque (contacts [i]);
		}
		DecayRecords ();
	}

	void ModelConstants(){
		RegisterForce<ConstantForces> (constantRecords, ConstantForces.Gravity, GravForce);
		if (myRigidbody.velocity.sqrMagnitude > 0)
			RegisterForce<ConstantForces> (constantRecords, ConstantForces.Drag, DragForce);
	}

	void ModelImpulse(ContactPoint2D contact){
		Debug.Log (contact.ToString ());
		RegisterForce (impulseRecords, contact.collider, contact.normal);
		Debug.DrawLine (contact.point, contact.point + contact.normal * 10,Color.red);
	}

	void ModelFriction(ContactPoint2D other){
	}

	void ModelTorque(ContactPoint2D other){
	}

	public void ResetTimeStep(Object target, params object[] data){
		transform.position = _position;
		transform.rotation = _rotation;
	}

	void RegisterForce<T> (Dictionary<T,Queue<Vector2>> record, T key, Vector2 val){
		int maxRecords = SMApplication.Instance.Tuning.maxContacts;
		if (!record.ContainsKey (key)) {
			record.Add (key, new Queue<Vector2> ());
			for (int i = 0; i < maxRecords; i++) {
				record [key].Enqueue (Vector2.zero);
			}
		}
		record [key].Enqueue (val);
		record [key].Dequeue ();
	}

	void DecayRecords(){
		DecayRecords (constantRecords);
		DecayRecords (impulseRecords);
		DecayRecords (frictionRecords);
		DecayTorques (torqueRecords);
	}


	void DecayRecords<T>(Dictionary<T,Queue<Vector2>> record){
		List<T> removalSchedule = new List<T> ();
		foreach (T key in record.Keys) {
			Debug.Log (string.Format("{0}: {1} entries.", key.ToString (),record[key].Count));

			record [key].Enqueue (Vector2.zero);
			record [key].Dequeue ();
			if (record [key].Aggregate ((a, b) => a + b).sqrMagnitude == 0)
				removalSchedule.Add (key);
		}
		foreach (T key in removalSchedule) {
			record.Remove (key);
		}
	}

	void DecayTorques<T>(Dictionary<T,Queue<float>> record){
		foreach (var key in record.Keys) {
			record [key].Enqueue (0);
			record [key].Dequeue ();
			if (record [key].Sum() == 0)
				record.Remove (key);
		}
	}
//	public Dictionary<PhysicsRecord,Manifold> myRecords = new Dictionary<PhysicsRecord, Manifold>();
//
//	public class Manifold{
//		int maxRecordLength = 1;
//		Queue<Vector2> forceRecord = new Queue<Vector2>();
//		List<ContactPoint2D> myContacts = new List<ContactPoint2D> ();
//
//		public Vector2 average { get { return forceRecord.Aggregate ((a, b) => a + b)/((float)forceRecord.Count); } }
//
//		public Manifold(int maxRecords, ContactPoint2D[] contacts){
//			maxRecordLength = maxRecords;
//			myContacts = new List<ContactPoint2D>( contacts);
//			for(int i =0; i <maxRecords; i++){
//				forceRecord.Enqueue(Vector2.zero);
//			}
//		}
//
//		public void AddRecord(Vector2 record){
//			forceRecord.Enqueue(record);
//			forceRecord.Dequeue ();
//		}
//
//		public void UpdateContacts(ContactPoint2D[] contacts){
//			myContacts = new List<ContactPoint2D>(contacts);
//		}
//	}

//	void LoadData(Object target, params object[] data){
//		ClearBuffers ();
//		velocity = myRigidbody.velocity;
//		angularVelocity = myRigidbody.angularVelocity;
//		position = myRigidbody.position;
//		persistentForces.Add (GravForce);
//		persistentForces.Add (DragForce);
//		persistentTorques.Add (AngularDragForce);
//	}
//
//	void ClearBuffers(){
//		persistentForces.Clear ();
//		normalForces.Clear ();
//		frictionForces.Clear ();
//		Torques.Clear ();
//		persistentTorques.Clear ();
//	}

//	void CalculateForces(){
//		int numCollisions = Physics2D.OverlapCollider (myCollider, SMApplication.Instance.Tuning.filter, collisions);
//		for (int i = 0; i < numCollisions; i++) {
//			CalculateNormalForce (collisions [i]);
//			CalculateFriction (collisions [i]);
//			CalculateTorque (collisions [i]);
//		}
//	}
//
//	void CalculateNormalForce(Collision2D col){
//		PhysicsRecord record = col.gameObject.GetComponent<PhysicsRecord> ();
//		Vector2 relVel = record.myRigidbody.velocity - myRigidbody.velocity;
////		float velAlongNormal = Vector2.Dot(relVel,col.contacts.First().
//
//		float e1 = myRigidbody.sharedMaterial.bounciness;
//		float e2 = record.myRigidbody.sharedMaterial.bounciness;
//		Vector2 normal;
//		RegisterForce (normalForces, record, normal);
//	}
//
//	void CalculateFriction(Collision2D col){
//		PhysicsRecord record = col.gameObject.GetComponent<PhysicsRecord> ();
//		Vector2 friction;
//		RegisterForce (frictionForces, record, friction);
//	}
//
//	void RegisterForce(Dictionary<PhysicsRecord,Vector2> forces, PhysicsRecord record, Vector2 force){
//		if (forces.ContainsKey (record)) {
//			forces [record] += force;
//		} else {
//			forces.Add (record, force);
//		}
//	}
//
//	void CalculateTorque(Collision2D col){
//		PhysicsRecord record = col.gameObject.GetComponent<PhysicsRecord> ();
//		float torque;
//		if (Torques.ContainsKey (record)) {
//			Torques [record] += torque;
//		} else {
//			Torques.Add (record, torque);
//		}
//	}
//
//	Vector2 GetSupportPoint(Vector2 direction){
//		float bestProjection = float.MinValue;
//		Vector2 bestVertex;
//
//		switch (myCollider.GetType ()) {
//		case typeof(PolygonCollider2D):
//			var poly = myCollider as PolygonCollider2D;
//			return poly.points.OrderByDescending (v => Vector2.Dot (v, direction)).First ();
////			for (int i = 0; i < poly.points.Count; i++) {
////				var v = poly.points [i];
////				float projection = Vector2.Dot (v, direction);
////				if (projection > bestProjection) {
////					bestProjection = projection;
////					bestVertex = v;
////				}
////			}
////			return bestVertex;
//			break;
//		case typeof(CircleCollider2D):
//			var circle = myCollider as CircleCollider2D;
//			return direction.normalized * circle.radius;
//			break;
//		case typeof(BoxCollider2D):
//			var box = myCollider as BoxCollider2D;
//			float x = direction.x > 0 ? box.size.x : -box.size.x;
//			float y = direction.y > 0 ? box.size.y : -box.size.y;
//			return new Vector2(x/2,y/2);
//			break;
//		default:
//			Debug.Log (string.Format ("{0} not supported as a collider. Add support for {0}.", myCollider.GetType ()));
//			return Vector2.zero;
//		}
//	}
//
//	Vector2 ComputePenetration(PhysicsRecord other){
//		switch (other.myCollider.GetType ()) {
//		case typeof(CircleCollider2D):
//			break;
//		case typeof(BoxCollider2D):
//			break;
//		case typeof(PolygonCollider2D):
//			break;
//		default:
//			Debug.Log (string.Format ("{0} not supported as a collider. Add support for {0}.", other.myCollider.GetType ()));
//			return Vector2.zero;
//		}
//	}
//
//	Vector2 ComputePenetration(CircleCollider2D first, CircleCollider2D second){
//		
//	}
//
//	Vector2 ComputePenetration(CircleCollider2D circle, List<Vector2> normals){
//		
//	}
//
//	Vector2 ComputePenetration (List<Vector2> first, List<Vector2> second){
//		
//	}

	protected override void Init ()
	{
		myRigidbody = GetComponent<Rigidbody2D> ();
		myCollider = GetComponent<Collider2D> ();
		contacts = new ContactPoint2D[maxContacts];
		_timeScale = Time.fixedDeltaTime;
		base.Init ();
	}

	protected override void SubscribeEvents ()
	{
//		SMApplication.Subscribe (SMEventTypes.LoadModelData, LoadData);
	}

	protected override void UnsubscribeEvents ()
	{
//		SMApplication.Unsubscribe (SMEventTypes.LoadModelData, LoadData);
	}



//	void UpdateManifolds(Object target, params object[] data){
//		
//	}

//	void OnCollisionEnter2D(Collision2D col){
//		var pr = col.gameObject.GetComponent<PhysicsRecord> ();
//		if (pr != null && !myRecords.ContainsKey (pr)) {
//			myRecords.Add(pr,new Manifold(PhysicsRecordManager.Instance.RecordsPerUpdate,col.contacts));
//		}
//		SMApplication.Notify (SMEventTypes.Collision, null, col);
//	}
//
//	void OnCollisionStay2D(Collision2D col){
//		var pr = col.gameObject.GetComponent<PhysicsRecord> ();
//		Manifold manifold;
//		if (myRecords.TryGetValue (pr, out manifold)) {
//			manifold.UpdateContacts (col.contacts);
//		}
//	}
//
//	void OnCollisionExit2D(Collision2D col){
//		var pr = col.gameObject.GetComponent<PhysicsRecord> ();
//		if (myRecords.ContainsKey(pr)) {
//			myRecords.Remove (pr);
//		}
//	}
}
