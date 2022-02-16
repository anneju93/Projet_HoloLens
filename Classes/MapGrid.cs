using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private GameObject cellTemplate;
    private int[,] gridArray;
    private Cell[,] Cells;

    void Start()
    {
        gridArray = new int[width, height];
        Cells = new Cell[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++){
            for(int z = 0; z < gridArray.GetLength(1); z++){
                CreateNewCell(x, z);
            }
        }
    }

    private Vector3 GetWorldPosition(float x, float z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }

    public Vector3 GetGridPosition(Vector3 targetPosition)
    {
        return targetPosition / cellSize;
    }

    private void CreateNewCell(int x, int z)
    {
        GameObject newCellGameObject = GameObject.Instantiate(cellTemplate, GetWorldPosition(x, z), cellTemplate.transform.rotation);

        //This line is unique to the asset used as by default it is always twice the size it should be
        newCellGameObject.transform.localScale = new Vector3(cellSize / 2, cellSize / 2, cellSize / 2);

        Cell newCell = new Cell(newCellGameObject, x, z);

        Cells[x, z] = newCell;
    }

    public void SnapToGrid(GameObject target)
    {
        Mini scriptT = (Mini)target.GetComponent(typeof(Mini));
        Vector3 gridPos = GetGridPosition(target.transform.position);

        try
        {
            if (!Cells[(int)gridPos.x, (int)gridPos.z].SnapTo(target))
            {
                //Selected cell isn't within range for player, return to initial position
                target.transform.position = scriptT.initialPosition;
            }
        }
        catch
        {
            //Selected cell is outside the grid, return to initial position
            target.transform.position = scriptT.initialPosition;
        }
    }

    public void GenerateMoveMap(GameObject target)
    {
        Mini scriptT = (Mini)target.GetComponent(typeof(Mini));
        Vector3 gridPos = GetGridPosition(scriptT.initialPosition);

        try
        {
            Cells[(int)gridPos.x, (int)gridPos.z].MapForMovement(scriptT.movesLeft);
        }
        catch
        {
            //Target started outside grid, reset position
            Cells[0, 0].SnapTo(target);
        }
    }

    public void ResetGreen()
    {
        for (int x = 0; x < Cells.GetUpperBound(0); x++)
        {
            for (int z = 0; z < Cells.GetUpperBound(1); z++)
            {
                Cells[x, z].MakeGreen(false);
            }
        }
    }

    public List<Cell> GetCellCross(int x, int z)
    {
        List<Cell> cross = new List<Cell>();

        for (int i = -1; i <= 1; i++)
        {
            if (i != 0)
            {
                try
                {
                    cross.Add(Cells[x + i, z]);
                }
                catch
                {
                    //cette case est out of bounds, on la passe
                }
            }
        }

        for (int i = -1; i <= 1; i++)
        {
            if (i != 0)
            {
                try
                {
                    cross.Add(Cells[x, z + i]);
                }
                catch
                {
                    //cette case est out of bounds, on la passe
                }
            }
        }

        return cross;
    }

    //################################################################################################################

    public class Cell
    {
        public int x, z;
        public MapGrid grid;
        public GameObject cellObject;
        public bool ValidForMovement, isGreen = false;

        public Cell(GameObject obj, int gridPosX, int gridPosZ)
        {
            cellObject = obj;
            x = gridPosX;
            z = gridPosZ;
            ValidForMovement = true;
        }

        public bool SnapTo(GameObject target)
        {
            if (isGreen)
            {
                Mini scriptT = (Mini)target.GetComponent(typeof(Mini));

                Vector3 worldPos = grid.GetWorldPosition(x, z);
                Vector3 initialGridPos = grid.GetGridPosition(scriptT.initialPosition);

                target.transform.position = new Vector3(worldPos.x + grid.cellSize / 2,
                                                        0,
                                                        worldPos.z + grid.cellSize / 2);

                int GridDistance = (int)(Mathf.Abs(scriptT.initialPosition.x - x) + Mathf.Abs(scriptT.initialPosition.z - z));
                scriptT.movesLeft -= GridDistance * 5;

                grid.ResetGreen();
                return true;
            }
            else return false;
        }

        public void MapForMovement(int movesLeft)
        {
            if(ValidForMovement && !isGreen)
            {
                MakeGreen(true);
                if (movesLeft != 0)
                {
                    foreach(Cell cell in grid.GetCellCross(x, z))
                    {
                        cell.MapForMovement(movesLeft - 1);
                    }
                }
            }
        }

        public void MakeGreen(bool green)
        {
            if (green)
            {
                isGreen = true;
                //Make cell Gameobject green
                //This is only for the purpose of visibility as I try to find a way to turn it green
                cellObject.transform.rotation = Quaternion.Euler(90, 0, 90);
            }
            else
            {
                isGreen = false;
                //Return GameObject to normal
                cellObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
    }
}
