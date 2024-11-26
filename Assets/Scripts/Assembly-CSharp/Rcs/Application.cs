using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rcs
{
	public class Application : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		private bool disposed;

		public static string ServerProduction
		{
			get
			{
				return RCSSDKPINVOKE.Application_ServerProduction_get();
			}
		}

		public static string ServerStaging
		{
			get
			{
				return RCSSDKPINVOKE.Application_ServerStaging_get();
			}
		}

		public static string ServerDevelopment
		{
			get
			{
				return RCSSDKPINVOKE.Application_ServerDevelopment_get();
			}
		}

		internal Application(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		public Application()
			: this(RCSSDKPINVOKE.new_Application(), true)
		{
		}

		internal static IntPtr getCPtr(Application obj)
		{
			return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				_DisposeUnmanaged();
				disposed = true;
			}
		}

		~Application()
		{
			Dispose(false);
		}

		private void _DisposeUnmanaged()
		{
			lock (this)
			{
				if (swigCPtr != IntPtr.Zero)
				{
					if (swigCMemOwn)
					{
						swigCMemOwn = false;
						RCSSDKPINVOKE.delete_Application(swigCPtr);
					}
					swigCPtr = IntPtr.Zero;
				}
			}
		}

		public static void SetRequestTimeout(int connectionTimeoutMillis)
		{
			RCSSDKPINVOKE.Application_SetRequestTimeout(connectionTimeoutMillis);
		}

		public static void Initialize(string publisherName, string productName)
		{
			RCSSDKPINVOKE.Application_Initialize_0(publisherName, productName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static void Initialize(string publisherName)
		{
			RCSSDKPINVOKE.Application_Initialize_1(publisherName);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static void Initialize()
		{
			RCSSDKPINVOKE.Application_Initialize_2();
		}

		public static void InitializeWithPath(string absolutePath)
		{
			RCSSDKPINVOKE.Application_InitializeWithPath_0(absolutePath);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static void InitializeWithPath()
		{
			RCSSDKPINVOKE.Application_InitializeWithPath_1();
		}

		public static void Update()
		{
			RCSSDKPINVOKE.Application_Update();
		}

		public static void Activate()
		{
			RCSSDKPINVOKE.Application_Activate();
		}

		public static void Suspend()
		{
			RCSSDKPINVOKE.Application_Suspend();
		}

		public static void UrlOpened(string url, string sourceApplication)
		{
			RCSSDKPINVOKE.Application_UrlOpened_0(url, sourceApplication);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static void UrlOpened(string url)
		{
			RCSSDKPINVOKE.Application_UrlOpened_1(url);
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}

		public static void Destroy()
		{
			RCSSDKPINVOKE.Application_Destroy();
		}

		public static void SetLogger(RCSSDK.Logger arg0)
		{
			RCSSDKPINVOKE.Application_SetLogger(Marshal.GetFunctionPointerForDelegate(arg0));
		}

		public static void EnableInternalLogger()
		{
			RCSSDKPINVOKE.Application_EnableInternalLogger();
		}

		public static void DisableInternalLogger()
		{
			RCSSDKPINVOKE.Application_DisableInternalLogger();
		}

		public static void OverwriteServiceConfiguration(string name, string url, Dictionary<string, string> parameters)
		{
			RCSSDKPINVOKE.Application_OverwriteServiceConfiguration(name, url, StringDict.getCPtr(parameters.ToSwigDict()));
			if (RCSSDK.SWIGPendingException.Pending)
			{
				throw RCSSDK.SWIGPendingException.Retrieve();
			}
		}
	}
}
