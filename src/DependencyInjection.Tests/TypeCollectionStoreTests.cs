using Moq;
using TNO.DependencyInjection.Abstractions;

namespace TNO.DependencyInjection.Tests;

[TestClass]
[TestCategory(Category.DependencyInjection)]
public class TypeCollectionStoreTests
{
   #region Fields
   private readonly TypeCollectionStore<object> _sut;
   #endregion

   #region Properties
   public static IEnumerable<object[]> AllRegistrationModes => DynamicDataProviders.GetAllRegistrationModes();
   #endregion
   public TypeCollectionStoreTests()
   {
      _sut = new TypeCollectionStore<object>();
   }

   [DynamicData(nameof(AllRegistrationModes))]
   [TestMethod]
   public void Add_UniqueValue_WithRegistrationMode_ProperlyStored(AppendValueMode registrationMode)
   {
      // Arrange
      object value = new object();
      Type type = typeof(object);

      // Act
      _sut.Add(type, value, registrationMode);

      // Assert
      Assert.IsTrue(_sut._store.TryGetValue(type, out List<object>? collection));
      Assert.That.AreEqual(1, collection.Count);
      Assert.AreSame(value, collection[0]);
   }

   [TestMethod]
   public void Add_ReplaceLatest_WithMultipleItems_ProperlyReplaced()
   {
      // Arrange
      Mock<IDisposable> second = new Mock<IDisposable>();

      object
         first = new object(),
         third = new object();
      Type type = typeof(object);

      _sut.Add(type, first, AppendValueMode.Append);
      _sut.Add(type, second.Object, AppendValueMode.Append);

      // Arrange Assert
      Assert.That.IsInconclusiveIfNot(_sut.TryGet(type, out object? lastStored));
      Assert.That.IsInconclusiveIfNot(ReferenceEquals(second.Object, lastStored));

      // Act
      _sut.Add(type, third, AppendValueMode.ReplaceLatest);

      // Assert
      Assert.IsTrue(_sut.TryGet(type, out lastStored));
      Assert.IsTrue(_sut._store[type].Count == 2);
      Assert.AreSame(third, lastStored);
      second.VerifyDisposed();
   }

   [TestMethod]
   public void Add_ReplaceAll_WithMultipleItems_ProperlyReplaced()
   {
      // Arrange
      Mock<IDisposable>
         first = new Mock<IDisposable>(),
         second = new Mock<IDisposable>();

      object third = new object();
      Type type = typeof(object);

      _sut.Add(type, first.Object, AppendValueMode.Append);
      _sut.Add(type, second.Object, AppendValueMode.Append);

      // Arrange Assert
      Assert.That.IsInconclusiveIfNot(_sut.Contains(type));
      Assert.That.IsInconclusiveIf(_sut._store[type].Count != 2);

      // Act
      _sut.Add(type, third, AppendValueMode.ReplaceAll);

      // Assert
      Assert.IsTrue(_sut.TryGet(type, out object? lastStored));
      Assert.IsTrue(_sut._store[type].Count == 1);
      Assert.AreSame(third, lastStored);
      first.VerifyDisposed();
      second.VerifyDisposed();
   }

   [TestMethod]
   public void TryGet_WithItems_ReturnsTrueAndLastAddedValue()
   {
      // Arrange
      object expectedValue = new object();
      Type type = typeof(object);

      _sut.Add(type, new object(), AppendValueMode.Append);
      _sut.Add(type, expectedValue, AppendValueMode.Append);

      // Act
      bool contains = _sut.TryGet(type, out object? value);

      // Assert
      Assert.IsTrue(contains);
      Assert.AreSame(expectedValue, value);
   }

   [TestMethod]
   public void TryGet_WithoutItems_ReturnsFalse_AndNullValue()
   {
      // Arrange
      Type type = typeof(object);

      // Act
      bool contains = _sut.TryGet(type, out object? value);

      // Assert
      Assert.IsFalse(contains);
      Assert.IsNull(value);
   }

   [TestMethod]
   public void GetAll_WithItemsAdded_ReturnsCorrectValuesInOrder()
   {
      // Arrange
      object
         first = new object(),
         second = new object(),
         third = new object();

      Type type = typeof(object);

      _sut.Add(type, first, AppendValueMode.Append);
      _sut.Add(type, second, AppendValueMode.Append);
      _sut.Add(type, third, AppendValueMode.Append);

      // Act
      object[] values = _sut.GetAll(type).ToArray();

      // Assert
      Assert.That.AreEqual(3, values.Length);
      Assert.AreSame(first, values[0]);
      Assert.AreSame(second, values[1]);
      Assert.AreSame(third, values[2]);
   }

