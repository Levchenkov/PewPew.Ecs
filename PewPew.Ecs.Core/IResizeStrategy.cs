namespace PewPew.Ecs.Core;

public interface IResizeStrategy
{
    int GetNewSize(int oldSize);
}