using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.Common;
using TNO.DependencyInjection.Abstractions;
using TNO.Common.Extensions;

namespace TNO.DependencyInjection
{
   public class TypeCollectionStore<T> : IDisposable where T : notnull
   {
      #region Fields
      [TestOnly]
      internal readonly Dictionary<Type, List<T>> _store = new Dictionary<Type, List<T>>();
      #endregion

      #region Methods
      public bool Contains(Type type) => _store.ContainsKey(type);
      public void Add(Type type, T value, RegistrationMode registrationMode = RegistrationMode.ReplaceAll)
      {
         if (!_store.TryGetValue(type, out List<T>? collection))
         {
            collection = new List<T>();
            _store.Add(type, collection);
         }

         if ((registrationMode == RegistrationMode.ReplaceLatest && collection.Count > 0) 
            || (registrationMode == RegistrationMode.ReplaceAll && collection.Count == 1))
         {
            collection[^1].TryDispose();
            collection[^1] = value;
         }
         else if (registrationMode == RegistrationMode.ReplaceAll && collection.Count > 0)
         {
            foreach (T collectionValue in collection)
               collectionValue.TryDispose();

            collection.Clear();

            collection.Add(value);
         }
         else
            collection.Add(value);
      }
      public IEnumerable<T> GetAll(Type type)
      {
         if (_store.TryGetValue(type, out List<T>? values))
         {
            foreach (T value in values)
               yield return value;
         }
      }
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

      public void Dispose()
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
}
