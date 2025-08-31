 using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class BitMask64Tests
{
    [Fact]
    public void SetBitAndClearBit()
    {
        var mask = new BitMask64();

        mask.Capacity.Should().Be(64);

        for (int i = 0; i < mask.Capacity; i++)
        {
            mask.GetBit(i).Should().Be(false);
        }

        for (int i = 0; i < mask.Capacity; i++)
        {
            mask.SetBit(i);
        }

        for (int i = 0; i < mask.Capacity; i++)
        {
            mask.GetBit(i).Should().Be(true);
        }

        for (int i = 0; i < mask.Capacity; i++)
        {
            mask.ClearBit(i);
        }

        for (int i = 0; i < mask.Capacity; i++)
        {
            mask.GetBit(i).Should().Be(false);
        }

        mask.SetBits(default(BitMask64).SetBit(3).SetBit(5).SetBit(7).SetBit(63));

        mask.GetBit(3).Should().Be(true);
        mask.GetBit(5).Should().Be(true);
        mask.GetBit(7).Should().Be(true);

        mask.ClearBits(default(BitMask64).SetBit(3).SetBit(5).SetBit(7).SetBit(63)).Should().Be(BitMask64.Zero);
    }
}