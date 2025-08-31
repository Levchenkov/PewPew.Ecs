namespace PewPew.Ecs.UnitTests;

public class DebugOnlyFactAttribute : FactAttribute {
#if RELEASE
    public DebugOnlyFactAttribute() {
        Skip = "Ignored when RELEASE";
    }
#endif
}