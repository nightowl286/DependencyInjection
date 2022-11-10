using System;
using System.Collections.Generic;
using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   /// <summary>
   /// Denotes an explanation about a <see cref="Constructor"/>.
   /// </summary>
   public interface IConstructorExplanation : IExplanation
   {
      #region Properties
      /// <summary>The <see cref="ConstructorInfo"/> that is explanation is associated with.</summary>
      ConstructorInfo Constructor { get; }

      /// <summary>
      /// A collection of relevant <see cref="IParameterExplanation"/>
      /// for the parameters of the <see cref="Constructor"/>.
      /// </summary>
      IReadOnlyCollection<IParameterExplanation> ParameterExplanations { get; }
      #endregion

      #region Methods
      /// <summary>Converts this explanation into a <see cref="ConstructorException"/>.</summary>
      /// <returns>An instance of <see cref="ConstructorException"/> that contains this explanation's data.</returns>
      new ConstructorException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
