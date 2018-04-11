using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMModel : SMMVC
{
	protected override void SubscribeEvents ()
	{
		SMApplication.Notify (SMEventTypes.OnModelSubscribed, this);
	} 

	protected override void UnsubscribeEvents ()
	{
		SMApplication.Notify (SMEventTypes.OnModelUnsubscribed, this);
	}
}


