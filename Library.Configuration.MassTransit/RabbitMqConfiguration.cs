using System;
using System.Configuration;

namespace Library.Configuration.MassTransit
{
	public static class RabbitMqConfiguration
	{
		public static Uri HostAddress
		{
			get
			{
				var hostAddress = ConfigurationManager.AppSettings["RabbitMqHostAddress"];
				if (string.IsNullOrEmpty(hostAddress))
					hostAddress = "rabbitmq://localhost/"; // fall back to localhost if not configured explicitly
				return new Uri(hostAddress);
			}
		}

		public static string Username
		{
			get
			{
				var username = ConfigurationManager.AppSettings["RabbitMqUsername"];
				if (string.IsNullOrEmpty(username))
					username = "guest";
				return username;
			}
		}

		public static string Password
		{
			get
			{
				var password = ConfigurationManager.AppSettings["RabbitMqPassword"];
				if (string.IsNullOrEmpty(password))
					password = "guest";
				return password;
			}
		}
	}
}