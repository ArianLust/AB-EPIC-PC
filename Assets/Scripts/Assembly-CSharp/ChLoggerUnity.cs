using System;

internal class ChLoggerUnity : IChLogger
{
	public void Info(Type tag, string message, params object[] args)
	{
		DebugLog.Log(tag, string.Format(message, args));
	}

	public void Warn(Type tag, string message, params object[] args)
	{
		DebugLog.Warn(tag, string.Format(message, args));
	}

	public void Error(Type tag, string message, params object[] args)
	{
		DebugLog.Error(tag, string.Format(message, args));
	}
}
