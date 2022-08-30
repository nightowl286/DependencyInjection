using System;
using System.Runtime.Serialization;

namespace TNO.DependencyInjection.Abstractions.Exceptions
{
	[Serializable]
	public class TypeException : Exception
	{
      #region Properties
		Type Type { get; }
		#endregion

		public TypeException(Type type) => Type = type;
		public TypeException(Type type, string message) : base(message) => Type = type;
      public TypeException(Type type, string message, Exception inner) : base(message, inner) => Type = type;
      protected TypeException(Type type, SerializationInfo info, StreamingContext context) : base(info, context) => Type = type;
   }
}
