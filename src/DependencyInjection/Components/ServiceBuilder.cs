using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using TNO.Common.Extensions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;
using TNO.DependencyInjection.Explanation;

namespace TNO.DependencyInjection.Components;

internal sealed class ServiceBuilder : IServiceBuilder
{
   #region Fields
   private readonly ServiceContext _context;
   #endregion
   public ServiceBuilder(ServiceContext context)
   {
      _context = context;
   }

   #region Methods
   public ITypeExplanation? Explain(Type type)
   {
      List<IConstructorExplanation> constructorExplanations = new List<IConstructorExplanation>();
      ConstructorInfo[] constructors = type.GetConstructors();
      foreach (ConstructorInfo constructor in constructors)
      {
         IConstructorExplanation? constructorExplanation = ExplainConstructor(constructor);
         if (constructorExplanation is not null)
            constructorExplanations.Add(constructorExplanation);
      }

      if (constructorExplanations.Count == 0 && constructors.Length > 0)
         return null;

      return new TypeExplanation(type, $"The type ({type}) doesn't contain a valid public constructor.", constructorExplanations);
   }
   public object Build(Type type)
   {
      try
      {
         return BuildCore(type);
      }
      catch (Exception)
      {
         ITypeExplanation? explanation = Explain(type);
         if (explanation is null)
            throw;

         throw explanation.ToException();
      }
   }
   public Func<object> BuildDelegate(Type type)
   {
      try
      {
         return BuildDelegateCore(type);
      }
      catch (Exception) // Todo(Anyone): Would be best to have a special internal only exception for this.
      {
         ITypeExplanation? explanation = Explain(type);
         if (explanation is null)
            throw;

         throw explanation.ToException();
      }
   }
   public bool CanBuild(Type type) => TryFindInjectableConstructor(type, out _, out _);
   public void Dispose() { }
   private object BuildCore(Type type)
   {
      if (TryFindInjectableConstructor(type, out ConstructorInfo? injectable, out ConstructorInfo? ambiguousAgainst))
      {
         ParameterInfo[] parameters = injectable.GetParameters();
         object?[] arguments = new object[parameters.Length];
         for (int i = 0; i < parameters.Length; i++)
         {
            ParameterInfo parameter = parameters[i];
            object? argument = BuildParameter(parameter);
            arguments[i] = argument;
         }

         return injectable.Invoke(arguments);
      }
      else if (ambiguousAgainst is not null)
         throw new Exception($"Ambiguous constructors were found on the type ({type}), ({injectable}) is ambiguous against ({ambiguousAgainst}).");
      else
         throw new Exception($"No injectable constructor was found on the type ({type}).");
   }
   private Func<object> BuildDelegateCore(Type type)
   {
      if (TryFindInjectableConstructor(type, out ConstructorInfo? injectable, out ConstructorInfo? ambiguousAgainst))
      {
         ParameterInfo[] parameters = injectable.GetParameters();
         Expression[] arguments = new Expression[parameters.Length];

         for (int i = 0; i < parameters.Length; i++)
         {
            ParameterInfo parameter = parameters[i];
            Expression argument = BuildParameterExpression(parameter);
            arguments[i] = argument;
         }

         Expression createInstance = Expression.New(injectable, arguments);
         Expression<Func<object>> buildExpression = Expression.Lambda<Func<object>>(createInstance, $"BuildDelegate<{type.FullName}>", null);

         return buildExpression.Compile();
      }
      else if (ambiguousAgainst is not null)
         throw new Exception($"Ambiguous constructors were found on the type ({type}), ({injectable}) is ambiguous against ({ambiguousAgainst}).");
      else
         throw new Exception($"No injectable constructor was found on the type ({type}).");
   }
   #endregion

   #region Helpers
   #region Explain
   private IConstructorExplanation? ExplainConstructor(ConstructorInfo constructor)
   {
      if (!constructor.IsPublic)
         return new ConstructorExplanation(constructor, $"The constructor ({constructor}) is not public.", Array.Empty<IParameterExplanation>());

      List<IParameterExplanation> parameterExplanations = new List<IParameterExplanation>();
      foreach (ParameterInfo parameter in constructor.GetParameters())
      {
         IParameterExplanation? parameterExplanation = ExplainParameter(parameter);
         if (parameterExplanation != null)
            parameterExplanations.Add(parameterExplanation);
      }

      if (parameterExplanations.Count == 0)
         return null;

      return new ConstructorExplanation(constructor, $"The constructor ({constructor}) contains invalid parameters.", parameterExplanations);
   }
   private IParameterExplanation? ExplainParameter(ParameterInfo parameter)
   {
      IParameterTypeExplanation? typeExplanation = ExplainParameterType(parameter.ParameterType);

      bool isValidNullable = IsTypeValid(parameter.ParameterType) && parameter.IsNullable();

      if (parameter.IsOut)
         return new ParameterExplanation(parameter, $"Out parameters ({parameter}) are not allowed.", isValidNullable ? null : typeExplanation);

      if (isValidNullable)
         return null;

      if (typeExplanation is not null)
         return new ParameterExplanation(parameter, $"The parameter ({parameter}) type is invalid.", typeExplanation);

      return null;
   }
   private IParameterTypeExplanation? ExplainParameterType(Type type)
   {
      if (!type.IsClass && !type.IsInterface)
         return new ParameterTypeExplanation(type, $"The type ({type}) is neither a class nor an interface.");

      if (_context.Facade.IsRegistered(type))
         return null;

      if (IsEnumerableType(type, out _))
         return null;

      return new ParameterTypeExplanation(type, $"The type ({type}) has not been registered.");
   }
   #endregion
   private object? BuildParameter(ParameterInfo parameter)
   {
      Type parameterType = parameter.ParameterType;
      if (_context.Facade.TryGet(parameterType, out object? instance))
         return instance;

