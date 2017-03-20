using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace Library.Messaging.MassTransit
{
	public static class AnonymousTypeExtensions
	{
		public static async Task PublishAnonymous<T>(this IPublishEndpoint publishEndpoint, Action<T> propertySetter,
			CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			await publishEndpoint.Publish(MessageHelper.CreateMessage(propertySetter), cancellationToken);
		}

		public static async Task PublishAnonymous<T>(this ConsumeContext context, Action<T> propertySetter,
			CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			await context.Publish(MessageHelper.CreateMessage(propertySetter), cancellationToken);
		}

		public static async Task SendAnonymous<T>(this ISendEndpoint sendEndpoint, Action<T> propertySetter,
			CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			await sendEndpoint.Send(MessageHelper.CreateMessage(propertySetter), cancellationToken);
		}

		public static async Task SendAnonymous<T>(this ConsumeContext context, Uri destinationAddress,
			Action<T> propertySetter)
			where T : class
		{
			await context.Send(destinationAddress, MessageHelper.CreateMessage(propertySetter));
		}

		public static async Task ScheduleSendAnonymous<T>(this ConsumeContext context, Uri destinationAddress,
			DateTime scheduledTime, Action<T> propertySetter, CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			await context.ScheduleSend(destinationAddress, scheduledTime, MessageHelper.CreateMessage(propertySetter),
				cancellationToken);
		}

		public static async Task ScheduleSendAnonymous<T>(this ConsumeContext context, Uri destinationAddress, TimeSpan delay,
			Action<T> propertySetter, CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			await context.ScheduleSend(destinationAddress, delay, MessageHelper.CreateMessage(propertySetter), cancellationToken);
		}

		public static async Task SendAnonymous<T>(this IBus bus, Uri destinationAddress, Action<T> propertySetter,
			CancellationToken cancellationToken = default(CancellationToken))
			where T : class
		{
			var target = await bus.GetSendEndpoint(destinationAddress);
			await target.Send(MessageHelper.CreateMessage(propertySetter), cancellationToken);
		}
	}
}