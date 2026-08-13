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
        Debug.Log("IsBuildingMode set to " + BuildingControl.IsBuildingMode);
        _selectedBuilding = null;
        Confirmbtm.gameObject.SetActive(false);
        Cancelbtm.gameObject.SetActive(false);
        Debug.Log("into building mode");
    }

    private void Update()
    {

        if (BuildingControl.IsBuildingMode && !BuildingControl.IsBeingDragged)
        {

        }
    }


   
}