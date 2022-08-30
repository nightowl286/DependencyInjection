using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
	[Serializable]
	public class ParameterException : Exception
	{
      #region Properties
		public ParameterInfo Parameter { get; }
		#endregion

		public ParameterException(ParameterInfo parameter) => Parameter = parameter;
		public ParameterException(ParameterInfo parameter, string message) : base(message) => Parameter = parameter;
		public ParameterException(ParameterInfo parameter, string message, Exception inner) : base(message, inner) => Parameter = parameter;
      protected ParameterException(ParameterInfo parameter, SerializationInfo info, StreamingContext context) : base(info, context) => Parameter = parameter;
   }
}
