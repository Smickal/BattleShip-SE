using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class GameManager : MonoBehaviour
{
    [Header("Player Ships")]
    public GameObject[] ships;
    public int[,] currentPlayerShipPos = new int[11, 11];

    [Header("Enemy Ships")]
    public int[] currentEnemyShipSize = new int[5];
    public int[,] currentEnemyShipPos = new int[11, 11];
    public GameObject tempProj;
    int currEnemyIndex = 0;
    int orientationRan, locationRanX, locationRanY;


    [Header("HUD")]
    public Button nextButton;
    public Button rotateButton;
    public Button resetButton;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private int shipIndex = 0;
    private bool hasPlacedShip = false;

    private ShipScript shipScript;
    private GameObject currentTile = null;
    private GameObject PinPointPos = null;

    private Vector2[] boatStartingLocation;

    // Start is called before the first frame update
    void Start()
    {
        boatStartingLocation = new Vector2[ships.Length];
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextButton.onClick.AddListener(() => NextShipClicked());
        rotateButton.onClick.AddListener(() => RotateCurrentShip());
        resetButton.onClick.AddListener(() => ResetCurrentSetup());

        for(int i = 0; i < ships.Length; i++)
        {
            boatStartingLocation[i] = ships[i].transform.position;
        }

        //initialize variables
        currEnemyIndex = 0;
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    void ResetCurrentSetup()
    {
        shipIndex = 0;
        for(int i = 0; i < ships.Length; i++)
        {
            ships[i].transform.position = boatStartingLocation[i];
            ships[i].gameObject.GetComponent<ShipScript>().ResetRotation();
            hasPlacedShip = false;
            currentTile = null;
            shipScript = null;
            PinPointPos = null;
        }
        //reset array
        for(int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                currentPlayerShipPos[j, i] = 0;
            }
         }
    }

    void NextShipClicked()
    {
        if(shipIndex <= ships.Length - 2)
        {
            PrintShipToArray(shipScript, currentTile);
            PrintArray(currentPlayerShipPos);


            shipIndex++;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            hasPlacedShip = false;
            currentTile = null;
            PinPointPos = null;
            // shipScript.FlashColor(Color.yellow);
        }
    }

    void PrintArray(int[,] miaw)
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j <11; j++)
            {
                if(miaw[i,j] != 0)
                    Debug.Log("(" + i + "," + j + ")= " + miaw[i, j].ToString());
            }
        }
    }
    
    void RotateCurrentShip()
    {
        if (!hasPlacedShip) return;
        int PointSpawned = PinPointPos.GetComponent<TileScript>().tileNumber;
        //Check Bisa muter ato kgk
        //Debug.Log(PinPointPos.GetComponent<TileScript>().tileNumber%10 + " " + (shipScript.shipSize - 1));
        if (shipScript.shipSize - 1 > PointSpawned % 10) return;


        //check Ada kapal kgk pas mau muter
        int tempX = PointSpawned % 10;
        int tempY = PointSpawned / 10;
        
        if(shipScript.isShipVertical)
        {
            //check for horizontal
            for (int i = 0; i < shipScript.shipSize; i++)
            {
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    return;
                }
                tempX--;
            }
        }
        else
        {
            //check for vertical
            tempX = PointSpawned % 10;
            tempY = PointSpawned / 10;
            if (shipScript.shipSize - 1 > tempY) return;

            for (int i = 0; i < shipScript.shipSize; i++)
            {
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    
                        return;
                }
                tempY--;
            }
        }
        //Rotate Kapal
        shipScript.RotateCurrentShip();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TileClicked(GameObject tile)
    {
        if(setupComplete && playerTurn)
        {
            //drop a missile
        }
        else if(!setupComplete)
        {
            currentTile = tile;
            if(IsShipAbleToSpawn(currentTile))
            {

                PlaceShip(tile);
                SetShipClickedTile(tile);
                hasPlacedShip = true;
            }

        }
    }

    bool IsShipAbleToSpawn(GameObject tile)
    {
        ShipScript currentShip = ships[shipIndex].GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
       //Debug.Log(tilePos);
        //check for max Vertically
        if(shipScript.isShipVertical)
        {
            bool verticalCheck = tilePos - (10 * (currentShip.shipSize-1)) >= 0;
            if (!verticalCheck) return false;
        }
        else
        {
            bool horizontalCheck = (tilePos % 10) >= currentShip.shipSize-1;
            if (!horizontalCheck) return false;
        }

        //checkfor Ship collide with other Ship
        bool checkAvailableforSpawn = false;
        int currPosY = tilePos;
        int tempY = tilePos / 10;
        int tempX = tilePos % 10;

        if(shipScript.isShipVertical)
        {
            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY -= 10;

                }
                tempY--;
            }
        }
        else
        {
            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY -= 1;

                }
                tempX--;
            }
        }
        

        if (checkAvailableforSpawn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffSetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;

        PinPointPos = tile;
    }

    void PrintShipToArray(ShipScript shipScript, GameObject tile)
    {
        int currPos = tile.GetComponent<TileScript>().tileNumber;
        if (shipScript.isShipVertical)
        {
            //SaveVerticalToArray();
            int tempY = currPos / 10;
            int tempX = currPos % 10;

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY--;
            }
        }
        else
        {
            //saveHorizontalToArray();
            int tempY = currPos / 10;
            int tempX = currPos % 10;

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX--;
            }
        }
    }

    void SetShipClickedTile(GameObject tile)
    {
        shipScript.SetClickedTile(tile);
    }


    void CheckForEnemySpawnAndWrite(int tempSize)
    {
        orientationRan = UnityEngine.Random.Range(0, 2);
        bool check;
        //check for valid place to spawn
        if (orientationRan == 0)
        {
            check = false;
            bool checkForShip = false;
            while (!check)
            {
                //check for place + size
                CheckForSize();
                //Debug.Log("Random: " + locationRanX + " " + locationRanY);
                int tempX = locationRanX;
                for (int i = 0; i < tempSize; i++)
                {
                   // Debug.Log(tempX + " " + locationRanY);
                    if (currentEnemyShipPos[tempX, locationRanY] > 0)
                    {
                        checkForShip = true;
                        break;
                    }
                    tempX++;
                }
                if (checkForShip)
                {
                    check = false;
                    checkForShip = false;
                }
                else check = true;
            }

            Debug.Log("Current Choosed Pos: "+ locationRanX + " " + locationRanY);
           // Debug.Log("CURRENT ENEMY SHIP SIZE: " + currentEnemyShipSize[currEnemyIndex]);
            for (int i = 0; i < tempSize; i++)
            {
                currentEnemyShipPos[locationRanX, locationRanY] = tempSize;
                //Debug.Log("Loc: " +locationRanX +" " + locationRanY); ;
                Instantiate(tempProj, new Vector2(locationRanX * 1.2f, locationRanY * 1.2f), Quaternion.identity, gameObject.transform);
                locationRanX++;
            }
        }
        else
        {
            ////check For Vertical
            check = false;
            bool checkForShip = false;
            while (!check)
            {
                //check for horizontal
                CheckForSize();
                //Debug.Log("Random: " + locationRanX + " " + locationRanY);
                int tempY = locationRanY;
                for (int i = 0; i < tempSize; i++)
                {
                    //Debug.Log(locationRanX + " " + tempY);
                    if (currentEnemyShipPos[locationRanX, tempY] > 0)
                    {
                        checkForShip = true;
                        break;
                    }
                    tempY++;
                }
                if (checkForShip)
                {
                    check = false;
                    checkForShip = false;
                }
                else check = true;
            }

            Debug.Log("Current Choosed Pos: " + locationRanX + " " + locationRanY);
            for (int i = 0; i < tempSize; i++)
            {
                currentEnemyShipPos[locationRanX, locationRanY] = tempSize;
                //Debug.Log("Loc: " + locationRanX + " " + locationRanY); ;
                Instantiate(tempProj, new Vector2(locationRanX * 1.2f, locationRanY * 1.2f), Quaternion.identity, gameObject.transform);
                locationRanY++;
            }


        }
        currEnemyIndex++;
    }

    private void CheckForSize()
    {
        bool checkForSize = false;
        while (!checkForSize)
        {
            locationRanX = UnityEngine.Random.Range(0, 10);
            locationRanY = UnityEngine.Random.Range(0, 10);
            
            //check for logical spawn !
            if (orientationRan == 0)
            {
                float temp = locationRanX + currentEnemyShipSize[currEnemyIndex];
                Debug.Log("X: " + temp);
                if (temp < 9f) checkForSize = true;
            }
            else if (orientationRan == 1)
            {
                float temp = locationRanY + currentEnemyShipSize[currEnemyIndex];
                Debug.Log("Y: " + temp);
                if (temp < 9f) checkForSize = true;
            }
        }
    }

    void CreateEnemy()
    {
        for(int i = 0; i < 5; i++)
        {
            //check that spot
            //randomize vertical or horizontal spot
            //randomize a spot
            // 0 = Horizontal, 1 = Vertical
            int tempSize = currentEnemyShipSize[currEnemyIndex];
            CheckForEnemySpawnAndWrite(tempSize);
            
            //get Current Index for ship size
            //note that spot
        }
    }
}
