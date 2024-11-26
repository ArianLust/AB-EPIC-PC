using System;
using System.Runtime.CompilerServices;

namespace Prime31
{
	public class FlurryAndroidManager : AbstractManager
	{
		[method: MethodImpl(32)]
		public static event Action<string> adAvailableForSpaceEvent;

		[method: MethodImpl(32)]
		public static event Action<string> adNotAvailableForSpaceEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onAdClosedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onApplicationExitEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onRenderFailedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> spaceDidFailToReceiveAdEvent;

		[method: MethodImpl(32)]
		public static event Action<string> spaceDidReceiveAdEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onAdClickedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onAdOpenedEvent;

		[method: MethodImpl(32)]
		public static event Action<string> onVideoCompletedEvent;

		static FlurryAndroidManager()
		{
			AbstractManager.initialize(typeof(FlurryAndroidManager));
		}

		public void adAvailableForSpace(string adSpace)
		{
			if (FlurryAndroidManager.adAvailableForSpaceEvent != null)
			{
				FlurryAndroidManager.adAvailableForSpaceEvent(adSpace);
			}
		}

		public void adNotAvailableForSpace(string adSpace)
		{
			if (FlurryAndroidManager.adNotAvailableForSpaceEvent != null)
			{
				FlurryAndroidManager.adNotAvailableForSpaceEvent(adSpace);
			}
		}

		public void onAdClosed(string adSpace)
		{
			if (FlurryAndroidManager.onAdClosedEvent != null)
			{
				FlurryAndroidManager.onAdClosedEvent(adSpace);
			}
		}

		public void onApplicationExit(string adSpace)
		{
			if (FlurryAndroidManager.onApplicationExitEvent != null)
			{
				FlurryAndroidManager.onApplicationExitEvent(adSpace);
			}
		}

		public void onRenderFailed(string adSpace)
		{
			if (FlurryAndroidManager.onRenderFailedEvent != null)
			{
				FlurryAndroidManager.onRenderFailedEvent(adSpace);
			}
		}

		public void spaceDidFailToReceiveAd(string adSpace)
		{
			if (FlurryAndroidManager.spaceDidFailToReceiveAdEvent != null)
			{
				FlurryAndroidManager.spaceDidFailToReceiveAdEvent(adSpace);
			}
		}

		public void spaceDidReceiveAd(string adSpace)
		{
			if (FlurryAndroidManager.spaceDidReceiveAdEvent != null)
			{
				FlurryAndroidManager.spaceDidReceiveAdEvent(adSpace);
			}
		}

		public void onAdClicked(string adSpace)
		{
			FlurryAndroidManager.onAdClickedEvent.fire(adSpace);
		}

		public void onAdOpened(string adSpace)
		{
			FlurryAndroidManager.onAdOpenedEvent.fire(adSpace);
		}

		public void onVideoCompleted(string adSpace)
		{
			FlurryAndroidManager.onVideoCompletedEvent.fire(adSpace);
		}
	}
}
