using UnityEngine;

[DisallowMultipleComponent]
public class BuildingPopUpSpawner : MonoBehaviour
{
    [Header("PopUp Configuration")]
    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private Transform popUpSpawnPoint;
    [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 4f, 0f);

    public void SpawnPopUp(Sprite icon, int amount)
    {
        if (popUpPrefab == null) return;

        Vector3 spawnPos = popUpSpawnPoint != null ? popUpSpawnPoint.position : transform.position + defaultOffset;
        GameObject popUpInstance = Instantiate(popUpPrefab, spawnPos, Quaternion.identity);

        if (popUpInstance.TryGetComponent(out PopUpUI popUpScript))
        {
            popUpScript.Setup(icon, amount);
        }
    }
}