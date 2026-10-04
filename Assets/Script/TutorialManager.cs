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
    public int dialogPart = 0;


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
                dialogPart = 1;
                break;

            case 1:
                // Part 2 completed: Collected Ingredients
                dialogPart = 2;
                Debug.Log("Ingredients Collected");
                break;

            case 2:
                dialogPart = 3;
                GameManager.Instance.AddGold(80000);
                SoundManager.PlaySound(SoundType.SOLD, 1);
                Debug.Log("Build Factory and Delivery");
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
        dialog.StartDialoguePart(dialogPart);
        Debug.Log(dialogPart);
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
