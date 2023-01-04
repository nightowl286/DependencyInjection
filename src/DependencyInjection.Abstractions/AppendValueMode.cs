namespace TNO.DependencyInjection.Abstractions;

/// <summary>
/// Represents the different modes of registration.
/// </summary>
public enum AppendValueMode : byte
{
   /// <summary>Specifies that all registration with the specified service type should be replaced.</summary>
   ReplaceAll,

   /// <summary>Specifies that the latest registration with the specified service type should be replaced.</summary>
   ReplaceLatest,

   /// <summary>Specifies that the new registration should be appended along with the current registrations.</summary>
   Append,
}