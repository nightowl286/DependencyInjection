using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
	[Serializable]
	public class ConstructorException : Exception
	{
		#region Properties
		public ConstructorInfo Constructor { get; }
		#endregion

		public ConstructorException(ConstructorInfo constructor) => Constructor = constructor;
		public ConstructorException(ConstructorInfo constructor, string message) : base(message) => Constructor = constructor;
		public ConstructorException(ConstructorInfo constructor, string message, Exception inner) : base(message, inner) => Constructor = constructor;
		protected ConstructorException(ConstructorInfo constructor, SerializationInfo info, StreamingContext context) : base(info, context) => Constructor = constructor;
	}
}
