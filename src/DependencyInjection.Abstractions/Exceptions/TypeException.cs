using System;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
	/// <summary>
	/// Represents errors about a given <see cref="Type"/>.
	/// </summary>
	[Serializable]
	public class TypeException : Exception
	{
      #region Properties
		/// <summary>The <see cref="System.Type"/> that the exception is related to.</summary>
		public Type Type { get; }
      #endregion

      #region Constructors
      /// <summary>Creates a new instance of <see cref="TypeException"/> for the given <paramref name="type"/>.</summary>
      /// <param name="type">The <see cref="System.Type"/> that this exception will be related to.</param>
      public TypeException(Type type) => Type = type;

      /// <summary>
      /// Creates a new instance of <see cref="TypeException"/> for the given 
      /// <paramref name="type"/> and with the specified <paramref name="message"/>.
      /// </summary>
      /// <param name="type">The <see cref="System.Type"/> that this exception will be related to.</param>
      /// <param name="message">The message that describes the error.</param>
      public TypeException(Type type, string message) : base(message) => Type = type;

      /// <summary>Creates a new instance of <see cref="TypeException"/> for the given <paramref name="type"/>.</summary>
      /// <param name="type">The <see cref="System.Type"/> that this exception will be related to.</param>
      /// <param name="message">The message that describes the error.</param>
      /// <param name="inner">The exception that is the cause of the current exception.</param>
      public TypeException(Type type, string message, Exception? inner) : base(message, inner) => Type = type;

      /// <summary>Creates a new instance of <see cref="TypeException"/> for the given <paramref name="type"/>, with serialized data.</summary>
      /// <param name="type">The <see cref="System.Type"/> that this exception will be related to.</param>
      /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
      protected TypeException(Type type, SerializationInfo info, StreamingContext context) : base(info, context) => Type = type;
      #endregion
   }
}
