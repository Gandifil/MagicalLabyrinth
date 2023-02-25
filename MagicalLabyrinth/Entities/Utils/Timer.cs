using MLEM.Animations;

namespace MagicalLabyrinth.Entities.Utils;

public class Timer: IUpdate
{
    private float _counter;

    public bool IsCompleted => _counter <= 0;
    
    public void Update(float dt)
    {
        if (!IsCompleted)
        {
            _counter -= dt;
        }
    }

    public void Reset(float waitSeconds)
    {
        _counter = waitSeconds;
    }
}