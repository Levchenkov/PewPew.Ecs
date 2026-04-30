namespace PewPew.Ecs.Core;

public interface IComponentsProvider :
    IComponentCollectionProvider,
    ITagCollectionProvider,
    ISingletonProvider,
    IStaticBufferCollectionProvider,
    IDynamicBufferCollectionProvider
{
}

public interface IComponentCollectionProvider : IComponentProvider
{
    ComponentCollection<T> GetComponents<T>() where T : struct, IComponent;
}

public interface ITagCollectionProvider : ITagProvider
{
    TagCollection<T> GetTags<T>() where T : struct, ITagComponent;
}

public interface IStaticBufferCollectionProvider : IStaticBufferProvider
{
    StaticBufferCollection<T> GetStaticBuffers<T>() where T : struct, IStaticBufferComponent;
}

public interface IComponentProvider
{
    bool HasComponent<T>(EntityId entityId) where T : struct, IComponent;

    ref T GetComponent<T>(EntityId entityId) where T : struct, IComponent;

    bool TryGetComponent<T>(EntityId entityId, out ComponentRef<T> componentRef) where T : struct, IComponent;

    ref T AddComponent<T>(EntityId entityId) where T : struct, IComponent;

    void DeleteComponent<T>(EntityId entityId) where T : struct, IComponent;
}

public interface ISingletonProvider
{
    bool HasSingleton<T>() where T : struct, ISingletonComponent;

    ref T GetSingleton<T>() where T : struct, ISingletonComponent;

    bool TryGetSingleton<T>(out ComponentRef<T> componentRef) where T : struct, ISingletonComponent;

    bool TryGetSingleton<T>(ref T component) where T : struct, ISingletonComponent;

    ref T AddSingleton<T>() where T : struct, ISingletonComponent;

    void DeleteSingleton<T>() where T : struct, ISingletonComponent;
}

public interface ITagProvider
{
    bool HasTag<T>(EntityId entityId) where T : struct, ITagComponent;

    void AddTag<T>(EntityId entityId) where T : struct, ITagComponent;

    void DeleteTag<T>(EntityId entityId) where T : struct, ITagComponent;
}

public interface IStaticBufferProvider
{
    bool HasStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent;

    StaticBuffer<T> GetStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent;

    StaticBuffer<T> AddStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent;

    void DeleteStaticBuffer<T>(EntityId entityId) where T : struct, IStaticBufferComponent;
}

public interface IDynamicBufferCollectionProvider : IDynamicBufferProvider
{
    DynamicBufferCollection<T> GetDynamicBuffers<T>() where T : struct, IDynamicBufferComponent;
}

public interface IDynamicBufferProvider
{
    bool HasDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent;

    DynamicBuffer<T> GetDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent;

    bool TryGetDynamicBuffer<T>(EntityId entityId, out DynamicBuffer<T> buffer) where T : struct, IDynamicBufferComponent;

    DynamicBuffer<T> AddDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent;

    void DeleteDynamicBuffer<T>(EntityId entityId) where T : struct, IDynamicBufferComponent;
}