   [TestMethod]
   public void GetAll_WithMixedItemsAdded_ReturnsValuesForCorrectType()
   {
      // Arrange
      Type type = typeof(object);
      Type otherType = typeof(string);
      object value = new object();
      string otherValue = "test";

      _sut.Add(type, value);
      _sut.Add(otherType, otherValue);

      // Act
      IEnumerable<object> values = _sut.GetAll(type);

      // Pre-Assert
      object[] result = values.ToArray();

      // Assert
      Assert.That.AreEqual(1, result.Length);
      Assert.That.AreEqual(value, result[0]);
   }

   [TestMethod]
   public void Contains_WithItem_ReturnsTrue()
   {
      // Arrange
      object value = new object();
      Type type = typeof(object);

      _sut.Add(type, value);

      // Act
      bool contains = _sut.Contains(type);

      // Assert
      Assert.IsTrue(contains);
   }

   [TestMethod]
   public void Contains_WithoutItem_ReturnsFalse()
   {
      // Arrange
      Type type = typeof(object);

      // Act
      bool contains = _sut.Contains(type);

      // Assert
      Assert.IsFalse(contains);
   }

   [TestMethod]
   public void Dispose_WithItems_RemovesItems()
   {
      // Arrange
      Type type = typeof(object);
      _sut.Add(type, new object());

      // Arrange Assert
      Assert.That.IsInconclusiveIfNot(_sut.Contains(type));

      // Act
      _sut.Dispose();

      // Assert
      Assert.IsFalse(_sut.Contains(type));
   }

   [TestMethod]
   public void Dispose_WithItems_DisposesItems()
   {
      // Arrange
      Type type = typeof(object);
      Mock<IDisposable> disposable1 = new Mock<IDisposable>();
      Mock<IDisposable> disposable2 = new Mock<IDisposable>();

      _sut.Add(type, disposable1.Object, AppendValueMode.Append);
      _sut.Add(type, disposable2.Object, AppendValueMode.Append);

      // Arrange Assert
      Assert.That.IsInconclusiveIfNot(_sut.Contains(type));

      // Act
      _sut.Dispose();

      // Assert
      disposable1.VerifyDisposed();
      disposable2.VerifyDisposed();
   }

   [TestMethod]
   public void GetTypes_WithTypesAdd_ReturnsAllTypes()
   {
      // Arrange
      Type type1 = typeof(object);
      Type type2 = typeof(int);
      Type type3 = typeof(string);
      Type[] expected = new[] { type1, type2, type3 }; // order does not matter

      _sut.Add(type1, new object());
      _sut.Add(type2, new object());
      _sut.Add(type3, new object());

      // Act
      IEnumerable<Type> types = _sut.GetTypes();

      // Pre-Assert
      Type[] result = types.ToArray();

      // Assert
      CollectionAssert.AreEquivalent(expected, result);
   }

   [TestMethod]
   public void GetAllValues_OneValuePerKey_ReturnsAllValues()
   {
      // Arrange
      object value1 = new object();
      string value2 = "test";
      object[] expected = new[] { value1, value2 };  // order does not matter

      _sut.Add(value1.GetType(), value1);
      _sut.Add(value2.GetType(), value2);

      // Act
      IEnumerable<object> values = _sut.GetAllValues();

      // Pre-Assert
      object[] result = values.ToArray();

      // Assert
      CollectionAssert.AreEquivalent(expected, result);
   }

   [TestMethod]
   public void GetAllValues_MultipleValuesPerKey_ReturnsAllValues()
   {
      object value1 = new object();
      object value2 = new object();
      string value3 = "test1";
      string value4 = "test2";
      object[] expected = new[] { value1, value2, value3, value4 }; // order does not matter

      foreach (object value in expected)
         _sut.Add(value.GetType(), value, AppendValueMode.Append);

      // Act
      IEnumerable<object> values = _sut.GetAllValues();

      // Pre-Assert
      object[] result = values.ToArray();

      // Assert
      CollectionAssert.AreEquivalent(expected, result);
   }
}