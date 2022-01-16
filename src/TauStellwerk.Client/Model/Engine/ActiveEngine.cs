using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model.Engine;

public class ActiveEngine : EngineFull
{
    private Direction _direction = Direction.Forwards;

    private int _throttle;

    public ActiveEngine(EngineFullDto engine)
        : base(engine)
    {
    }

    public Direction Direction
    {
        get => _direction;
        internal set => SetProperty(ref _direction, value);
    }

    public int Throttle
    {
        get => _throttle;
        internal set => SetProperty(ref _throttle, value);
    }

    public static new ActiveEngine? Create(EngineFullDto? engine)
    {
        if (engine == null)
        {
            return null;
        }

        return new ActiveEngine(engine);
    }
}
