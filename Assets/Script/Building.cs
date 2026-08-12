namespace DevelopersHub.ClashOfWhatecer
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Building : MonoBehaviour
    {

        private static Building _instance = null; public static Building instance { get { return _instance; } set { _instance = value; } }
        public static bool IsBeingDragged { get; private set; } = false;



        [System.Serializable]
        public class Level
        {
            public int level = 1;
            public Sprite icon = null;
            public GameObject mesh = null;
        }

        private BuildGrid _grid = null;

        [SerializeField]
        private int _rows = 1; public int rows { get { return _rows; } }
        [SerializeField]
        private int _columns = 1; public int columns { get { return _columns; } }

        [SerializeField]
        private MeshRenderer _baseArea = null;

        [SerializeField] private Level[] _levels = null;

        private int _currentX = 0; public int currentX { get { return _currentX; } }
        private int _currentY = 0; public int currentY { get { return _currentY; } }
        private int _X = 0;
        private int _Y = 0;

        private bool _isDragging = false;           
        private Vector3 _dragBaseLocalPos;          
        private Collider _buildingCollider;



        private void Awake()
        {
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
            Vector3 position = UI_Main.instance._grid.GetCenterPosition(x, y, _rows, _columns);
            transform.position = position;

        }

        public void StartMovingOnGrid()
        {
            _X = _currentX;
            _Y = _currentY;


        }

        public void RemoveFromGrid()
        {
            _instance = null;
            Destroy(gameObject);

        }

        public void UpdateGridPosition(Vector3 baseWorldPos, Vector3 currentWorldPos)
        {
            Vector3 dir = currentWorldPos - baseWorldPos;

            Vector3 localDir = UI_Main.instance._grid.transform.InverseTransformDirection(dir);
            int xDis = Mathf.RoundToInt(localDir.x / UI_Main.instance._grid.cellSize);
            int yDis = Mathf.RoundToInt(localDir.z / UI_Main.instance._grid.cellSize);

            _currentX = _X + xDis;
            _currentY = _Y + yDis;

            Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX, _currentY, _rows, _columns);
            transform.position = position;
        }

        private void Start()
        {
            StartMovingOnGrid();
            PlaceOnGrid(_currentX, _currentY);
            UpdateGridPosition(Vector3.zero, Vector3.zero);




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
            Debug.Log("UI_Main.instance: " + UI_Main.instance);
            Debug.Log("UI_Main.instance._grid: " + UI_Main.instance?._grid);
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.transform.IsChildOf(this.transform) || hit.collider.gameObject == this.gameObject)
                {

                    IsBeingDragged = true;
                    _isDragging = true;
                    StartMovingOnGrid();

                    Vector3 worldPos = GetWorldPositionOnGrid(screenPos);
                    if (worldPos != Vector3.zero)
                    {
                        _dragBaseLocalPos = UI_Main.instance._grid.transform.InverseTransformPoint(worldPos);
                    }
                    else
                    {
                        _isDragging = false;
                    }
                }
            }
        }

        private void OnPointerDrag(Vector2 screenPos)
        {
            Vector3 worldPos = GetWorldPositionOnGrid(screenPos);
            if (worldPos != Vector3.zero)
            {
                Vector3 currentLocalPos = UI_Main.instance._grid.transform.InverseTransformPoint(worldPos);
                UpdateGridPosition(_dragBaseLocalPos, currentLocalPos);
            }
        }

        private void OnPointerUp()
        {
            _isDragging = false;
            IsBeingDragged = false;
            Debug.Log("finish draging: (" + _currentX + ", " + _currentY + ")");
        }

        private Vector3 GetWorldPositionOnGrid(Vector2 screenPos)
        {
            if (Camera.main == null) { Debug.LogError("dont have MainCamera！"); return Vector3.zero; }
            if (UI_Main.instance?._grid == null)
                return Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Plane gridPlane = new Plane(Vector3.up, UI_Main.instance._grid.transform.position);
            if (gridPlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

    }     
}
