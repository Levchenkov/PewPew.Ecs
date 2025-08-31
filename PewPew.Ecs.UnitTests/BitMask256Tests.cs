using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class BitMask256Tests
{
    [Fact]
    public void SetBitAndClearBit()
    {
        var mask = new BitMask256();

        mask.Capacity.Should().Be(256);

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

        mask.SetBits(default(BitMask256).SetBit(0).SetBit(63).SetBit(64).SetBit(127).SetBit(128).SetBit(255));

        mask.GetBit(0).Should().Be(true);
        mask.GetBit(63).Should().Be(true);
        mask.GetBit(64).Should().Be(true);
        mask.GetBit(127).Should().Be(true);
        mask.GetBit(128).Should().Be(true);
        mask.GetBit(255).Should().Be(true);

        mask.ClearBits(default(BitMask256).SetBit(0).SetBit(63).SetBit(64).SetBit(127).SetBit(128).SetBit(255)).Should().Be(BitMask256.Zero);
    }
}