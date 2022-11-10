using System;
using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   /// <summary>
   /// Denotes an explanation about a <see cref="Parameter"/>.
   /// </summary>
   public interface IParameterExplanation : IExplanation
   {
      #region Properties
      /// <summary>The <see cref="ParameterInfo"/> that this explanation is associated with.</summary>
      ParameterInfo Parameter { get; }

      /// <summary>A possible <see cref="IParameterTypeExplanation"/> about the type of <see cref="Parameter"/>.</summary>
      IParameterTypeExplanation? ParameterTypeExplanation { get; }
      #endregion

      #region Methods
      /// <summary>Converts this explanation into a <see cref="ParameterException"/>.</summary>
      /// <returns>An instance of <see cref="ParameterTypeException"/> that contains this explanation's data.</returns>
      new ParameterException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
