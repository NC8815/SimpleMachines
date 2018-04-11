using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.Linq;

namespace Quests{

	public enum QuestType{
		Undefined,
		Tutorial,
		Transition,
		Cutscene
	}

	public class QuestManager : Singleton<QuestManager>, ISubscriber {

		public const string _version = "version";
		public const string _from = "from";
		public const string _to = "to";
		public const string _event = "event";
		public const string _target = "target";
		public const string _data = "data";
		public const string _skip = "skippable";
		public const string _steps = "steps";
		public const string _type = "type";
		public const string _name = "name";
		public const string _quests = "quests";
		public const string _noData = "noData";
		public const string _noName = "noName";
		public const string _title = "title";
		public const string _prefabName = "prefabName";
		public const string _duration = "duration";
		public const string _message = "message";

		public TextAsset DefaultQuests;
		private string _questJSON;

		protected List<Quest> questList;

		#region ISubscriber implementation

		void Awake(){
			SubscribeEvents ();
		}

		public override void OnDestroy ()
		{
			UnsubscribeEvents ();
			base.OnDestroy ();
		}

		public void SubscribeEvents ()
		{
			SMApplication.Subscribe (SMEventTypes.QuestTerminated, TerminateQuest);
			SMApplication.Subscribe (SMEventTypes.TutorialMessage, ShowTutorialMessage);
		}

		public void UnsubscribeEvents ()
		{
			SMApplication.Unsubscribe (SMEventTypes.QuestTerminated, TerminateQuest);
			SMApplication.Unsubscribe (SMEventTypes.TutorialMessage, ShowTutorialMessage);
		}

		void ShowTutorialMessage(UnityEngine.Object target, params object[] data){
			if (data.Length > 0) {
				JSONNode D = JSON.Parse (data[0] as string);
				DoozyUI.UIManager.NotificationManager.ShowNotification (D [_prefabName], D [_duration], false, D [_title], D [_message]);
			} else {
				DoozyUI.UIManager.NotificationManager.ShowNotification ("TestNote", -1, false, "Tutorial Data Error", "No Data");
			}
		}

		public void HideTutorialMessage(){
			SMApplication.Notify (SMEventTypes.CloseTutorial);
		}

		void TerminateQuest(UnityEngine.Object target, params object[] data){
			if ((QuestManager)target == this && data.Length > 0 && data [0] is Quest) {
				questList.Remove ((Quest)data [0]);
				SaveQuests ();
			}
		}

		#endregion

		void TestLoad(){
			questList = ValidateQuests (string.Empty)[_quests].GetQuests ();
		}

		public void TestLoad(string questJSON){
			var N = JSON.Parse (questJSON);
			if (N == null || N[_version] == null || N [_version] != SMApplication.Instance.Tuning.version) {
				ResetQuests ();
				N = JSON.Parse (_questJSON);
			}
			questList = N [_quests].GetQuests ();
		}

		public JSONNode ValidateQuests(string questJSON){
			var N = JSON.Parse (questJSON);
			if (N == null) {
				Debug.Log ("N is null.");
				return JSON.Parse (DefaultQuests.text);
			}
			if (N [_version] == null) {
				Debug.Log ("version is null");
				return JSON.Parse (DefaultQuests.text);
			}
			if (N [_version] != SMApplication.Instance.Tuning.version) {
				ResetQuests ();
				Debug.Log ("version mismatch");
				return JSON.Parse (DefaultQuests.text);
			}
			return N;
		}

		void ResetQuests ()
		{
			_questJSON = DefaultQuests.text;
		}

		void InitiateQuests ()
		{
			foreach (Quest quest in questList) {
				quest.ContinueQuest ();
			}
		}

		public void Start(){
			TestLoad ();
			SaveQuests ();
			InitiateQuests ();
		}

		public void SaveQuests(){
//			System.IO.File.WriteAllText (Application.dataPath + "/test.txt", WriteQuests ());
		}

		public string WriteQuests(){
			var result = JSON.Parse ("{}");
			result [_version] = SMApplication.Instance.Tuning.version;
			var quests = result [_quests].AsArray;
			for (int i = 0; i < questList.Count; i++) {
				quests [i].WriteQuest (questList [i]);
			}
			result [_quests] = quests;
			return result.ToString ();
		}