      if (IsEnumerableType(parameterType, out Type? elementType))
      {
         IEnumerable<object> all = _context.Facade.GetAll(elementType);
         return ConvertToEnumerableType(all, parameterType);
      }

      if (parameter.IsNullable())
         return null;

      throw new Exception($"Failed to build the parameter ({parameter}).");
   }
   private Expression BuildParameterExpression(ParameterInfo parameter)
   {
      Type parameterType = parameter.ParameterType;
      if (_context.Facade.IsRegistered(parameterType))
      {
         Expression instance = Expression.Constant(_context.Facade);
         Expression get = Expression.Call(
            instance,
            nameof(_context.Facade.Get),
            null,
            Expression.Constant(parameterType));

         return Expression.TypeAs(get, parameterType);
      }


      throw new Exception();
   }
   private bool TryFindInjectableConstructor(Type type, [NotNullWhen(true)] out ConstructorInfo? injectable, out ConstructorInfo? ambiguousAgainst)
   {
      // Todo(Anyone): Old task, something to do with type checks?;
      ConstructorInfo[] constructors = type.GetConstructors();
      IEnumerable<ConstructorInfo> filtered = FilterInvalidConstructors(constructors);

      return TryFindInjectableConstructor(filtered, out injectable, out ambiguousAgainst);
   }
   private bool TryFindInjectableConstructor(IEnumerable<ConstructorInfo> constructors, [NotNullWhen(true)] out ConstructorInfo? injectable, out ConstructorInfo? ambiguousAgainst)
   {
      injectable = null;
      foreach (ConstructorInfo constructorInfo in constructors)
      {
         ParameterInfo[] parameters = constructorInfo.GetParameters();
         if (AreAllParametersInjectable(parameters))
         {
            if (injectable is null)
               injectable = constructorInfo;
            else
            {
               ambiguousAgainst = constructorInfo;
               return false;
            }
         }
      }

      ambiguousAgainst = null;
      return injectable is not null;
   }
   private bool AreAllParametersInjectable(IEnumerable<ParameterInfo> parameters)
   {
      foreach (ParameterInfo parameter in parameters)
      {
         if (!IsParameterInjectable(parameter))
            return false;
      }
      return true;
   }
   private bool IsParameterInjectable(ParameterInfo parameter)
   {
      Debug.Assert(IsParameterTypeValid(parameter));

      if (parameter.IsNullable())
         return true;

      Type type = parameter.ParameterType;

      return IsTypeInjectable(type);
   }
   private bool IsTypeInjectable(Type type)
   {
      if (_context.Facade.IsRegistered(type))
         return true;

      if (IsEnumerableType(type, out Type? elementType))
         return IsTypeValid(elementType);

      return false;
   }
   private static IEnumerable<ConstructorInfo> FilterInvalidConstructors(IEnumerable<ConstructorInfo> constructors)
   {
      foreach (ConstructorInfo constructor in constructors)
      {
         ParameterInfo[] parameters = constructor.GetParameters();
         if (AreAllParameterTypesValid(parameters))
            yield return constructor;
      }
   }
   private static bool AreAllParameterTypesValid(IEnumerable<ParameterInfo> parameters)
   {
      foreach (ParameterInfo parameter in parameters)
      {
         if (!IsParameterTypeValid(parameter))
            return false;
      }
      return true;
   }
   private static bool IsParameterTypeValid(ParameterInfo parameter)
   {
      if (parameter.IsOut)
         return false;

      Type type = parameter.ParameterType;
      return IsTypeValid(type);
   }
   private static bool IsTypeValid(Type type) => type.IsClass || type.IsInterface;
   private static bool IsEnumerableType(Type type, [NotNullWhen(true)] out Type? elementType)
   {
      if (type.IsArray)
      {
         elementType = type.GetElementType();
         Debug.Assert(elementType is not null);
         return true;
      }

      elementType = null;
      return false;
   }
   private static object ConvertToEnumerableType(IEnumerable<object> instances, Type type)
   {
      if (type.IsArray)
      {
         object[] instancesArray = instances.ToArray();
         Array array = Array.CreateInstance(type.GetElementType()!, instancesArray.Length);

         for (int i = 0; i < instancesArray.Length; i++)
         {
            object inst = instancesArray[i];
            array.SetValue(inst, i);
         }
         return array;
      }

      throw new NotSupportedException($"Can't convert to the enumerable type {type}.");
   }
   #endregion
}