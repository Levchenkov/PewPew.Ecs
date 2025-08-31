// namespace PewPew.Ecs.UnitTests;
//
// public class BitMaskTests
// {
//     [Fact]
//     public void Has_AllBitsSet_ReturnsTrue()
//     {
//         // Arrange
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b1111 }); // Example bit mask
//         var bitMask2 = new BitMask(new[] { 0b1111, 0b1111 }); // Same as bitMask1
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result);
//     }
//
//     [Fact]
//     public void Has_SubsetBits_ReturnsTrue()
//     {
//         // Arrange
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b1111 }); // All bits set
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010 }); // Subset of bits set
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result); // bitMask1 should have all bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_MissingBits_ReturnsFalse()
//     {
//         // Arrange
//         var bitMask1 = new BitMask(new[] { 0b0011, 0b1010 }); // Only a few bits set
//         var bitMask2 = new BitMask(new[] { 0b1111, 0b1111 }); // All bits set
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.False(result); // bitMask1 is missing bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_LengthMismatch_ThrowsArgumentException()
//     {
//         // Arrange
//         var bitMask1 = new BitMask(new[] { 0b1111 }); // Length 1
//         var bitMask2 = new BitMask(new[] { 0b1111, 0b1111 }); // Length 2
//
//         // Act & Assert
//         Assert.Throws<ArgumentException>(() => bitMask1.Has(bitMask2));
//     }
//
//     [Fact]
//     public void Has_NullValue_ThrowsArgumentNullException()
//     {
//         // Arrange
//         var bitMask1 = new BitMask(new[] { 0b1111 }); // Valid bit mask
//
//         // Act & Assert
//         Assert.Throws<ArgumentNullException>(() => bitMask1.Has(null));
//     }
//
//     [Fact]
//     public void Has_SmallArray_NoVectorization_ReturnsTrue()
//     {
//         // Arrange: Array size smaller than Vector128 threshold (for simplicity, assume threshold is 4 elements)
//         var bitMask1 = new BitMask(new[] { 0b1111 }); // Single element
//         var bitMask2 = new BitMask(new[] { 0b1111 }); // Identical
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result); // Should pass without vectorization
//     }
//
//     [Fact]
//     public void Has_MediumArray_Vector128_ReturnsTrue()
//     {
//         // Arrange: Array size that fits Vector128 operations (e.g., 4 elements or more but smaller than Vector256)
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b1111, 0b1111, 0b1111 });
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010, 0b1100, 0b0111 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result); // bitMask1 should have all bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_MediumArray_Vector128_ReturnsFalse()
//     {
//         // Arrange: Vector128-size arrays with missing bits in bitMask1
//         var bitMask1 = new BitMask(new[] { 0b0001, 0b0010, 0b1100, 0b0011 });
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010, 0b1111, 0b0111 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.False(result); // bitMask1 is missing some bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_LargeArray_Vector256_ReturnsTrue()
//     {
//         // Arrange: Large array that fits Vector256 operations
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b1111, 0b1111, 0b1111, 0b1111, 0b1111, 0b1111, 0b1111 });
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010, 0b1100, 0b0111, 0b1110, 0b0110, 0b1011, 0b0101 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result); // bitMask1 should have all bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_LargeArray_Vector256_ReturnsFalse()
//     {
//         // Arrange: Large array with missing bits in bitMask1
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b0110, 0b1100, 0b1110, 0b0010, 0b1101, 0b0011, 0b0100 });
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010, 0b1111, 0b0111, 0b1110, 0b0110, 0b1011, 0b0101 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.False(result); // bitMask1 is missing some bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_MixedBitsWithVectorization_ReturnsTrue()
//     {
//         // Arrange: Mixed bits, should still work with vectorization
//         var bitMask1 = new BitMask(new[] { 0b1111, 0b1010, 0b1101, 0b0111 });
//         var bitMask2 = new BitMask(new[] { 0b0011, 0b1010, 0b1100, 0b0111 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.True(result); // bitMask1 has all bits from bitMask2
//     }
//
//     [Fact]
//     public void Has_MixedBitsWithVectorization_ReturnsFalse()
//     {
//         // Arrange: Mixed bits with missing bits in bitMask1
//         var bitMask1 = new BitMask(new[] { 0b1110, 0b1010, 0b1100, 0b0111 });
//         var bitMask2 = new BitMask(new[] { 0b1111, 0b1111, 0b1111, 0b1111 });
//
//         // Act
//         bool result = bitMask1.Has(bitMask2);
//
//         // Assert
//         Assert.False(result); // bitMask1 is missing some bits from bitMask2
//     }
//
// }