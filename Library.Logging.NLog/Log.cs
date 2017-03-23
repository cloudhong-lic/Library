using System;
using Library.Logging.v0;
using NLog;

namespace Library.Logging.NLog
{
	public class Log : ILog
	{
		private readonly Logger _logger;

		public Log(Logger logger)
		{
			_logger = logger;
		}

		public void Debug(Func<string> logEntry)
		{
			if (_logger.IsDebugEnabled)
				_logger.Debug(logEntry.Invoke());
		}

		public void Debug(Func<string> logEntry, Exception e)
		{
			if (_logger.IsDebugEnabled)
				_logger.Debug(e, logEntry.Invoke());
		}

		public void Error(string logEntry)
		{
			if (_logger.IsErrorEnabled)
				_logger.Error(logEntry);
		}

		public void Error(string logEntry, Exception e)
		{
			if (_logger.IsErrorEnabled)
				_logger.Error(e, logEntry);
		}

		public void Fatal(string logEntry)
		{
			if (_logger.IsFatalEnabled)
				_logger.Fatal(logEntry);
		}

		public void Fatal(string logEntry, Exception e)
		{
			if (_logger.IsFatalEnabled)
				_logger.Fatal(e, logEntry);
		}

		public void Info(Func<string> logEntry)
		{
			if (_logger.IsInfoEnabled)
				_logger.Info(logEntry.Invoke());
		}

		public void Info(string logEntry)
		{
			if (_logger.IsInfoEnabled)
				_logger.Info(logEntry);
		}

		public void Info(string logEntry, Exception e)
		{
			if (_logger.IsInfoEnabled)
				_logger.Info(e, logEntry);
		}

		public void Trace(Func<string> logEntry)
		{
			if (_logger.IsTraceEnabled)
				_logger.Trace(logEntry.Invoke());
		}

		public void Trace(Func<string> logEntry, Exception e)
		{
			if (_logger.IsTraceEnabled)
				_logger.Trace(e, logEntry.Invoke());
		}

		public void Warn(string logEntry)
		{
			if (_logger.IsWarnEnabled)
				_logger.Warn(logEntry);
		}

		public void Warn(string logEntry, Exception e)
		{
			if (_logger.IsWarnEnabled)
				_logger.Warn(e, logEntry);
		}
	}
}