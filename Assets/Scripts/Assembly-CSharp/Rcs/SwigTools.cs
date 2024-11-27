using System;

namespace Rcs
{
	internal static class SwigTools
	{
		public static IdentitySessionBaseSharedPtr MakeIdentitySharedPtr(IdentitySessionBase identitySession)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SwigTools_MakeIdentitySharedPtr(IdentitySessionBase.getCPtr(identitySession));
			return (!(intPtr == IntPtr.Zero)) ? new IdentitySessionBaseSharedPtr(intPtr) : null;
		}

		public static void FreeIdentitySharedPtr(IdentitySessionBaseSharedPtr identitySessionPtrToShared)
		{
			RCSSDKPINVOKE.SwigTools_FreeIdentitySharedPtr(IdentitySessionBaseSharedPtr.getCPtr(identitySessionPtrToShared));
		}

		public static SessionSharedPtr MakeSessionSharedPtr(Session session)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SwigTools_MakeSessionSharedPtr(Session.getCPtr(session));
			return (!(intPtr == IntPtr.Zero)) ? new SessionSharedPtr(intPtr, false) : null;
		}

		public static SessionSharedPtr MakeSessionSharedPtr(IntPtr sessionPtr)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SwigTools_MakeSessionSharedPtr(sessionPtr);
			return (!(intPtr == IntPtr.Zero)) ? new SessionSharedPtr(intPtr, false) : null;
		}

		public static void FreeSessionSharedPtr(SessionSharedPtr ptrToSessionShared)
		{
			RCSSDKPINVOKE.SwigTools_FreeSessionSharedPtr(SessionSharedPtr.getCPtr(ptrToSessionShared));
		}

		public static SessionSharedPtr CopySessionSharedPtr(SessionSharedPtr session)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SwigTools_CopySessionSharedPtr(SessionSharedPtr.getCPtr(session));
			return (!(intPtr == IntPtr.Zero)) ? new SessionSharedPtr(intPtr, true) : null;
		}

		public static IdentitySessionBaseSharedPtr DowncastSessionSharedPtr(SessionSharedPtr session)
		{
			IntPtr intPtr = RCSSDKPINVOKE.SwigTools_DowncastSessionSharedPtr(SessionSharedPtr.getCPtr(session));
			return (!(intPtr == IntPtr.Zero)) ? new IdentitySessionBaseSharedPtr(intPtr) : null;
		}

		public static IntPtr GetSessionPtr(SessionSharedPtr session)
		{
			return RCSSDKPINVOKE.SwigTools_GetSessionPtr(SessionSharedPtr.getCPtr(session));
		}

		public static IntPtr GetIdentitySessionBasePtr(IdentitySessionBaseSharedPtr identitySession)
		{
			return RCSSDKPINVOKE.SwigTools_GetIdentitySessionBasePtr(IdentitySessionBaseSharedPtr.getCPtr(identitySession));
		}
	}
}
