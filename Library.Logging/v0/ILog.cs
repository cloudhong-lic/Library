using System;

namespace Library.Logging.v0
{
	public interface ILog
	{
		void Debug(Func<string> logEntry);
		void Debug(Func<string> logEntry, Exception e);

		void Error(string logEntry);
		void Error(string logEntry, Exception e);

		void Fatal(string logEntry);
		void Fatal(string logEntry, Exception e);

		void Info(Func<string> logEntry);
		void Info(string logEntry);
		void Info(string logEntry, Exception e);

		void Trace(Func<string> logEntry);
		void Trace(Func<string> logEntry, Exception e);

		void Warn(string logEntry);
		void Warn(string logEntry, Exception e);
	}
}