		public IEnumerator SetTimer(TimerData data){
//			Debug.Log ("setting timer for " + data.delay);
			yield return new WaitForSecondsRealtime (data.delay);
//			Debug.Log ("time's up!");
			data.quest.ContinueQuest ();
		}
	}

	public struct TimerData{
		public Quest quest;
		public float delay;

		public TimerData(Quest q, float t){
			quest = q; 
			delay = t;
		}
	}

	public class Quest{
		public Queue<QuestStep> steps { get; private set; }
		public QuestType type { get; private set; }
		public string name { get; private set; }
		QuestStep current = null;

		public Quest(List<QuestStep> s, string t, string n){
			type = Enum.IsDefined (typeof(QuestType), t) ? (QuestType)Enum.Parse (typeof(QuestType), t) : QuestType.Undefined;
			steps = new Queue<QuestStep> (s);
			name = n;
		}

		public void ContinueQuest(){
			ContinueQuest (null);
		}

		bool IsData(string data){
			return data != QuestManager._noData;
		}

		string Parse(params object[] data){
			if (data.Length == 0)
				return QuestManager._noData;
			return data [0].ToString ();
		}

		void ContinueQuest(UnityEngine.Object target, params object[] data){
			if (current != null) {
				if (IsData (current.dataFrom) && current.dataFrom != Parse(data))
					return;
				
				if (current.eventFrom != SMEventTypes.WaitForSeconds) {
					current.Send ();
					SMApplication.Unsubscribe (current.eventFrom, ContinueQuest);
				}

				steps.Dequeue ();
				if (type != QuestType.Tutorial) {
					steps.Enqueue (current);
				}

				QuestManager.Instance.SaveQuests ();
			}

			if(steps.Count>0){
				current = steps.Peek ();
				if (current.eventFrom == SMEventTypes.WaitForSeconds) {
					float delay;
					if (float.TryParse (current.dataTo, out delay))
						QuestManager.Instance.StartCoroutine ("SetTimer", new TimerData (this, delay));
				} else {
					SMApplication.Subscribe (current.eventFrom, ContinueQuest);
				}

				SMApplication.Notify (SMEventTypes.StepComplete);
				return;
			}

			SMApplication.Notify (SMEventTypes.QuestTerminated, QuestManager.Instance, this);
		}
	}

	public class QuestStep{
		public SMEventTypes eventFrom { get; private set; }
		public SMEventTypes eventTo { get; private set; }
		public string dataFrom { get; private set; }
		public string dataTo { get; private set; }
		public bool skippable {get; private set;}

		public QuestStep(string f, string t, string d, bool s){
			eventFrom = Enum.IsDefined(typeof(SMEventTypes),f) ? (SMEventTypes)Enum.Parse(typeof(SMEventTypes),f) : SMEventTypes.FailedSubscription;
			eventTo = Enum.IsDefined(typeof(SMEventTypes),t) ? (SMEventTypes)Enum.Parse(typeof(SMEventTypes),t) : SMEventTypes.FailedNotification;
			dataTo = d;
			skippable = s;
			Debug.Log (ToString ());
		}

		public QuestStep(string fromEvent, string fromData, string toEvent, string toData, bool skippable){
			this.eventFrom = Enum.IsDefined(typeof(SMEventTypes),fromEvent) ? 
				(SMEventTypes)Enum.Parse(typeof(SMEventTypes),fromEvent) : 
				SMEventTypes.FailedSubscription;
			this.eventTo = Enum.IsDefined(typeof(SMEventTypes),toEvent) ? 
				(SMEventTypes)Enum.Parse(typeof(SMEventTypes),toEvent) : 
				SMEventTypes.FailedNotification;
			this.dataFrom = fromData;
			this.dataTo = toData;
//			Debug.Log (toData);
			this.skippable = skippable;
			Debug.Log (ToString ());

		}

		public void Send(){
			SMApplication.Notify (eventTo, null, dataTo);
		}

		public override string ToString ()
		{
			return string.Join (" ", new string[]{ eventFrom.ToString (), eventTo.ToString (), dataFrom, dataTo, skippable.ToString () });
		}
			
	}

	public static class SimpleExtension {

