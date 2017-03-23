using System;
using Library.Logging.v0;
using NLog;

namespace Library.Logging.NLog
{
	public class LogFactory : ILogFactory
	{
		public ILog CreateLog(Type type)
		{
			return new Log(LogManager.GetLogger(type.FullName));
		}

		public ILog CreateNamedLog(string name)
		{
			return new Log(LogManager.GetLogger(name));
		}
	}
}