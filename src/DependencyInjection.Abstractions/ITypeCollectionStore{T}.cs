using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TNO.DependencyInjection.Abstractions;

/// <summary>
/// Denotes a collection of keys (<see cref="Type"/>) and one or more values of the type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the values to store.</typeparam>
public interface ITypeCollectionStore<T> : IDisposable where T : notnull
{
   #region Methods
   /// <summary>Checks whether the given <paramref name="type"/> is a key in this collection.</summary>
   /// <param name="type">The <see cref="Type"/> to check.</param>
   /// <returns><see langword="true"/> if the given <paramref name="type"/> is a key, <see langword="false"/> otherwise.</returns>
   bool Contains(Type type);

   /// <inheritdoc cref="Contains(Type)"/>
   /// <typeparam name="U">The type to check.</typeparam>
   bool Contains<U>() => Contains(typeof(U));

   /// <summary>Adds the specified <paramref name="type"/>/<paramref name="value"/> pair to this collection.</summary>
   /// <param name="type">The type to use as the key.</param>
   /// <param name="value">The value to associate with the given <paramref name="type"/>.</param>
   /// <param name="appendValueMode">The mode to use when adding the given <paramref name="value"/>.</param>
   void Add(Type type, T value, AppendValueMode appendValueMode = AppendValueMode.ReplaceAll);

   /// <inheritdoc cref="Add(Type, T, AppendValueMode)"/>
   /// <typeparam name="U">The type to use as the key.</typeparam>
   void Add<U>(T value, AppendValueMode appendValueMode = AppendValueMode.ReplaceAll) => Add(typeof(U), value, appendValueMode);

   /// <summary>Retrieves all the values that are associated with the given <paramref name="type"/>.</summary>
   /// <param name="type">The type to retrieves the values for.</param>
   /// <returns>An enumerable of the values associated with the given <paramref name="type"/>.</returns>
   IEnumerable<T> GetAll(Type type);

   /// <inheritdoc cref="GetAll(Type)"/>
   /// <typeparam name="U">The type to retrieve the values for.</typeparam>
   IEnumerable<T> GetAll<U>() => GetAll(typeof(U));

   /// <summary>Retrieves all the values that are stored in this collection.</summary>
   /// <returns>An enumerable of all the stored values.</returns>
   IEnumerable<T> GetAllValues();

   /// <summary>Tries to get the last value associated with the given <paramref name="type"/>.</summary>
   /// <param name="type">The type to retrieve the <paramref name="value"/> for.</param>
   /// <param name="value">
   /// The last value that was associated wit the given <paramref name="type"/>, 
   /// or <see langword="null"/> if a value could not be obtained.
   /// </param>
   /// <returns><see langword="true"/> if the <paramref name="value"/> could be obtained, <see langword="false"/> otherwise.</returns>
   bool TryGet(Type type, [NotNullWhen(true)] out T? value);

   /// <inheritdoc cref="TryGet(Type, out T?)"/>
   /// <typeparam name="U">The type to retrieve the <paramref name="value"/> for.</typeparam>
   bool TryGet<U>([NotNullWhen(true)] out T? value) => TryGet(typeof(U), out value);

   /// <summary>Get all the types that are used as keys.</summary>
   /// <returns>An enumerable of the types.</returns>
   IEnumerable<Type> GetTypes();
   #endregion
}