using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [System.NonSerialized]
    public PlayerTurn currentTurn;

    private MapGrid gridScript;
    private GameObject grid;
    public GameObject startingCell, playerObject;
    private PlayerCharacter pScript;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.FindWithTag("MapGrid");
        gridScript = (MapGrid)grid.GetComponent(typeof(MapGrid));

        pScript = (PlayerCharacter)playerObject.GetComponent(typeof(PlayerCharacter));
    }

    // Update is called once per frame
    void Update()
    {
    }

    //public because I might need to call it in case of softlock
    public void StartPlayerTurn()
    {
        currentTurn = new PlayerTurn(pScript);
    }

    private void PlacePlayerAtStart()
    {
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        //Check if a playercharacter exists
        if(foundPlayers.Length > 0){
            //Player exists, place at starting cell
            GameObject foundPlayer = foundPlayers[0];

            //Set player position before snapping to grid
            foundPlayer.transform.position = startingCell.transform.position;
            Debug.Log("Snapping to : " + foundPlayer.transform.position.x  + " , " + foundPlayer.transform.position.z);

            gridScript.SnapToGrid(foundPlayer);
        }
        else{
            //Player doesn't exist
        }
    }
    //######## Turn Class ####################################################

    public class PlayerTurn
    {
        public int atksLeft;
        public bool turnOver = false;

        public PlayerTurn(PlayerCharacter player)
        {
            atksLeft = player.AttacksInTurn;
            player.MovesLeft = (int)(player.Speed / 5);
            //unlock menu
        }
        public void EndTurn()
        {
            //Lock menu
            turnOver = true;
        }
    }

}