		public static List<Quest> GetQuests(this JSONNode node){
			var result = new List<Quest> ();
			if (node != null && (node is JSONArray)) {
				for (int i = 0; i < node.Count; i++) {
					result.Add (node[i].ReadQuest());
				}
			} else {
				Debug.Log (node.Value + " is null or not an array.");
			}
			return result;
		}

		public static Quest ReadQuest(this JSONNode node){
			if (node.IsObject) {
				string t = (node [QuestManager._type] != null) ? node [QuestManager._type].Value : QuestType.Undefined.ToString ();
				string n = (node [QuestManager._name] != null) ? node [QuestManager._name].Value : QuestManager._noName;
				Debug.Log (string.Join (" ", new string[]{ n, t}));
				List<QuestStep> s = (node [QuestManager._steps] != null) ? node [QuestManager._steps].GetSteps () : new List<QuestStep> ();
//				return new Quest (node [QuestManager.stepsName].GetSteps (), node [QuestManager.typeName], node[QuestManager.nameName]);
				return new Quest(s,t,n);
			} else {
				Debug.Log (node.Value + " is not a JSONObject.");
				return null;
			}
		}

		public static List<QuestStep> GetSteps(this JSONNode node){
			var result = new List<QuestStep> ();
			if (node != null && (node is JSONArray)) {
				for (int i = 0; i < node.Count; i++) {
					result.Add (node [i].ReadStep ());
				}
			}
			return result;
		}
			
		public static QuestStep ReadStep(this JSONNode node){
			JSONObject N = node.AsObject;
			JSONObject F = N [QuestManager._from].AsObject;
//			Debug.Log (F.Value);
			JSONObject T = N [QuestManager._to].AsObject;
//			Debug.Log (F.Value);
			string fromEvent = (F [QuestManager._event] != null) ? 
				F [QuestManager._event].Value : 
				SMEventTypes.StepComplete.ToString();
			string fromData = (F [QuestManager._data] != null) ? 
				F [QuestManager._data].Value : 
				QuestManager._noData;
			string toEvent = (T [QuestManager._event] != null) ? 
				T [QuestManager._event].Value : 
				SMEventTypes.None.ToString ();
			string toData = (T [QuestManager._data] != null) ? 
				T [QuestManager._data].ToString() : 
				QuestManager._noData;
			bool skippable = (node [QuestManager._skip] != null) ? 
				node [QuestManager._skip].AsBool : 
				true;
			return new QuestStep (fromEvent, fromData, toEvent, toData, skippable);
		}

//		public static QuestStep ReadStep(this JSONNode node){
//			if (node.IsObject) {
//				string f = (node [QuestManager._from] != null) ? node [QuestManager._from].Value : SMEventTypes.StepComplete.ToString ();
//				string t = (node [QuestManager._to] != null) ? node [QuestManager._to].Value : SMEventTypes.None.ToString ();
//				string d = (node [QuestManager._data] != null) ? node [QuestManager._data].Value : QuestManager._noData;
//				bool s = (node [QuestManager._skip] != null) ? node [QuestManager._skip].AsBool : true;
//				return new QuestStep (f, t, d, s);
//			} else {
//				Debug.Log (node.Value + " is not a JSONObject.");
//				return null;
//			}
//		}

		public static JSONNode WriteStep(this JSONNode node, QuestStep step){//, string fromName = QuestManager.fromName, string toName = QuestManager.toName, string dataName = QuestManager.dataName, string skipName = QuestManager.skipName){
			var result = node.AsObject;
			result [QuestManager._from][QuestManager._event] = step.eventFrom.ToString ();
			result [QuestManager._from] [QuestManager._data] = step.dataFrom;
			result [QuestManager._to][QuestManager._event] = step.eventTo.ToString ();
			result [QuestManager._to][QuestManager._data] = step.dataTo;
			result [QuestManager._skip].AsBool = step.skippable;
			return result;
		}

		public static JSONNode WriteQuest(this JSONNode node, Quest quest){
			var result = node.AsObject;
			result [QuestManager._type] = quest.type.ToString ();
			result [QuestManager._name] = quest.name;
			var steps = result [QuestManager._steps].AsArray;
			var stepList = new List<QuestStep> (quest.steps);
			for (int i = 0; i < stepList.Count; i++) {
				steps [i].WriteStep (stepList [i]);
			}
			result [QuestManager._steps] = steps;
			return result;
		}
	}

}