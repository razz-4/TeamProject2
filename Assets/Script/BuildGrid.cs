namespace DevelopersHub.ClashOfWhatecer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BuildGrid : MonoBehaviour
    {
        public int _rows = 45;
        public int _columns = 45;
        public float _cellSize = 1f;public float cellSize { get { return _cellSize; } }

        public static BuildGrid Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

#if UNITY_EDITOR

        public Vector3 GetStartPosition(int x, int y)
        {
            Vector3 position = transform.position;
            position += (transform.right.normalized * x * _cellSize) + (transform.forward.normalized * y * _cellSize);
            return position;
        }

        public Vector3 GetCenterPosition(int x, int y, int rows, int columns)
        {
            Vector3 start = GetStartPosition(x, y);
            start += (transform.right.normalized * columns * _cellSize * 0.5f) + (transform.forward.normalized * rows * _cellSize * 0.5f);
            return start;
        }

        public Vector3 GetEndPosition(int x, int y, int rows, int columns)
        {
            Vector3 start = GetStartPosition(x, y);
            start += (transform.right.normalized * columns * _cellSize) + (transform.forward.normalized * rows * _cellSize);
            return start;
        }


        public bool IsWorldPositionInGrid(Vector3 position, int x, int y, int rows, int columns)
        {
            Vector3 localPos = transform.InverseTransformPoint(position);
            Rect rect = new Rect(x, y, rows, columns);
            return rect.Contains(new Vector2(localPos.x, localPos.z));
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i <= _rows; i++)
            {
                Vector3 point = transform.position + transform.forward.normalized * _cellSize * (float)i;
                Gizmos.DrawLine(point, point + transform.right.normalized * _cellSize * (float)_columns);

            }
            for (int i = 0; i <= _columns; i++)
            {
                Vector3 point = transform.position + transform.right.normalized * _cellSize * (float)i;
                Gizmos.DrawLine(point, point + transform.forward.normalized * _cellSize * (float)_rows);

            }
        }

        internal bool IsWorldPositionIsOnPlane(object buildBasePosition, int currentX, int currentY, int rows, int columns)
        {
            throw new NotImplementedException();
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            int x = Mathf.FloorToInt(localPos.x / _cellSize);
            int y = Mathf.FloorToInt(localPos.z / _cellSize);
            return new Vector2Int(x, y);
        }
#endif


    }


}