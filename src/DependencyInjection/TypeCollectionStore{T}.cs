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
   [TestOnly(AccessModifiers.Private)]
   internal readonly Dictionary<Type, List<T>> _store = new Dictionary<Type, List<T>>();
   #endregion

   #region Methods
   /// <inheritdoc/>
   public bool Contains(Type type) => _store.ContainsKey(type);

   /// <inheritdoc/>
   public void Add(Type type, T value, AppendValueMode appendValueMode = AppendValueMode.ReplaceAll)
   {
      if (!_store.TryGetValue(type, out List<T>? collection))
      {
         collection = new List<T>();
         _store.Add(type, collection);
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

   /// <inheritdoc/>
   public IEnumerable<T> GetAll(Type type)
   {
      if (_store.TryGetValue(type, out List<T>? values))
      {
         foreach (T value in values)
            yield return value;
      }
   }

   /// <inheritdoc/>
   public IEnumerable<T> GetAllValues()
   {
      foreach (List<T> values in _store.Values)
      {
         foreach (T value in values)
            yield return value;
      }
   }

   /// <inheritdoc/>
   public bool TryGet(Type type, [NotNullWhen(true)] out T? value)
   {
      if (_store.TryGetValue(type, out List<T>? values))
      {
         if (values.Count > 0)
         {
            value = values[^1];
            return true;
         }
      }

      value = default;
      return false;
   }

   /// <inheritdoc/>
   public IEnumerable<Type> GetTypes() => _store.Keys;

   /// <inheritdoc/>
   protected override void DisposeManaged()
   {
      foreach (List<T> collection in _store.Values)
      {
         Debug.Assert(collection is not null);

         foreach (T value in collection)
            value.TryDispose();
      }

      _store.Clear();
   }

   #endregion
}