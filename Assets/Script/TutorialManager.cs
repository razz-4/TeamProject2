using System;
using UnityEngine;

[System.Serializable]
//In mainUI.cs onBuildingBtnPress
public class RingPositionData
{
    public string locationName; // Unity uses this as the element header in the Inspector
    public Vector3 position;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public static event Action NextDialog;
    public static event Action OnGlowinRingPressed;
    
    [SerializeField] private GlowingRing glowingRing;
    [SerializeField] private Dialog dialog;
    [SerializeField] private RingPositionData[] glowingRingData;
    private int dialogPart = 0;


    private void OnEnable()
    {
        Dialog.EndDialog += HandleDialogueEnd;
        OnGlowinRingPressed += GlowingRingPressed;
    }

    private void OnDisable()
    {
        Dialog.EndDialog -= HandleDialogueEnd;
        OnGlowinRingPressed -= GlowingRingPressed;
    }

    public void HandleDialogueEnd(int completedPartIndex)
    {
        dialogPart = 0;
        switch (completedPartIndex)
        {
            case 0:
                // Part 1 completed: show glowing ring at building area
                Debug.Log("Part 0 ended -> Move glowing ring to greenhouse.");
                glowingRing.SetPosition(glowingRingData[0].position);
                glowingRing.ShowRing();
                break;

            case 1:
                // Part 2 completed: enable shop UI
                Debug.Log("Part 1 ended -> Open shop UI.");
                break;

            case 2:
                // Part 3 completed: start gameplay loop
                Debug.Log("Part 2 ended -> Complete tutorial.");
                break;

            default:
                Debug.Log($"Dialogue Part {completedPartIndex} ended.");
                break;
        }
    }

    public void SetGlowRingPosition(int index)
    {
        glowingRing.SetPosition(glowingRingData[index].position);
        glowingRing.ShowRing();
    }

    public void GlowingRingPressed()
    {
        glowingRing.HideRing();
    }

    public void GoNextDialog()
    {
        dialogPart++;
        dialog.StartDialoguePart(dialogPart);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    
}
