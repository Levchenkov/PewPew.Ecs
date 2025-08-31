using PewPew.Ecs.Filters.Masks;

namespace PewPew.Ecs.UnitTests;

public class BitMask128Tests
{
    [Fact]
    public void SetBitAndClearBit()
    {
        var mask = new BitMask128();

        mask.Capacity.Should().Be(128);

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

        mask.SetBits(default(BitMask128).SetBit(0).SetBit(63).SetBit(64).SetBit(127));

        mask.GetBit(0).Should().Be(true);
        mask.GetBit(63).Should().Be(true);
        mask.GetBit(64).Should().Be(true);
        mask.GetBit(127).Should().Be(true);

        mask.ClearBits(default(BitMask128).SetBit(0).SetBit(63).SetBit(64).SetBit(127)).Should().Be(BitMask128.Zero);
    }
}