using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum SMEventTypes {
	//Target null, data none
	None,
	OnApplicationStart,
	LoadModelData,
	CalculateForces,
	ApplyForces,
	DisplayModelData,
	BeginModelSimulation,
	EndModelSimulation,

	OnControllerSubscribed,
	OnControllerUnsubscribed,
	OnControllerInputAxes,
	OnControllerInputBegin,
	OnControllerInputContinue,
	OnControllerInputEnd,

	//TouchEvents - These are parsed by individual states, who notify their personal version if they are listening. If they aren't listening, they don't pass the message.
	OnPointerDrag, //Touch moves while on this object
	OnPointerDown, //Touch begins on this object
	OnPointerUp, //Touch ends on this object
	OnPointerClick, //Touch ends on this object and began on this object
	OnPointerEnter, //Touch moves onto this object
	OnPointerExit, //Touch moves off of this object

	RotaryMenuUpdate,

	//SetupState TouchEvents - When SetupState is active, if one of the above touchEvents occurs, SetupState releases the corresponding notification. Objects do not respond directly to raw touchEvents
	SetupOnPointerDrag, //Touch moves while on this object
	SetupOnPointerDown, //Touch begins on this object
	SetupOnPointerUp, //Touch ends on this object
	SetupOnPointerClick, //Touch ends on this object and began on this object
	SetupOnPointerEnter, //Touch moves onto this object
	SetupOnPointerExit, //Touch moves off of this object

	OnModelSubscribed,
	OnModelUnsubscribed,
	OnPlayerLoaded,
	OnViewSubscribed,
	OnViewUnsubscribed,
	OnStateSubscribed,
	OnStateUnsubscribed,
	OnStateEnter,
	OnStateExit,
	ChangeNavigationState,
	PlayerMoveInput,

	PDRegisterConstants,
	PDStepTime,
	PDRegisterReactions,
	PDResetTimeStep,
	PDDecayForces,
	PDCullForces,

	OnLevelLoad, //Game receives input to load a level 
	LoadLevel, //Signal Level Loader to actually load the level
	LevelLoadComplete, //Signal rest of the game that level is completely loaded
	LevelReset,
	BeginTutorial,
	Collision,
	CollisionEnd,
	UpdatePhysicsRecords,
	LevelCollisionGoal,
	EndLevel,
	FadeComplete,
	FadeToBlack,
	FadeFromBlack,

	QuestTerminated,
	TutorialMessage,
	CloseTutorial,
	StepComplete,
	WaitForSeconds,
	TestPrint,
	TestTwo,
	TestThree,
	FailedSubscription,
	FailedNotification
}

class SMApplication : Singleton<SMApplication>
{
	public List<SMController> Controllers = new List<SMController>();
	public Tuning Tuning = null;

	#region Event Lists
	public delegate void NotificationHandler(UnityEngine.Object target, params object[] data);
	NotificationHandler Empty = delegate(UnityEngine.Object target, object[] data) {};

	Dictionary<SMEventTypes,NotificationHandler> notifications = new Dictionary<SMEventTypes, NotificationHandler> ();
	#endregion

	#region Initialization
	void Awake(){
		SubscribeEvents ();
	}

	void Start(){
		Notify (SMEventTypes.OnApplicationStart);
	}
	#endregion

	#region Static Methods
	public static void Subscribe(SMEventTypes eventType, NotificationHandler notification){
		Instance._Subscribe (eventType, notification);
	}

	public static void Unsubscribe(SMEventTypes eventType, NotificationHandler notification){
		if (!applicationIsQuitting)
			Instance._Unsubscribe (eventType, notification);
	}

	public static void Notify(SMEventTypes eventType){
		if (!applicationIsQuitting)
			Instance._Notify (eventType, Instance);
	}

	public static void Notify (SMEventTypes eventType, UnityEngine.Object target, params object[] data){
		if (!applicationIsQuitting)
			Instance._Notify (eventType, target, data);
	}
	#endregion

	#region Protected Methods
	public override void OnDestroy ()
	{
		notifications.Clear ();
		base.OnDestroy ();
	} 

	public void _Notify(SMEventTypes eventType){
		Notify (eventType, this);
	}

	protected void _Notify(SMEventTypes eventType, UnityEngine.Object target, params object[] data){
		DoozyUI.UIManager.SendGameEvent (eventType.ToString ());
		NotificationHandler note;
		if (notifications.TryGetValue (eventType, out note) && note != null) {
			Debug.Log (eventType);
			notifications [eventType].Invoke (target, data);
		} else {
			Debug.Log (string.Format ("No one is listening to {0}", eventType.ToString ()));
		}
	}

	protected void _Subscribe(SMEventTypes eventType, NotificationHandler notification){
		if (!notifications.ContainsKey (eventType)) {
			notifications.Add (eventType, Empty);
		}
			notifications [eventType] += notification;
	}

	protected void _Unsubscribe(SMEventTypes eventType, NotificationHandler notification){
		if (notifications.ContainsKey (eventType)) {
			notifications [eventType] -= notification;
		} else {
			print (eventType.ToString() + " not defined in notifications. Unsubscription failed.");
		}
	}

	#endregion

	#region Notification Handlers
	protected void SubscribeEvents(){
		Subscribe (SMEventTypes.OnControllerSubscribed, HandleControllerSubscribed);
		Subscribe (SMEventTypes.OnControllerUnsubscribed, HandleControllerUnsubscribed);
		Subscribe (SMEventTypes.OnStateEnter, HandleStateEnter);
		//test
		Subscribe(SMEventTypes.TestPrint,PrintTest);
	}

	protected void HandleControllerSubscribed(UnityEngine.Object target, params object[] data){
		SMController controller = (SMController)target;
		Controllers.Add (controller);
	}

	protected void HandleControllerUnsubscribed(UnityEngine.Object target, params object[] data){
		Controllers.Remove ((SMController)target);
	}

	protected void HandleStateEnter(UnityEngine.Object target, params object[] data){
//		print (target.name + " changed state to " + target);
	}

	protected void UnsubscribeEvents(){
		Unsubscribe (SMEventTypes.OnControllerSubscribed, HandleControllerSubscribed);
		Unsubscribe (SMEventTypes.OnControllerUnsubscribed, HandleControllerUnsubscribed);
		Unsubscribe (SMEventTypes.OnStateEnter, HandleStateEnter);

		Unsubscribe(SMEventTypes.TestPrint,PrintTest);

	}
	#endregion

	public void Quit(){
		Debug.Log ("Quitting");
		Application.Quit ();
	}

	public void PrintTest(UnityEngine.Object test){
		Debug.Log (test != null);
	}

	void PrintTest(UnityEngine.Object target, params object[] data){
		Debug.Log (data[0]);
	}
}