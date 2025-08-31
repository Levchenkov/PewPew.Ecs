namespace PewPew.Ecs.Core;

public class DoubleSizeStrategy : IResizeStrategy
{
    private readonly int _minSize;

    public DoubleSizeStrategy(int minSize)
    {
        _minSize = minSize;
    }

    public int GetNewSize(int oldSize)
    {
        return Math.Max(oldSize * 2, _minSize);
    }
}