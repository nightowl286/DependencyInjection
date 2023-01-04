using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions;

/// <summary>
/// Represents errors about a given <see cref="Parameter"/>.
/// </summary>
[Serializable]
public class ParameterException : Exception
{
   #region Properties
   /// <summary>The <see cref="ParameterInfo"/> that the exception is related to.</summary>
   public ParameterInfo Parameter { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of <see cref="ParameterException"/> for the given <paramref name="parameter"/>.</summary>
   /// <param name="parameter">The <see cref="ParameterInfo"/> that this exception will be related to.</param>
   public ParameterException(ParameterInfo parameter) => Parameter = parameter;

   /// <summary>
   /// Creates a new instance of <see cref="ParameterException"/> for the given 
   /// <paramref name="parameter"/> and with the specified <paramref name="message"/>.
   /// </summary>
   /// <param name="parameter">The <see cref="ParameterInfo"/> that this exception will be related to.</param>
   /// <param name="message">The message that describes the error.</param>>
   public ParameterException(ParameterInfo parameter, string message) : base(message) => Parameter = parameter;

   /// <summary>Creates a new instance of <see cref="ParameterException"/> for the given <paramref name="parameter"/>.</summary>
   /// <param name="parameter">The <see cref="ParameterInfo"/> that this exception will be related to.</param>
   /// <param name="message">The message that describes the error.</param>
   /// <param name="inner">The exception that is the cause of the current exception.</param>
   public ParameterException(ParameterInfo parameter, string message, Exception inner) : base(message, inner) => Parameter = parameter;

   /// <summary>Creates a new instance of <see cref="ParameterException"/> for the given <paramref name="parameter"/>, with serialized data.</summary>
   /// <param name="parameter">The <see cref="ParameterInfo"/> that this exception will be related to.</param>
   /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
   /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
   protected ParameterException(ParameterInfo parameter, SerializationInfo info, StreamingContext context) : base(info, context) => Parameter = parameter;
   #endregion
}