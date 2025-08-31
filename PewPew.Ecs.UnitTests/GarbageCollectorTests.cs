using System.Runtime.CompilerServices;

namespace PewPew.Ecs.UnitTests;

public class GarbageCollectorTests
{
    [Fact]
    public void ArrayMovesToNextGenerationAfterGC()
    {
        var array = new int[1000];
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }

        ref var e1 = ref array[1];
        ref var e2 = ref Unsafe.Add(ref e1, 1);

        int initialGeneration = GC.GetGeneration(array);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        int newGeneration = GC.GetGeneration(array);

        Assert.True(newGeneration > initialGeneration, "Array should be in the next generation");
        Assert.True(e1 == 1);
        Assert.True(e2 == 2);
    }
}