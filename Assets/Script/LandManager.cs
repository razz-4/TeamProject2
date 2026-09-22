using DevelopersHub.ClashOfWhatecer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class LandManager : MonoBehaviour
{
    public static LandManager Instance { get; private set; }

    [System.Serializable]
    public class Plot
    {
        public string plotName = "new plot";
        public int startX;
        public int startY;
        public int width;
        public int height;
        public int cost = 100;
        public bool isUnlocked = false;
        public GameObject visualIndicator;
    }

    public List<Plot> plots = new List<Plot>();
    public int playerGold = 500; 

    [Header("UI Button")]
    public GameObject buyButtonPrefab; 
    private GameObject currentBuyButtonInstance;
    private int selectedPlotIndex = -1;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvas = FindAnyObjectByType<Canvas>();

    }

    private void Start()
    {
        RefreshAllVisuals();
    }

    private void Update()
    {
        Debug.Log("Update running");
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane gridPlane = new Plane(Vector3.up, BuildGrid.Instance.transform.position);
            if (gridPlane.Raycast(ray, out float enter))
            {
                Debug.Log("press");
                Vector3 worldPos = ray.GetPoint(enter);
                TrySelectPlot(worldPos);
            }
            else
            {
                Debug.Log("no hit");
                HideBuyButton();
            }
        }
    }

    public void TrySelectPlot(Vector3 worldPos)
    {
        if (BuildGrid.Instance == null) return;
        Vector2Int gridPos = BuildGrid.Instance.WorldToGrid(worldPos);

        for (int i = 0; i < plots.Count; i++)
        {
            var p = plots[i];
            if (gridPos.x >= p.startX && gridPos.x < p.startX + p.width &&
                gridPos.y >= p.startY && gridPos.y < p.startY + p.height)
            {
                if (!p.isUnlocked)
                {
                    selectedPlotIndex = i;
                    ShowBuyButtonForPlot(i);
                    return;
                }
                else
                {
                    HideBuyButton();
                    return;
                }
            }
        }
        HideBuyButton();
    }

    private void ShowBuyButtonForPlot(int index)
    {
        Plot p = plots[index];
        if (buyButtonPrefab == null)
        {
            Debug.LogError("no button");
            return;
        }

        if (currentBuyButtonInstance == null)
        {
            currentBuyButtonInstance = Instantiate(buyButtonPrefab, canvas.transform);
        }

        Vector3 centerWorld = BuildGrid.Instance.GetCenterPosition(p.startX, p.startY, p.height, p.width);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(centerWorld);
        currentBuyButtonInstance.transform.position = screenPos;

        TextMeshProUGUI btnText = currentBuyButtonInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = $"Buy\n{p.cost} coin";

        Button btn = currentBuyButtonInstance.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            if (BuyPlot(selectedPlotIndex))
            {
                HideBuyButton();
            }
        });

        currentBuyButtonInstance.SetActive(true);
    }

    private void HideBuyButton()
    {
        if (currentBuyButtonInstance != null)
            currentBuyButtonInstance.SetActive(false);
        selectedPlotIndex = -1;
    }

    public bool BuyPlot(int plotIndex)
    {
        if (plotIndex < 0 || plotIndex >= plots.Count) return false;
        Plot p = plots[plotIndex];
        if (p.isUnlocked) return false;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found");
            return false;
        }
        if (!GameManager.Instance.SpendGold(p.cost))
        {
            Debug.Log("no enough gold");
            return false;
        }

        p.isUnlocked = true;
        UpdatePlotVisual(p);
        Debug.Log($"unlock plot：{p.plotName}");
        return true;
    }

    private void UpdatePlotVisual(Plot p)
    {
        if (p.visualIndicator != null)
            p.visualIndicator.SetActive(!p.isUnlocked);
    }

    public void RefreshAllVisuals()
    {
        foreach (var p in plots)
            UpdatePlotVisual(p);
    }

    public bool IsCellUnlocked(int x, int y)
    {
        foreach (var p in plots)
        {
            if (!p.isUnlocked) continue;
            if (x >= p.startX && x < p.startX + p.width &&
                y >= p.startY && y < p.startY + p.height)
                return true;
        }
        return false;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (plots == null || BuildGrid.Instance == null) return;
        foreach (var plot in plots)
        {
            Vector3 center = BuildGrid.Instance.GetCenterPosition(plot.startX, plot.startY, plot.height, plot.width);
            Vector3 size = new Vector3(plot.width * BuildGrid.Instance.cellSize, 0.1f, plot.height * BuildGrid.Instance.cellSize);
            Gizmos.color = plot.isUnlocked ? Color.green : Color.red;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawCube(center, size);
            UnityEditor.Handles.Label(center + Vector3.up * 0.5f, plot.plotName + (plot.isUnlocked ? " ok " : " lock "));
        }
    }
#endif
}