using System;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
	[Serializable]
	public class ParameterTypeException : Exception
	{
		#region Properties
		public Type Type { get; private set; }
		#endregion

		public ParameterTypeException(Type type) => Type = type;
		public ParameterTypeException(Type type, string message) : base(message) => Type = type;
		public ParameterTypeException(Type type, string message, Exception inner) : base(message, inner) => Type = type;
		protected ParameterTypeException(Type type, SerializationInfo info, StreamingContext context) : base(info, context) => Type = type;
	}
}
