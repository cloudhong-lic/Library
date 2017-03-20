using System;
using GreenPipes.Internals.Extensions;

namespace Library.Messaging.MassTransit
{
	public static class MessageHelper
	{
		public static T CreateMessage<T>(Action<T> propertySetter) where T : class
		{
			var implementationType = TypeCache.GetImplementationType(typeof(T));
			var message = (T) Activator.CreateInstance(implementationType);
			propertySetter(message);
			return message;
		}
	}
}