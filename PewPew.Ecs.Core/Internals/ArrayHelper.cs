namespace PewPew.Ecs.Core.Internals;

internal static class ArrayHelper
{
    public static void ResizeTwiceAndFill<T>(ref T[] src, Func<T> factory, int minSize = 0)
    {
        var oldSize = src.Length;
        var newSize = Math.Max(minSize, src.Length << 1);
        Array.Resize(ref src, newSize);

        for (int i = oldSize; i < newSize; i++)
        {
            src[i] = factory();
        }
    }
}