namespace PewPew.Ecs.Core;

public interface IComponentsInitializer :
    IComponentInitializer,
    ITagInitializer,
    ISingletonInitializer,
    IStaticBufferInitializer
{
}

public interface IComponentInitializer
{
    void InitComponent<T>() where T : struct, IComponent;

    void InitComponent<T>(int maxComponentsPerSet) where T : struct, IComponent;
}

public interface ITagInitializer
{
    void InitTag<T>() where T : struct, ITagComponent;

    void InitTag<T>(int maxComponentsPerSet) where T : struct, ITagComponent;
}

public interface ISingletonInitializer
{
    void InitSingleton<T>() where T : struct, ISingletonComponent;
}

public interface IStaticBufferInitializer
{
    void InitStaticBuffer<T>() where T : struct, IStaticBufferComponent;

    void InitStaticBuffer<T>(int maxComponentsPerSet, int maxElementsCount) where T : struct, IStaticBufferComponent;
}