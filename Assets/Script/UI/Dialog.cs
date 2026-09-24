using System.Collections;
using TMPro;
using UnityEngine;
using System;

[System.Serializable]
public class DialoguePart
{
    public string partName; // Optional: helps identify parts in the Inspector (e.g., "Tutorial Part 1")
    [TextArea(2, 5)]
    public string[] lines;
}

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.05f;

    [Header("Dialogue Content")]
    public DialoguePart[] dialogueParts; // Set up Part 0, Part 1, Part 2 in the Unity Inspector
    public static event Action<int> EndDialog;

    private string[] lines; // Holds the active dialogue lines for the current part
    private int CurrentDialogIndex;
    private int index;

    

    void Start()
    {
        // Plays the first dialogue part (Part 0) when the game starts
        StartDialoguePart(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If line is finished typing, advance to the next line
            if (textComponent.text == lines[index])
            {
                NextLine();
                SoundManager.PlaySound(SoundType.UI, 1);
            }
            // If line is still typing, instantly complete the current line
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                SoundManager.PlaySound(SoundType.UI, 1);
            }
        }
    }

    /// <summary>
    /// Call this function from anywhere to load and play a specific part (e.g., StartDialoguePart(1) for Part 2)
    /// </summary>
    public void StartDialoguePart(int partIndex)
    {
        if (partIndex >= 0 && partIndex < dialogueParts.Length)
        {
            CurrentDialogIndex = partIndex;
            // Load lines from the chosen dialogue part
            lines = dialogueParts[partIndex].lines;

            if (lines != null && lines.Length > 0)
            {
                gameObject.transform.parent.gameObject.SetActive(true);
                index = 0;
                textComponent.text = string.Empty;

                StopAllCoroutines();
                StartCoroutine(TypeLine());
            }
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // End of current dialogue part
            GameObject wholeDialog = gameObject.transform.parent.gameObject;
            wholeDialog.SetActive(false);

            EndOfDialog(); //Call the event
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void EndOfDialog()
    {
        EndDialog?.Invoke(CurrentDialogIndex);

    }
}