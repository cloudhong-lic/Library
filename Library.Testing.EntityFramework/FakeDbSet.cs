using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Library.Testing.EntityFramework
{
	public class FakeDbSet<T> : IDbSet<T>, IDbAsyncEnumerable<T> where T : class
	{
		private readonly HashSet<T> _data;
		private readonly IQueryable _query;
		private int _identity = 1;
		private List<PropertyInfo> _keyProperties;

		public delegate void DbChangeEventHandler(object sender, T row);

		public event DbChangeEventHandler OnInsert;
		public event DbChangeEventHandler OnDelete;

		private void GetKeyProperties()
		{
			_keyProperties = new List<PropertyInfo>();
			var alternatives = new List<PropertyInfo>();
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (property.GetCustomAttributes(true).OfType<KeyAttribute>().Any())
					_keyProperties.Add(property);
				else if ((property.Name == typeof(T).Name + "Id") || (property.Name == "Id"))
					alternatives.Add(property);
			}
			if (!_keyProperties.Any() && alternatives.Count == 1)
				_keyProperties.Add(alternatives.First());
		}

		private void GenerateId(T entity)
		{
			// If non-composite integer key
			if (_keyProperties.Count == 1 && _keyProperties[0].PropertyType == typeof(Int32))
			{
				var currentId = (int)_keyProperties[0].GetValue(entity, null);
				if (currentId == 0)
					_keyProperties[0].SetValue(entity, _identity++, null);
			}
		}

		public FakeDbSet()
			: this(new T[] { })
		{

		}
		public FakeDbSet(IEnumerable<T> startData)
		{
			GetKeyProperties();
			_data = (startData != null ? new HashSet<T>(startData) : new HashSet<T>());
			_query = _data.AsQueryable();
		}

		public virtual T Find(params object[] keyValues)
		{
			if (keyValues.Length != _keyProperties.Count)
				throw new ArgumentException("Incorrect number of keys passed to find method");

			IQueryable<T> keyQuery = this.AsQueryable<T>();
			for (int i = 0; i < keyValues.Length; i++)
			{
				var x = i; // nested linq
				keyQuery = keyQuery.Where(entity => _keyProperties[x].GetValue(entity, null).Equals(keyValues[x]));
			}

			return keyQuery.SingleOrDefault();
		}

		public T Add(T item)
		{
			GenerateId(item);
			_data.Add(item);

			var onInsert = OnInsert;
			if (onInsert != null)
				onInsert(this, item);

			return item;
		}

		public T Remove(T item)
		{
			_data.Remove(item);

			var onDelete = OnDelete;
			if (onDelete != null)
				onDelete(this, item);

			return item;
		}

		public T Attach(T item)
		{
			_data.Add(item);
			return item;
		}

		public void Detach(T item)
		{
			_data.Remove(item);
		}

		Type IQueryable.ElementType
		{
			get { return _query.ElementType; }
		}

		Expression IQueryable.Expression
		{
			get { return _query.Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return new TestDbAsyncQueryProvider<T>(_query.Provider); }
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public T Create()
		{
			return Activator.CreateInstance<T>();
		}

		public ObservableCollection<T> Local
		{
			get
			{
				return new ObservableCollection<T>(_data);
			}
		}

		public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
		{
			return Activator.CreateInstance<TDerivedEntity>();
		}

		public IDbAsyncEnumerator<T> GetAsyncEnumerator()
		{
			return new TestDbAsyncEnumerator<T>(_data.GetEnumerator());
		}

		IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
		{
			return this.GetAsyncEnumerator();
		}
	}
}
