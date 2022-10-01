using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridScript<TGridObject>
{
    public const int HeatMapMax = 100;
    public const int HeatMapMin = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    TextMesh[,] printedText;
    public bool drawDebug;
    readonly float y = -10f;

    public GridScript(int width, int height, float cellSize, Vector3 originPosition, Func<GridScript<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        printedText = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        drawDebug = false;
        if (drawDebug)
        {

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < gridArray.GetLength(1); z++)
                {
                    printedText[x, z] = CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) , 8, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, z) - new Vector3(cellSize, 0, cellSize) * 0.5f, GetWorldPosition(x, z + 1)  - new Vector3(cellSize, 0, cellSize) * 0.5f, Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z) - new Vector3(cellSize, 0, cellSize) * 0.5f, GetWorldPosition(x + 1, z) - new Vector3(cellSize, 0, cellSize) * 0.5f, Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(width, 0) - new Vector3(cellSize, 0, cellSize) * 0.5f, GetWorldPosition(width, height) - new Vector3(cellSize, 0, cellSize) * 0.5f, Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(0, height) - new Vector3(cellSize, 0, cellSize) * 0.5f, GetWorldPosition(width, height) - new Vector3(cellSize, 0, cellSize) * 0.5f, Color.white, 100f);

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs args) =>
            {
                printedText[args.x, args.z].text = gridArray[args.x, args.z]?.ToString();
            };
        }
    }

    public int GetWidth
    {
        get { return width; }
    }

    public int GetHeight => height;

    public float GetCellSize => cellSize;


    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, y, z) *cellSize + originPosition ;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.RoundToInt((worldPosition - originPosition).z / cellSize);
    }

    void SetGridObject(int x, int z, TGridObject value)
    {
        if(x >= 0 && z >= 0 && x< width && z< height)
        {
            gridArray[x, z] = value;
            printedText[x, z].text = gridArray[x, z].ToString();
            //OnGridValueChangedEventArgs myArgs = new OnGridValueChangedEventArgs();
            //myArgs.x = x;
            //myArgs.y = y;
            //if (OnGridValueChanged != null) { OnGridValueChanged(this, new OnGridValueChangedEventArgs{ x = x, y = y }); } 
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXY(worldPosition, out int x , out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        return default;
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }

    public void TriggerOnGridObjectChange(int x, int z)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, z = z });
    }


    public TextMesh CreateWorldText(string text, Transform parent = null,  Vector3 localPosition = default, int fontSize = 8, Color? color = null, TextAnchor textAnchor = TextAnchor.MiddleCenter , TextAlignment textAlignment = TextAlignment.Center)
    {
        if (color == null) color = Color.white;
        return CreateWorldtext(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment);
    }

    public TextMesh CreateWorldtext(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        transform.rotation = Quaternion.Euler(90f, 0, 0);
        //textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

}

