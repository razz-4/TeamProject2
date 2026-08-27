using System.Collections.Generic;
using UnityEngine;

public class Factory : BuildingClass
{

    public List<int> selectedDrugs = new List<int>();

    private void Awake()
    {
        buildingType = "Factory";
        placable = true;
        level = 1;
        isLock = false;
    }

    public void ProduceDrugs(Drug drug, int index)
    {
        if (!selectedDrugs.Contains(index))
        {
            selectedDrugs.Add(index);
        }
    }

    public void CancelDrugs(Drug drug, int index)
    {
        if (selectedDrugs.Contains(index))
        {
            selectedDrugs.Remove(index);
        }
    }
}
