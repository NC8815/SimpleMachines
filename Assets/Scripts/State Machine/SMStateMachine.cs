using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMStateMachine : SMModel
{
	public virtual SMState CurrentState {
		get { return _currentState; }
		set { Transition (value); }
	}
	protected SMState _currentState;
	protected bool _inTransition;

	public virtual T GetState<T>() where T : SMState{
		T target = GetComponent<T> ();
		if (target == null)
			target = gameObject.AddComponent<T> ();
		return target;
	}

	public virtual SMState GetState(System.Type type){
		Component target = GetComponent (type);
		if (target == null)
			target = gameObject.AddComponent (type);
		return (SMState)target;
	}

	public virtual void ChangeState<T>() where T : SMState{
		CurrentState = GetState<T> ();
	}

	public virtual void ChangeState(System.Type type){
		CurrentState = GetState (type);
	}
		
	protected virtual void Transition(SMState value){
		if (_currentState == value || _inTransition)
			return;

		_inTransition = true;

		if (_currentState != null)
			_currentState.Exit ();

		_currentState = value;

		if (_currentState != null)
			_currentState.Enter ();

		_inTransition = false;
	}

	protected override void Init ()
	{
	}
}
