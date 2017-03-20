using System;

namespace Library.Logging.v0
{
	public interface ILogFactory
	{
		ILog CreateLog(Type type);
		ILog CreateNamedLog(string name);
	}
}