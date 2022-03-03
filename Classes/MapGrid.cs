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

        //For testing purposes, creates a wall with a 1 cell wide doorway
        for(int z = 0; z < gridArray.GetLength(1); z++)
        {
            Cells[4, z].ValidForMovement = false;
        }
        Cells[4  , 4].ValidForMovement = true;
    }

    private Vector3 GetWorldPosition(float x, float z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }

    public Vector3 GetGridPosition(Vector3 worldPosition)
    {
        return worldPosition / cellSize;
    }

    private void CreateNewCell(int x, int z)
    {
        GameObject newCellGameObject = GameObject.Instantiate(cellTemplate, GetWorldPosition(x, z), cellTemplate.transform.rotation);

        //This line is unique to the asset used as by default it is always twice the size it should be
        newCellGameObject.transform.localScale = new Vector3(cellSize / 2, cellSize / 2, cellSize / 2);

        Cell newCell = new Cell(newCellGameObject, x, z, this);

        Cells[x, z] = newCell;
    }

    public void SnapToGrid(GameObject target)
    {
        Mini scriptT = (Mini)target.GetComponent(typeof(Mini));
        Vector3 gridPos = GetGridPosition(target.transform.position);

        Debug.Log("Snapping to : " + gridPos.x  + " , " + gridPos.z);

        try
        {
            if (!Cells[(int)gridPos.x, (int)gridPos.z].SnapTo(target))
            {
                //Selected cell isn't within range for player, return to initial position
                target.transform.position = scriptT.initialPosition;
                ResetGreen();
            }
        }
        catch
        {
            //Selected cell is outside the grid, return to initial position
            target.transform.position = scriptT.initialPosition;
            ResetGreen();
        }
    }

    public void GenerateMoveMap(GameObject target)
    {
        Mini scriptT = (Mini)target.GetComponent(typeof(Mini));
        Vector3 gridPos = GetGridPosition(scriptT.initialPosition);

        try
        {
            Cell currentCell = Cells[(int)gridPos.x, (int)gridPos.z];
            currentCell.MiniOnCell = null;
            currentCell.ValidForMovement = true;
            currentCell.MapForMovement(scriptT.MovesLeft);
        }
        catch
        {
            //Target started outside grid, reset position
            Cells[0, 0].SnapTo(target);
        }
    }

    public void ResetGreen()
    {
        for (int x = 0; x <= Cells.GetUpperBound(0); x++)
        {
            for (int z = 0; z <= Cells.GetUpperBound(1); z++)
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
                    //This cell is out of bounds, we do nothing and skip it
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
                    //This cell is out of bounds, we do nothing and skip it
                }
            }
        }

        return cross;
    }

    public List<GameObject> GetMeleeTargets(GameObject attacker)
    {
        Vector3 gridPos = GetGridPosition(attacker.transform.position);

        List<GameObject> targets = new List<GameObject>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (!(x == 0 && z == 0))
                {
                    try
                    {
                        GameObject target = Cells[(int)(gridPos.x + x), (int)(gridPos.z + z)].MiniOnCell;
                        if (target != null)
                        {
                            targets.Add(target);
                        }
                    }
                    catch
                    {
                            //This cell is out of bounds, we do nothing and skip it
                    }
                }
            }
        }

        return targets;
    }

    public void RemoveMiniFromCell(GameObject target)
    {
        Vector3 gridPos = GetGridPosition(target.transform.position);
        Cell targetCell = Cells[(int)gridPos.x, (int)gridPos.z];

        targetCell.MiniOnCell = null;
        targetCell.ValidForMovement = true;
    }

    //################ End of MapGrid Class ################################################################################################

    public class Cell
    {
        public int x, z;
        public MapGrid grid;
        public GameObject cellObject, MiniOnCell;
        public bool ValidForMovement, isGreen = false;

        public Cell(GameObject obj, int gridPosX, int gridPosZ, MapGrid parentGrid)
        {
            cellObject = obj;
            x = gridPosX;
            z = gridPosZ;
            ValidForMovement = true;
            grid = parentGrid;
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
                scriptT.MovesLeft -= GridDistance;

                MiniOnCell = target;
                ValidForMovement = false;

                grid.ResetGreen();
                return true;
            }
            else return false;
        }

        public void MapForMovement(int movesLeft)
        {
            if (ValidForMovement)
            {
                MakeGreen(true);
                if (movesLeft > 0)
                {
                    foreach (Cell cell in grid.GetCellCross(x, z))
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
                cellObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(.095f, .37f, .095f, .47f));
                isGreen = true;
            }
            else
            {
                cellObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(.22f, .22f, .22f, .47f));
                isGreen = false;
            }
        }
    }
}
