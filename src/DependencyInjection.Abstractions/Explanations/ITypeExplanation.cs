using System;
using System.Collections.Generic;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   /// <summary>
   /// Denotes an explanation about a <see cref="Type"/>.
   /// </summary>
   public interface ITypeExplanation : IExplanation
   {
      #region Properties
      /// <summary>The <see cref="System.Type"/> that this explanation is associated with.</summary>
      Type Type { get; }

      /// <summary>
      /// A collection of relevant <see cref="IConstructorExplanation"/>
      /// for the constructors of the <see cref="Type"/>.
      /// </summary>
      IReadOnlyCollection<IConstructorExplanation> ConstructorExplanations { get; }
      #endregion

      #region Methods
      /// <summary>Converts this explanation into a <see cref="TypeException"/>.</summary>
      /// <returns>An instance of <see cref="TypeException"/> that contains this explanation's data.</returns>
      new TypeException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
