using DevelopersHub.ClashOfWhatecer;   
using UnityEngine;
using UnityEngine.UI;


public class BuildingModeManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button enterBuildingModeBtn;
    public Button Confirmbtm;
    public Button Cancelbtm;

    private BuildingControl _selectedBuilding;



    public static BuildingModeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    
    public static void SelectBuilding(BuildingControl building)
    {
        if (Instance == null) 
            return;
        if (Instance._selectedBuilding != null && Instance._selectedBuilding != building)
        {
            Instance._selectedBuilding.CancelMove();
        }
        Instance._selectedBuilding = building;
        // let the confirm and cancel button come out
        Instance.Confirmbtm.gameObject.SetActive(true);
        Instance.Cancelbtm.gameObject.SetActive(true);
    }




    private void Start()
    {

        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);

        enterBuildingModeBtn.onClick.AddListener(EnterBuildingMode);
        Confirmbtm.onClick.AddListener(Confirm);
        Cancelbtm.onClick.AddListener(Cancel);
    }

    private void Confirm()
    {
        if (_selectedBuilding != null)
        {
            _selectedBuilding.ConfirmMove();   
            _selectedBuilding = null;
            Confirmbtm.gameObject.SetActive(false);
            Cancelbtm.gameObject.SetActive(false);
            BuildingControl.IsBuildingMode = false; 
        }
    }

    private void Cancel()
    {
        if (_selectedBuilding != null)
        {
            _selectedBuilding.CancelMove();    
            _selectedBuilding = null;
            Confirmbtm.gameObject.SetActive(false);
            Cancelbtm.gameObject.SetActive(false);
            BuildingControl.IsBuildingMode = false;
        }
    }

    private void EnterBuildingMode()
    {
        BuildingControl.IsBuildingMode = true;
        if (_selectedBuilding != null)
        {
            _selectedBuilding.CancelMove(); 
            _selectedBuilding = null;
        }
        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_selectedBuilding != null && Camera.main != null)
        {
            UpdateButtonPositions();
        }
    }

    private void UpdateButtonPositions()
    {
        Vector3 worldPos = _selectedBuilding.transform.position;

        worldPos += new Vector3(0f, 0f, 0f); 

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0f)
        {
            Confirmbtm.gameObject.SetActive(false);
            Cancelbtm.gameObject.SetActive(false);
            return;
        }

        Confirmbtm.transform.position = screenPos + new Vector3(-80f, -80f, 0f);
        Cancelbtm.transform.position = screenPos + new Vector3(80f, -80f, 0f);
    }



}