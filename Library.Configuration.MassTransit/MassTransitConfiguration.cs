using System.Configuration;

namespace Library.Configuration.MassTransit
{
	public static class MassTransitConfiguration
	{
		public static int ConcurrencyLimit
		{
			get
			{
				string concurrencyLimit = ConfigurationManager.AppSettings["MassTransitConcurrencyLimit"];
				int value;
				if (int.TryParse(concurrencyLimit, out value) && value > 0)
					return value;
				return 1;
			}
		}

		public static ushort PrefetchCount
		{
			get
			{
				string prefetchCount = ConfigurationManager.AppSettings["MassTransitPrefetchCount"];
				ushort value;
				if (ushort.TryParse(prefetchCount, out value) && value > 0)
					return value;
				return 8;
			}
		}
	}
}