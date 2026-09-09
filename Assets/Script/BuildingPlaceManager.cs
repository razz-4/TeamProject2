using System.Collections.Generic;
using UnityEngine;

public class BuildingPlaceManager : MonoBehaviour
{
    public static BuildingPlaceManager Instance { get; private set; }

    
    private HashSet<string> occupiedCells = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

   
    public void RegisterBuilding(int startX, int startY, int rows, int cols, object buildingId)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                string key = $"{startX + i},{startY + j}";
                if (!occupiedCells.Contains(key))
                    occupiedCells.Add(key);
                else
                    Debug.LogWarning($"格子 ({startX + i},{startY + j}) 已被占用，注册可能重叠！");
            }
        }
    }

    
    public void UnregisterBuilding(int startX, int startY, int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                string key = $"{startX + i},{startY + j}";
                occupiedCells.Remove(key);
            }
        }
    }

    
    public bool CanPlace(int startX, int startY, int rows, int cols, object excludeBuildingId = null)
    {
        //check if the cells are unlocked
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int cellX = startX + i;
                int cellY = startY + j;
                if (!LandManager.Instance.IsCellUnlocked(cellX, cellY))
                {
                    return false; 
                }
            }
        }
        //check if the cells are occupied
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                string key = $"{startX + i},{startY + j}";
                if (occupiedCells.Contains(key))
                {
                    
                    return false;
                }
            }
        }
        return true;
    }

    
    public void UpdateBuildingPosition(int oldX, int oldY, int newX, int newY, int rows, int cols, object buildingId)
    {
        UnregisterBuilding(oldX, oldY, rows, cols);
        RegisterBuilding(newX, newY, rows, cols, buildingId);
    }
}