using System;
using System.Text;
using GreenPipes;
using MassTransit;
using MassTransit.NewIdFormatters;
using MassTransit.RabbitMqTransport;

namespace Library.Configuration.MassTransit
{
	/// <summary>
	///     Setup MassTransit RabbitMq bus using configuration from the appSettings
	/// </summary>
	public static class RabbitMqSetupExtensions
	{
		private static readonly INewIdFormatter _formatter = new ZBase32Formatter();

		public static IRabbitMqHost SetupRabbitMqHostFromConfig(this IRabbitMqBusFactoryConfigurator configurator)
		{
			configurator.PrefetchCount = MassTransitConfiguration.PrefetchCount;
			var host = configurator.Host(RabbitMqConfiguration.HostAddress, h =>
			{
				h.Username(RabbitMqConfiguration.Username);
				h.Password(RabbitMqConfiguration.Password);
			});

			return host;
		}

		public static void ApplySettingsFromConfig(this IRabbitMqReceiveEndpointConfigurator configurator)
		{
			configurator.UseConcurrencyLimit(MassTransitConfiguration.ConcurrencyLimit);
			configurator.PrefetchCount = MassTransitConfiguration.PrefetchCount;
		}


		/// <summary>
		///     Make up a temporary queue name based on the given prefix,
		///     in the format of prefix-machinename-base64guid
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public static string ToTemporaryQueueName(this string prefix)
		{
			var sb = new StringBuilder(prefix);
			sb.Append('-');
			var machineName = Environment.MachineName;
			foreach (var c in machineName)
				if (char.IsLetterOrDigit(c))
					sb.Append(c);
				else if (c == '.' || c == '_' || c == '-' || c == ':')
					sb.Append(c);
			sb.Append('-');
			sb.Append(NewId.Next().ToString(_formatter));

			return sb.ToString();
		}

		/// <summary>
		///     Converts simple queue name to fully qualified Uri that MassTransit requires
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static Uri ToRabbitMqDestinationAddress(this string queueName)
		{
			return new Uri(RabbitMqConfiguration.HostAddress, queueName);
		}
	}
}