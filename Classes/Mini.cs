using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini : MonoBehaviour
{
    private GameObject grid;
    public Vector3 initialPosition;
    public int maxMove, movesLeft;
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("MapGrid");
    }
    
}
