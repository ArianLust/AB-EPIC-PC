using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Prime31
{
	public class GoogleCloudMessagingManager : AbstractManager
	{
		[method: MethodImpl(32)]
		public static event Action<Dictionary<string, object>> notificationReceivedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> registrationSucceededEvent;

		[method: MethodImpl(32)]
		public static event Action<string> registrationFailedEvent;

		[method: MethodImpl(32)]
		public static event Action unregistrationSucceededEvent;

		[method: MethodImpl(32)]
		public static event Action<string> unregistrationFailedEvent;

		static GoogleCloudMessagingManager()
		{
			AbstractManager.initialize(typeof(GoogleCloudMessagingManager));
		}

		public void notificationReceived(string json)
		{
			GoogleCloudMessagingManager.notificationReceivedEvent.fire(json.dictionaryFromJson());
		}

		public void registrationSucceeded(string registrationId)
		{
			GoogleCloudMessagingManager.registrationSucceededEvent.fire(registrationId);
		}

		public void unregistrationFailed(string param)
		{
			GoogleCloudMessagingManager.unregistrationFailedEvent.fire(param);
		}

		public void registrationFailed(string error)
		{
			GoogleCloudMessagingManager.registrationFailedEvent.fire(error);
		}

		public void unregistrationSucceeded(string empty)
		{
			GoogleCloudMessagingManager.unregistrationSucceededEvent.fire();
		}
	}
}
