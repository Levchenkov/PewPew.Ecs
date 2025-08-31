using PewPew.Ecs.Demo;

public class SystemCollection
{
    private List<IStateful> _stateSetters = new();
    private List<IUpdatableSystem> _updatableSystems = new();

    public void Add<T>(T system)
    {
        if (system is IStateful stateSetter)
        {
            _stateSetters.Add(stateSetter);
        }
        if (system is IUpdatableSystem updatableSystem)
        {
            _updatableSystems.Add(updatableSystem);
        }
    }

    public void Update(State state)
    {
        foreach (var system in _stateSetters)
        {
            system.SetState(state.World1, state.World2);
        }

        foreach (var system in _updatableSystems)
        {
            system.Update();
        }
    }
}