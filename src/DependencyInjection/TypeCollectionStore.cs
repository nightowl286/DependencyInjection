using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.Common;
using TNO.Common.Disposable;
using TNO.Common.Extensions;
using TNO.DependencyInjection.Abstractions;

namespace TNO.DependencyInjection
{
   // Todo(Anyone): Add an interface for this;
   // Todo(Anyone): Add generic extension methods for this;

   /// <summary>
   /// Represents a collection of keys (<see cref="Type"/>) and one or more values of the type <typeparamref name="T"/>.
   /// </summary>
   /// <typeparam name="T">The type of the values to store.</typeparam>
   public class TypeCollectionStore<T> : DisposableBase where T : notnull
   {
      #region Fields
      [TestOnly(AccessModifiers.Private)]
      internal readonly Dictionary<Type, List<T>> _store = new Dictionary<Type, List<T>>();
      #endregion

      #region Methods
      /// <summary>Checks whether the given <paramref name="type"/> is a key in this collection.</summary>
      /// <param name="type">The <see cref="Type"/> to check.</param>
      /// <returns><see langword="true"/> if the given <paramref name="type"/> is a key, <see langword="false"/> otherwise.</returns>
      public bool Contains(Type type) => _store.ContainsKey(type);

      /// <summary>Adds the specified <paramref name="type"/>/<paramref name="value"/> pair to this collection.</summary>
      /// <param name="type">The type to use as the key.</param>
      /// <param name="value">The value to associate with the given <paramref name="type"/>.</param>
      /// <param name="registrationMode">The registration mode to use which adding the given <paramref name="value"/>.</param>
      public void Add(Type type, T value, AppendValueMode registrationMode = AppendValueMode.ReplaceAll)
      {
         if (!_store.TryGetValue(type, out List<T>? collection))
         {
            collection = new List<T>();
            _store.Add(type, collection);
         }

         if ((registrationMode == AppendValueMode.ReplaceLatest && collection.Count > 0)
            || (registrationMode == AppendValueMode.ReplaceAll && collection.Count == 1))
         {
            collection[^1].TryDispose();
            collection[^1] = value;
         }
         else if (registrationMode == AppendValueMode.ReplaceAll && collection.Count > 0)
         {
            foreach (T collectionValue in collection)
               collectionValue.TryDispose();

            collection.Clear();

            collection.Add(value);
         }
         else
            collection.Add(value);
      }

      /// <summary>Retrieves all the values that are associated with the given <paramref name="type"/>.</summary>
      /// <param name="type">The type to retrieves the values for.</param>
      /// <returns>An enumerable of the values associated with the given <paramref name="type"/>.</returns>
      public IEnumerable<T> GetAll(Type type)
      {
         if (_store.TryGetValue(type, out List<T>? values))
         {
            foreach (T value in values)
               yield return value;
         }
      }

      /// <summary>Tries to get the last value associated with the given <paramref name="type"/>.</summary>
      /// <param name="type">The type to retrieve the <paramref name="value"/> for.</param>
      /// <param name="value">
      /// The last value that was associated wit the given <paramref name="type"/>, 
      /// or <see langword="null"/> if a value could not be obtained.
      /// </param>
      /// <returns><see langword="true"/> if the <paramref name="value"/> could be obtained, <see langword="false"/> otherwise.</returns>
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
}