using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMView : SMMVC
{
	protected override void SubscribeEvents ()
	{
		SMApplication.Notify (SMEventTypes.OnViewSubscribed, this);
	} 

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Notify (SMEventTypes.OnViewUnsubscribed, this);
	}
}


