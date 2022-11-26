using System;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
   /// <summary>
   /// The exception that is thrown when a registrar instance is locked.
   /// </summary>
   [Serializable]
   public class RegistrarLockedException : Exception
   {
      #region Constructors
      /// <summary>Creates a new instance of <see cref="RegistrarLockedException"/>.</summary>
      public RegistrarLockedException() : base("This registrar is locked.") { }

      /// <summary>Creates a new instance of <see cref="RegistrarLockedException"/> with the given <paramref name="message"/>.</summary>
      /// <param name="message">The message that describes the error.</param>
      public RegistrarLockedException(string message) : base(message) { }

      /// <summary>Creates a new instance of <see cref="RegistrarLockedException"/> with the given <paramref name="message"/>.</summary>
      /// <param name="message">The message that describes the error.</param>
      /// <param name="inner">The exception that is the cause of the current exception.</param>
      public RegistrarLockedException(string message, Exception inner) : base(message, inner) { }

      /// <summary>Creates a new instance of <see cref="RegistrarLockedException"/>.</summary>
      /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
      protected RegistrarLockedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
      #endregion
   }
}
