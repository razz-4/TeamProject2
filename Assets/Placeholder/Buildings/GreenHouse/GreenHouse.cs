using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class GreenHouse : BuildingClass
{
    public int maxToggle;

    [Header("Timer")]
    [SerializeField] private CountDownTimer Timer;
    [SerializeField] private float growthDuration = 60f;
    private bool isGrowing = false;

    // Save index positions instead of object references
    public List<int> selectedIndices = new List<int>(); //{0,5}
    private List<Ingredient> currentGrowingIngredients = new List<Ingredient>();

    private void Awake()
    {
        buildingType = "GreenHouse";
        placable = true;
        level = 1;
        coinNeededForUpgrade = 10;
        isLock = false;
    }

        private void Update()
    {
        // 1. Only tick the timer if something is actively growing
        if (isGrowing)
        {
            // Tick counts down. When it hits 0, it returns true.
            if (Timer.Tick(Time.deltaTime))
            {
                HarvestCrop();
            }
        }
    }

    public void ProductIngredients(Ingredient targetIngredient, int index)
    {
        if (!selectedIndices.Contains(index))
        {
            selectedIndices.Add(index);
        }

        // Add to our active growth tracking list
        if (!currentGrowingIngredients.Contains(targetIngredient))
        {
            currentGrowingIngredients.Add(targetIngredient);
        }

        // 2. Start the 60-second growth timer when the player clicks/starts production
        if (!isGrowing)
        {
            Timer.Reset(growthDuration); // Sets duration to 60s and resets CurrentTime
            isGrowing = true;
            Debug.Log($"Growth started! Waiting {growthDuration} seconds for harvest...");
        }
    }

    // 3. This triggers automatically when the 60 seconds are up
    private void HarvestCrop()
    {
        Debug.Log("Crops are fully grown! Awarding ingredients to player.");
        
        isGrowing = false; // Stop the timer from updating further

        // Apply your perSecond modifications to your global ItemManager now that they are ready
        foreach (Ingredient ingredient in currentGrowingIngredients)
        {
            foreach (Ingredient globalIngredient in ItemManager.Instance.ingredients)
            {
                if (globalIngredient == ingredient)
                {
                    // Award the player their resource batch! 
                    // (Adjust this line to match your Ingredient script's inventory variable)
                    // globalIngredient.quantity += 1; 
                    
                    globalIngredient.currentPerSecond += globalIngredient.perSecond;
                }
            }
        }

        // Clear the batch out so the greenhouse is ready for the next plant cycle
        currentGrowingIngredients.Clear();
        selectedIndices.Clear(); 
    }

    public void CancelProductIngredients(Ingredient targetIngredient, int index)
    {
        if (selectedIndices.Contains(index))
        {
            selectedIndices.Remove(index);
        }

        if (currentGrowingIngredients.Contains(targetIngredient))
        {
            currentGrowingIngredients.Remove(targetIngredient);
        }

        // If the player cancels everything mid-growth, turn off the timer
        if (selectedIndices.Count == 0)
        {
            isGrowing = false;
            Timer.Reset(0f);
        }

        foreach (Ingredient ingredient in ItemManager.Instance.ingredients)
        {
            if (ingredient == targetIngredient)
            {
                ingredient.currentPerSecond -= ingredient.perSecond;
            }
        }
    }

    public void CollectIngredient(Ingredient ingredient)
    {
        foreach(Ingredient globalIngredient in ItemManager.Instance.ingredients)
        {
            if (globalIngredient == ingredient)
            {
                globalIngredient.quantity += ingredient.currentPerSecond;
                Debug.Log($"Produce {ingredient.currentPerSecond} {ingredient.name}.");
            }
        }
    }
}
