using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelUpButton : MonoBehaviour
{
    private BuildingClass currentBuildingClass;



    [SerializeField] private TextMeshProUGUI priceText;
    public void SetCurrentBuildingClass(BuildingClass buildingClass)
    {
        currentBuildingClass = buildingClass;
    }

    public void LevelUpBuilding()
    {
        if (currentBuildingClass == null) return;

        // Don't allow upgrading beyond level 3
        if (currentBuildingClass.level >= 3)
        {
            SoundManager.PlaySound(SoundType.DECLINE, 1);
            return;
        }

        // Check if player has enough gold first
        if (!GameManager.Instance.SpendGold(currentBuildingClass.coinNeededForUpgrade))
        {
            SoundManager.PlaySound(SoundType.DECLINE, 1);
            return;
        }

        currentBuildingClass.LevelUp();

        currentBuildingClass.coinNeededForUpgrade =
            (int)(currentBuildingClass.coinNeededForUpgrade *
                  currentBuildingClass.upgradeMultiplier);

        SoundManager.PlaySound(SoundType.SOLD, 1);

        UIBuildingManager.Instance.UpdateLevelUpUI();
    }
}
