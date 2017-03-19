using System;
using System.Data.Entity;
using System.Linq;

namespace Library.Testing.EntityFramework
{
	public abstract class FakeDbContext
	{
		protected FakeDbContext()
		{
			var t = GetType();

			var iDbSets = from p in t.GetProperties()
						  where p.PropertyType.IsGenericType
						  where p.PropertyType.GetGenericTypeDefinition() == typeof(IDbSet<>)
						  select p;

			foreach (var iDbSet in iDbSets)
			{
				var e = iDbSet.PropertyType.GetGenericArguments()[0];

				var j = typeof(FakeDbSet<>).MakeGenericType(e);

				var fakeDbSet = Activator.CreateInstance(j);

				iDbSet.SetValue(this, fakeDbSet);
			}
		}
	}
}
