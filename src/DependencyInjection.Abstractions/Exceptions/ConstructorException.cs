using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
   // Todo(Nightowl): Implement proper exception practices;
   // Todo(Anyone): Add a default exception message;
   /// <summary>
   /// Represents errors about a given <see cref="Constructor"/>.
   /// </summary>
   [Serializable]
   public class ConstructorException : Exception
   {
      #region Properties
      /// <summary>The <see cref="ConstructorInfo"/> that the exception is related to.</summary>
      public ConstructorInfo Constructor { get; }
      #endregion

      #region Constructors
      /// <summary>Creates a new instance of <see cref="ConstructorException"/> for the given <paramref name="constructor"/>.</summary>
      /// <param name="constructor">The <see cref="ConstructorInfo"/> that this exception will be related to.</param>
      public ConstructorException(ConstructorInfo constructor) => Constructor = constructor;

      /// <summary>
      /// Creates a new instance of <see cref="ConstructorException"/> for the given 
      /// <paramref name="constructor"/> and with the specified <paramref name="message"/>.
      /// </summary>
      /// <param name="constructor">The <see cref="ConstructorInfo"/> that this exception will be related to.</param>
      /// <param name="message">The message that describes the error.</param>
      public ConstructorException(ConstructorInfo constructor, string message) : base(message) => Constructor = constructor;

      /// <summary>Creates a new instance of <see cref="ConstructorException"/> for the given <paramref name="constructor"/>.</summary>
      /// <param name="constructor">The <see cref="ConstructorInfo"/> that this exception will be related to.</param>
      /// <param name="message">The message that describes the error.</param>
      /// <param name="inner">The exception that is the cause of the current exception.</param>
      public ConstructorException(ConstructorInfo constructor, string message, Exception inner) : base(message, inner) => Constructor = constructor;


      // Todo(Anyone): Remove the constructor parameter in this overload;
      /// <summary>Creates a new instance of <see cref="ConstructorException"/> for the given <paramref name="constructor"/>, with serialized data.</summary>
      /// <param name="constructor">The <see cref="ConstructorInfo"/> that this exception will be related to.</param>
      /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
      protected ConstructorException(ConstructorInfo constructor, SerializationInfo info, StreamingContext context) : base(info, context) => Constructor = constructor;
      #endregion
   }
}
