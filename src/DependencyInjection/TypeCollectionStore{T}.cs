using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.Common;
using TNO.Common.Disposable;
using TNO.Common.Extensions;
using TNO.DependencyInjection.Abstractions;

namespace TNO.DependencyInjection;

/// <inheritdoc cref="ITypeCollectionStore{T}"/>
public class TypeCollectionStore<T> : DisposableBase, ITypeCollectionStore<T> where T : notnull
{
   #region Fields
   private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

   [TestOnly(AccessModifiers.Private)]
   internal readonly Dictionary<Type, List<T>> _store = new Dictionary<Type, List<T>>();
   #endregion

   #region Methods
   /// <inheritdoc/>
   public bool Contains(Type type)
   {
      _lock.EnterReadLock();
      try
      {
         return _store.ContainsKey(type);
      }
      finally
      {
         _lock.ExitReadLock();
      }
   }

   /// <inheritdoc/>
   public void Add(Type type, T value, AppendValueMode appendValueMode = AppendValueMode.ReplaceAll)
   {
      _lock.EnterWriteLock();
      try
      {
         if (_store.TryGetValue(type, out List<T>? collection) == false)
         {
            collection = new List<T>() { value };
            _store.Add(type, collection);

            return;
         }

         if ((appendValueMode == AppendValueMode.ReplaceLatest && collection.Count > 0)
            || (appendValueMode == AppendValueMode.ReplaceAll && collection.Count == 1))
         {
            collection[^1].TryDispose();
            collection[^1] = value;
         }
         else if (appendValueMode == AppendValueMode.ReplaceAll && collection.Count > 0)
         {
            foreach (T collectionValue in collection)
               collectionValue.TryDispose();

            collection.Clear();

            collection.Add(value);
         }
         else
            collection.Add(value);
      }
      finally
      {
         _lock.ExitWriteLock();
      }
   }

   /// <inheritdoc/>
   public bool TryAdd(Type type, T value)
   {
      _lock.EnterUpgradeableReadLock();
      try
      {
         if (_store.ContainsKey(type))
            return false;

         List<T> collection = new List<T>() { value };
         _store.Add(type, collection);

         return true;
      }
      finally
      {
         _lock.ExitUpgradeableReadLock();
      }
   }

   /// <inheritdoc/>
   public IReadOnlyCollection<T> GetAll(Type type)
   {
      _lock.EnterReadLock();
      try
      {
         if (_store.TryGetValue(type, out List<T>? values))
            return values;

         return Array.Empty<T>();
      }
      finally
      {
         _lock.ExitReadLock();
      }
   }

   /// <inheritdoc/>
   public IReadOnlyCollection<T> GetAllValues()
   {
      _lock.EnterReadLock();
      try
      {
         List<T> allValues = new List<T>();
         foreach (List<T> values in _store.Values)
         {
            foreach (T value in values)
               allValues.Add(value);
         }

         return allValues;
      }
      finally
      {
         _lock.ExitReadLock();
      }
   }

   /// <inheritdoc/>
   public bool TryGet(Type type, [NotNullWhen(true)] out T? value)
   {
      _lock.EnterReadLock();
      try
      {
         if (_store.TryGetValue(type, out List<T>? values))
         {
            if (values.Count > 0)
            {
               value = values[^1];
               return true;
            }
         }
      }
      finally
      {
         _lock.ExitReadLock();
      }

      value = default;
      return false;
   }

   /// <inheritdoc/>
   public IReadOnlyCollection<Type> GetTypes()
   {
      _lock.EnterReadLock();
      try
      {
         return _store.Keys.ToList();
      }
      finally
      {
         _lock.ExitReadLock();
      }
   }

   /// <inheritdoc/>
   protected override void DisposeManaged()
   {
      _lock.EnterWriteLock();
      try
      {
         foreach (List<T> collection in _store.Values)
         {
            Debug.Assert(collection is not null);

            foreach (T value in collection)
               value.TryDispose();
         }

         _store.Clear();
      }
      finally
      {
         _lock.ExitWriteLock();
      }
   }
   #endregion
}