using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rcs
{
	public class AsyncCallInfo<TService> where TService : class
	{
		private List<Delegate> handlers = new List<Delegate>();

		public TService Service { get; private set; }

		public AsyncCallInfo(TService service, params Delegate[] handlers)
		{
			Service = service;
			foreach (Delegate handler in handlers)
			{
				AddHandler(handler);
			}
		}

		public AsyncCallInfo()
		{
		}

		public AsyncCallInfo<TService> AddHandler(Delegate handler)
		{
			if (!(handler is Delegate))
			{
				throw new ArgumentException("handler should be a delegate");
			}
			handlers.Add(handler);
			return this;
		}

		public TDelegate GetHandler<TDelegate>() where TDelegate : class
		{
			foreach (Delegate handler in handlers)
			{
				if (handler.GetType() == typeof(TDelegate))
				{
					return handler as TDelegate;
				}
			}
			throw new Exception("AsyncCallInfo - handler for type '" + typeof(TDelegate).ToString() + "' not found.");
		}

		public IntPtr Pin()
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(this));
		}
	}
}
