using DevelopersHub.ClashOfWhatecer;
using UnityEngine;
using UnityEngine.UI;

public class BuildingModeControl : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button enterBuildingModeBtn;
    public Button Confirmbtm;
    public Button Cancelbtm;

    private BuildingControl _selectedBuilding;
    private bool dialogPlayed = false;

    public static BuildingModeControl Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);

        enterBuildingModeBtn.onClick.AddListener(EnterBuildingMode);
        Confirmbtm.onClick.AddListener(Confirm);
        Cancelbtm.onClick.AddListener(Cancel);
    }



    public static void SelectBuilding(BuildingControl building)
    {
        if (Instance == null)
            return;

        if (building == null)
            return;

        // If this building is already selected, do nothing.
        if (Instance._selectedBuilding == building)
            return;

        // Remove selection/glow from previous building.
        if (Instance._selectedBuilding != null)
        {
            Instance._selectedBuilding.CancelMove();

            BuildingEditGlow oldGlow =
                Instance._selectedBuilding.GetComponent<BuildingEditGlow>();

            if (oldGlow != null)
            {
                oldGlow.StopGlow();
            }
        }

        // Select the new building.
        Instance._selectedBuilding = building;

        // Start the new building's glow.
        BuildingEditGlow newGlow =
            building.GetComponent<BuildingEditGlow>();

        if (newGlow != null)
        {
            newGlow.StartGlow();
        }

        // Show buttons.
        Instance.Confirmbtm.gameObject.SetActive(true);
        Instance.Cancelbtm.gameObject.SetActive(true);

        Debug.Log(
            "Selected building: " + building.gameObject.name
        );
    }



    public static void SelectNewBuilding(BuildingControl building)
    {
        if (Instance == null)
        {
            Debug.LogWarning(
                "BuildingModeManager.Instance is null. " +
                "Cannot select spawned building."
            );

            return;
        }

        if (building == null)
            return;

        // Make sure we are in building mode.
        BuildingControl.IsBuildingMode = true;

        // Select the newly spawned building.
        SelectBuilding(building);
    }



    private void Confirm()
    {
        if (_selectedBuilding != null)
        {
            _selectedBuilding.ConfirmMove();

            BuildingEditGlow glow =
                _selectedBuilding.GetComponent<BuildingEditGlow>();

            if (glow != null)
            {
                glow.StopGlow();
            }

            _selectedBuilding = null;
        }

        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);


        BuildingControl.IsBuildingMode = false;

        //Tutorial part
        if (!dialogPlayed) { TutorialManager.Instance.GoNextDialog(); dialogPlayed = true; TutorialManager.Instance.dialogPart = 2; }

        Debug.Log("Building move confirmed.");
    }


    private void Cancel()
    {
        if (_selectedBuilding != null)
        {
            _selectedBuilding.CancelMove();

            BuildingEditGlow glow =
                _selectedBuilding.GetComponent<BuildingEditGlow>();

            if (glow != null)
            {
                glow.StopGlow();
            }

            _selectedBuilding = null;
        }

        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);

        BuildingControl.IsBuildingMode = false;

        Debug.Log("Building move cancelled.");
    }


    //========= ENTER BUILDING MODE ===========

    public void SetBuildingMode(BuildingControl currentBuilding)
    {
        _selectedBuilding = currentBuilding;
    }
    private void EnterBuildingMode()
    {
        BuildingControl.IsBuildingMode = true;

        // Clear any old selection.
        if (_selectedBuilding != null)
        {
            BuildingEditGlow glow =
                _selectedBuilding.GetComponent<BuildingEditGlow>();

            if (glow != null)
            {
                glow.StopGlow();
            }

            _selectedBuilding = null;
        }

        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);

        Debug.Log("Entered building mode.");
    }



    public static void ExitBuildingMode()
    {
        if (Instance == null)
            return;

        if (Instance._selectedBuilding != null)
        {
            BuildingEditGlow glow =
                Instance._selectedBuilding.GetComponent<BuildingEditGlow>();

            if (glow != null)
            {
                glow.StopGlow();
            }

            Instance._selectedBuilding = null;
        }

        Instance.Confirmbtm.gameObject.SetActive(false);
        Instance.Cancelbtm.gameObject.SetActive(false);

        BuildingControl.IsBuildingMode = false;
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
