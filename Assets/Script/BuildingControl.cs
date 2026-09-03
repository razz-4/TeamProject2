namespace DevelopersHub.ClashOfWhatecer
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class BuildingControl : MonoBehaviour
    {

        private static BuildingControl _instance = null; public static BuildingControl instance { get { return _instance; } set { _instance = value; } }
        public static bool IsBeingDragged { get; private set; } = false;
        public static bool IsBuildingMode { get; set; } = false;
        [SerializeField] private BuildGrid _grid;

        private int _originalX, _originalY;



        [System.Serializable]
        public class Level
        {
            public int level = 1;
            public Sprite icon = null;
            public GameObject mesh = null;
        }

        

        [SerializeField]
        private int _rows = 1; public int rows { get { return _rows; } }
        [SerializeField]
        private int _columns = 1; public int columns { get { return _columns; } }

        [SerializeField]
        private MeshRenderer _baseArea = null;

        [SerializeField] private Level[] _levels = null;

        [SerializeField] private Vector2Int spawnGridPosition = new Vector2Int(0, 0);
        private int _currentX = 0; public int currentX { get { return _currentX; } }
        private int _currentY = 0; public int currentY { get { return _currentY; } }
        private int _X = 0;
        private int _Y = 0;

        private bool _isDragging = false;           
        private Vector3 _dragBaseLocalPos;          
        private Collider _buildingCollider;

        private object buildingId;
        private Color defaultColor;






        private void Awake()
        {
            if (_baseArea != null)
                defaultColor = _baseArea.material.color;

            buildingId = System.Guid.NewGuid().ToString();
            if (_grid == null)
            {
                _grid = BuildGrid.Instance;
                if (_grid == null)
                    Debug.LogError("BuildGrid.Instance is null, make sure BuildGrid exists in scene!");
            }
            _buildingCollider = GetComponent<Collider>();

            if (_buildingCollider == null)
                Debug.LogWarning("Building dont have collider cant drag");
        }

        public void PlaceOnGrid(int x, int y)
        {
            _currentX = x;
            _currentY = y;
            _Y = y;
            _X = x;
            Vector3 position = _grid.GetCenterPosition(x, y, _rows, _columns);
            transform.position = position;

        }

        public void StartMovingOnGrid()
        {
            _X = _currentX;
            _Y = _currentY;
            _originalX = _currentX;
            _originalY = _currentY;

            if (BuildingPlaceManager.Instance != null)
                BuildingPlaceManager.Instance.UnregisterBuilding(_currentX, _currentY, _rows, _columns);



        }

        public void RemoveFromGrid()
        {
            BuildingPlaceManager.Instance.UnregisterBuilding(_currentX, _currentY, _rows, _columns);
            Destroy(gameObject);
        }

        public void UpdateGridPosition(Vector3 baseWorldPos, Vector3 currentWorldPos)
        {
            Vector3 dir = currentWorldPos - baseWorldPos;
            Vector3 localDir = _grid.transform.InverseTransformDirection(dir);
            int xDis = Mathf.RoundToInt(localDir.x / _grid.cellSize);
            int yDis = Mathf.RoundToInt(localDir.z / _grid.cellSize);

            int newX = _X + xDis;
            int newY = _Y + yDis;

            //give limit let building cannot go out of grid
            int maxX = BuildGrid.Instance._columns - _columns;
            int maxY = BuildGrid.Instance._rows - _rows;
            newX = Mathf.Clamp(newX, 0, maxX);
            newY = Mathf.Clamp(newY, 0, maxY);

            _currentX = newX;
            _currentY = newY;
            Vector3 position = _grid.GetCenterPosition(_currentX, _currentY, _rows, _columns);
            transform.position = position;

            bool canPlace = BuildingPlaceManager.Instance.CanPlace(newX, newY, _rows, _columns, buildingId);
            if (_baseArea != null)
            {
                _baseArea.material.color = canPlace ? defaultColor : Color.red;
            }

           
        }
        

        private void Start()
        {
            int maxX = BuildGrid.Instance._columns - _columns;
            int maxY = BuildGrid.Instance._rows - _rows;
            _currentX = Mathf.Clamp(spawnGridPosition.x, 0, maxX);
            _currentY = Mathf.Clamp(spawnGridPosition.y, 0, maxY);
            StartMovingOnGrid();
            PlaceOnGrid(_currentX, _currentY);
            UpdateGridPosition(Vector3.zero, Vector3.zero);
            if (BuildingPlaceManager.Instance != null)
                BuildingPlaceManager.Instance.RegisterBuilding(_currentX, _currentY, _rows, _columns, buildingId);
            Debug.Log($"Default color: {defaultColor}");

        }

        public void ConfirmMove()
        {
            bool canPlace = BuildingPlaceManager.Instance.CanPlace(_currentX, _currentY, _rows, _columns, buildingId);
            if (canPlace)
            { 
                BuildingPlaceManager.Instance.RegisterBuilding(_currentX, _currentY, _rows, _columns, buildingId);
                _originalX = _currentX;
                _originalY = _currentY;
                if (_baseArea != null)
                    _baseArea.material.color = defaultColor; 
                Debug.Log($"building {name} makesure got place ({_currentX}, {_currentY})");
            }
            else
            {
                Debug.LogWarning("here got building return");
                CancelMove(); 
            }
        }


        public void CancelMove()
        {
            _currentX = _originalX;
            _currentY = _originalY;

            BuildingPlaceManager.Instance.RegisterBuilding(_currentX, _currentY, _rows, _columns, buildingId);
            PlaceOnGrid(_currentX, _currentY); 

            if (_baseArea != null)
                _baseArea.material.color = defaultColor;
            Debug.Log($"building {name} no move cancel ({_currentX}, {_currentY})");
        }

        public void PlaceAtGridPosition(int x, int y)
        {
            BuildingPlaceManager.Instance.UnregisterBuilding(_currentX, _currentY, _rows, _columns);

            _currentX = x;
            _currentY = y;
            _X = x;
            _Y = y;
            _originalX = x;
            _originalY = y;

            Vector3 position = _grid.GetCenterPosition(x, y, _rows, _columns);
            transform.position = position;

            BuildingPlaceManager.Instance.RegisterBuilding(x, y, _rows, _columns, buildingId);

            if (_baseArea != null)
                _baseArea.material.color = defaultColor;
        }


        private void Update()
        {
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    OnPointerDown(Mouse.current.position.ReadValue());

                if (Mouse.current.leftButton.isPressed && _isDragging)
                    OnPointerDrag(Mouse.current.position.ReadValue());

                if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
                    OnPointerUp();
            }

            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch != null)
                {
                    if (touch.press.wasPressedThisFrame)
                        OnPointerDown(touch.position.ReadValue());

                    if (touch.press.isPressed && _isDragging)
                        OnPointerDrag(touch.position.ReadValue());

                    if (touch.press.wasReleasedThisFrame && _isDragging)
                        OnPointerUp();
                }
            }
        }
        private void OnPointerDown(Vector2 screenPos)
        {
            if (!IsBuildingMode) return;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                BuildingControl bc = hit.collider.GetComponentInParent<BuildingControl>();
                if (bc == this)
                {
                    BuildingModeManager.SelectBuilding(this);
                    IsBeingDragged = true;
                    _isDragging = true;
                    StartMovingOnGrid();

                    Vector3 worldPos = GetWorldPositionOnGrid(screenPos);
                    if (worldPos != Vector3.zero)
                    {
                        _dragBaseLocalPos = _grid.transform.InverseTransformPoint(worldPos);
                    }
                    else
                    {
                        _dragBaseLocalPos = _grid.transform.InverseTransformPoint(transform.position);
                        _isDragging = true; // 保持拖拽启用
                        IsBeingDragged = true;
                    }
                }
            }
        }

        private void OnPointerDrag(Vector2 screenPos)
        {
            Vector3 worldPos = GetWorldPositionOnGrid(screenPos);
            if (worldPos != Vector3.zero)
            {
                Vector3 currentLocalPos = _grid.transform.InverseTransformPoint(worldPos);
                UpdateGridPosition(_dragBaseLocalPos, currentLocalPos);
            }
            Debug.Log("draging: (" + _currentX + ", " + _currentY + ")");
        }

        private void OnPointerUp()
        {
            _isDragging = false;
            IsBeingDragged = false;
            if (_baseArea != null)
            {
                bool canPlace = BuildingPlaceManager.Instance.CanPlace(_currentX, _currentY, _rows, _columns, buildingId);
                _baseArea.material.color = canPlace ? defaultColor : Color.red;
            }
        }

        private Vector3 GetWorldPositionOnGrid(Vector2 screenPos)
        {
            if (Camera.main == null) { Debug.LogError("dont have MainCamera！"); return Vector3.zero; }
            if (_grid == null)
                return Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Plane gridPlane = new Plane(Vector3.up, _grid.transform.position);
            if (gridPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            Debug.Log("GetWorldPositionOnGrid: " + screenPos);
            Vector3 fallback = transform.position;
            fallback.y = _grid.transform.position.y;
            return fallback;

        }

    }     
}
