using UnityEngine;
using UnityEngine.Events;

public class BuildingTimerController : MonoBehaviour
{
    [SerializeField] private LoadingTimer loadingTimer;

    public UnityEvent OnTimerFinished => loadingTimer != null ? loadingTimer.onTimerFinish : null;

    private void OnEnable()
    {
        if (loadingTimer == null)
        {
            loadingTimer = GetComponentInChildren<LoadingTimer>();
        }
    }

    public void StartProductionTimer()
    {
        if (loadingTimer == null) return;

        loadingTimer.ResetTimer();
        loadingTimer.StartTimer();
        loadingTimer.showTimer();
    }

    public void StopProductionTimer()
    {
        if (loadingTimer == null) return;

        loadingTimer.StopTimer();
        loadingTimer.hideTimer();
    }

    public LoadingTimer GetTimer()
    {
        return loadingTimer;
    }
}