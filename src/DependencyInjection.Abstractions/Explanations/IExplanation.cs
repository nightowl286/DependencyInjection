using System;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   /// <summary>
   /// Denotes the common members an explanation requires.
   /// </summary>
   public interface IExplanation
   {
      #region Properties
      /// <summary>The explanation string that should be shown to the developer.</summary>
      string Explanation { get; }
      #endregion

      #region Methods
      /// <summary>Convert this explanation's data into a suitable <see cref="Exception"/>.</summary>
      /// <returns>An <see cref="Exception"/> that contains this explanation's data.</returns>
      Exception ToException();
      #endregion
   }
}
