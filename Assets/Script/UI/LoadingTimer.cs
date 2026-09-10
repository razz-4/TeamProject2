using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingTimer : MonoBehaviour
{
    private Image radialImage;

    [SerializeField] private CountDownTimer Timer;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timer Property")]
    [SerializeField] private bool isTimerOn = false;
    [SerializeField] private bool onRepeat = false;

    [Header("Event Listener")]
    public UnityEvent onTimerFinish;

    private void Awake()
    {
        radialImage = GetComponentInChildren<Image>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        SetTimer(5f);
    }
    private void Update()
    {
        if(isTimerOn)
        {
            //if timer reaches 0
            if(Timer.Tick(Time.deltaTime))
            {
                if (radialImage != null)
                {
                    radialImage.fillAmount = 0f;
                }

                //Send a signal and can call other functions
                onTimerFinish?.Invoke();

                //Stop timer when onRepeat is false.
                if(!onRepeat)
                {
                    StopTimer();
                }
                return;
            }

            if (radialImage != null && radialImage.fillAmount >= 0f)
            {
                radialImage.fillAmount = Timer.CurrentTime / Timer.Duration;
            }
            
        }
    }

    public void ResetTimer()
    {
        Timer.Reset();
    }

    public void SetTimer(float duration)
    {
        Timer.Reset(duration);
        radialImage.fillAmount = Timer.CurrentTime / Timer.Duration;
    }

    public bool StartTimer()
    {
        isTimerOn = true;
        showTimer();
        return isTimerOn;
    }

    public bool StopTimer()
    {
        isTimerOn = false;
        hideTimer();
        return isTimerOn;
    }

    public void hideTimer()
    {
        canvasGroup.alpha = 0;
    }

    public void showTimer()
    {
        canvasGroup.alpha = 1;
    }
}
