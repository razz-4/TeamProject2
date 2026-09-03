using UnityEngine;

[System.Serializable]
public struct CountDownTimer
{
    [SerializeField] private float Duration;
    [SerializeField] private float CurrentTime;
    public bool IsFinished => CurrentTime <= 0f;

    public CountDownTimer(float duration)
    {
        Duration = duration;
        CurrentTime = duration;
    }

    /// <summary>
    /// Call this inside Update(). Returns true when the duration completes and automatically resets.
    /// </summary>
    public bool Tick(float deltaTime)
    {
        if (CurrentTime > 0f)
        {
            CurrentTime -= deltaTime;
        }

        if (CurrentTime <= 0f)
        {
            Reset();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the timer back to its initial duration.
    /// </summary>
    public void Reset()
    {
        CurrentTime = Duration;
    }

    /// <summary>
    /// Changes the timer duration and resets it.
    /// </summary>
    public void Reset(float newDuration)
    {
        Duration = newDuration;
        CurrentTime = newDuration;
    }
}
