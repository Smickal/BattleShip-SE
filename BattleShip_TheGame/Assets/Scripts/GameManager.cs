using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("Player Ships")]
    public GameObject[] ships;
    public int[,] currentPlayerShipPos = new int[11, 11];
    public int totalPlayerSpot;

    [Header("Enemy Ships")]
    private int enemyShipID = 1;
    public int[,] enemyShipHealth = new int[6, 1];
    public int[,] playerShipHealth = new int[6, 1];

    public int[] currentEnemyShipSize = new int[5];
    public int[,] currentEnemyShipPos = new int[11, 11];
    public int totalEnemySpot;
    public GameObject tempProj;
    int currEnemyIndex = 0;
    int orientationRan, locationRanX, locationRanY;


    [Header("Buttons")]
    public Button nextButton;
    public Button leftRotateButton;
    public Button rightRotateButton;
    public Button resetButton;
    public Button randomizeButton;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] GameObject docks;
    [SerializeField] TextMeshProUGUI shipTypeText;
    [SerializeField] GameObject shipModelText;

    string preparingPhaseText = "Preparing Phase";
    string gameBeginText = "Game Begin!";
    string spawningEnemiesText = "Spawning Enemies";

    [Header("Tiles")]
    [SerializeField] GameObject prepPhaseTiles;
    [SerializeField] GameObject gamePhaseTiles;

    private bool setupComplete = false;
    private int shipIndex = 0;
    private bool firstShipSpawned = false;
    private bool TileColorCheck = false;
    private bool isRandomizeButtonPressed = false;

    private ShipScript shipScript;
    private GameObject currentTile = null;
    private GameObject shipTile = null;

    private Vector2[] boatStartingLocation;
    GamePhase gamePhase;

    private void Awake()
    {
        //Initialize buttons
        nextButton.onClick.AddListener(() => NextShipClicked());
        leftRotateButton.onClick.AddListener(() => RotateLeftCurrentShip());
        rightRotateButton.onClick.AddListener(() => RotateRightCurrentShip());
        resetButton.onClick.AddListener(() => ResetCurrentSetup());
        randomizeButton.onClick.AddListener(() => RandomizeAllShips());


        foreach (int shipSize in currentEnemyShipSize)
        {
            totalEnemySpot += shipSize;
        }

        foreach (GameObject ship in ships)
        {
            totalPlayerSpot += ship.GetComponent<ShipScript>().shipSize;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        boatStartingLocation = new Vector2[ships.Length];
        shipScript = ships[shipIndex].GetComponent<ShipScript>();


        for (int i = 0; i < ships.Length; i++)
        {
            boatStartingLocation[i] = ships[i].transform.position;
        }

        //initialize variables
        currEnemyIndex = 0;
        Random.InitState((int)System.DateTime.Now.Ticks);

        gamePhaseTiles.SetActive(false);
        gamePhase = GetComponent<GamePhase>();

        DisplayCurrentSelectedShip(ships[0].gameObject);

        leftRotateButton.gameObject.SetActive(false);
        rightRotateButton.gameObject.SetActive(false);


    }

    void StartGame()
    {

            DisableRandomizeButton();
            nextButton.onClick.RemoveAllListeners();
            PrepGamePhase();
            setupComplete = true;

    }
    void RandomizeAllShips()
    {
        ResetCurrentSetup();
        isRandomizeButtonPressed = true;
        for(int i = 0; i < ships.Length; i++)
        {
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            //Set RandomTile Placement
            //Place that ship
            int shipRotation = RandomizeShipOrientation();
            //Debug.Log("ShipRotation = " + shipRotation);
            GameObject tile =  CheckForRandomizePlayerLocation(shipRotation, shipScript.shipSize);
            shipScript.SetShipRotation(shipRotation);
            PlaceShip(tile);
            PrintShipToArray(shipScript, tile);
            shipIndex++;
        }

        
    }

    int RandomizeShipOrientation()
    {
        orientationRan = Random.Range(0, 4);

        if (orientationRan == 0) return 0;  //arah kebawah 
        else if (orientationRan == 1) return -90;//arah kekiri
        else if (orientationRan == 2) return 90;//arah kekanan
        else if (orientationRan == 3) return -180;//arah keatas
        else return 0;
    }

    GameObject CheckForRandomizePlayerLocation(int tempRotation, int shipSize)
    {
        int tempX, tempY;
        int posX, posY;
        GameObject tile = null;
        do
        {
            tempX = Random.Range(0, 10);
            tempY = Random.Range(0, 10);
            //Debug.Log("Ship pos: " + ((char)(tempX + 65)).ToString() + " , " + (tempY+1));
            if (currentPlayerShipPos[tempX, tempY] != 0) continue;

            int tilePos = tempX + (tempY * 10);
            //Debug.Log("Tilenum = " + tilePos);

            if (tempRotation == 0) //Check MAX Placed Area (BAWAH)
            {
                bool verticalCheckBawah = tilePos - (10 * (shipSize - 1)) >= 0;
                if (!verticalCheckBawah) continue;
            }
            else if (tempRotation == 90 ) //Check MAX Placed Area (Kanan)
            {
                bool horizontalCheckKanan = 9 - (tilePos % 10) >= shipSize - 1;
                if (!horizontalCheckKanan) continue;
            }
            else if (tempRotation == -90) //Check MAX Placed Area (Kiri)
            {
                bool horizontalCheckKiri = (tilePos % 10) >= shipSize - 1;
                if (!horizontalCheckKiri) continue;
            }
            else if (tempRotation == -180) //Check MAX Placed Area (Atas)
            {
                bool verticalCheckAtas = tilePos + (10 * (shipSize - 1)) <= 99;
                if (!verticalCheckAtas) continue;
            }


            posX = tempX;
            posY = tempY;

            bool checkCollideWithOtherShip = false;

            if(tempRotation == 0)
            {
                for(int i = 0; i < shipSize; i++)
                {
                    if (currentPlayerShipPos[tempX, tempY] > 0)
                    {
                        checkCollideWithOtherShip = true;
                        break;
                    }
                    tempY--;                           
                }
            }
            else if(tempRotation == -90)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    if (currentPlayerShipPos[tempX, tempY] > 0)
                    {
                        checkCollideWithOtherShip = true;
                        break;
                    }
                    tempX--;
                }
            }
            else if(tempRotation == 90)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    if (currentPlayerShipPos[tempX, tempY] > 0)
                    {
                        checkCollideWithOtherShip = true;
                        break;
                    }
                    tempX++;
                }
            }
            else if(tempRotation == -180)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    if (currentPlayerShipPos[tempX, tempY] > 0)
                    {
                        checkCollideWithOtherShip = true;
                        break;
                    }
                    tempY++;
                }
            }


            if(!checkCollideWithOtherShip)
            {
                break;
            }


        } while (true);

        int tileTempNum = posX + (posY * 10);
        //Debug.Log("TileTempNum :  " + tileTempNum);
        //Search for desired tile
        foreach (TileScript tileScript in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tileScript.tileNumber == tileTempNum)
            {
                tile = tileScript.gameObject;
                return tile;
                
            }
        }

        return null;
    }


    void CheckShipForNextRotation()
    {
        if (shipTile && shipScript)
        {

            if (CheckRotateLeftShipCollide(shipTile))
            {
                leftRotateButton.gameObject.SetActive(true);
            }
            else
            {
                leftRotateButton.gameObject.SetActive(false);
            }

            if (CheckRotateRightShipCollide(shipTile))
            {
                rightRotateButton.gameObject.SetActive(true);
            }
            else
            {
                rightRotateButton.gameObject.SetActive(false);
            }
        }
        else
        {
            rightRotateButton.gameObject.SetActive(false);
            leftRotateButton.gameObject.SetActive(false);
        }
    }

    void PrepGamePhase()
    {
        //hide / destroy docks
        Destroy(docks);
        //Change Location and size for prepTiles
        prepPhaseTiles.transform.localPosition = new Vector2(15f, 5f);
        prepPhaseTiles.transform.localScale = Vector2.one * 0.5f;
        //disable Interactfor prepTiles
        foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.DisableInteractive();
        }
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.EnableGamePhase();
        }

        //hide all button except next
        resetButton.gameObject.SetActive(false);
        leftRotateButton.gameObject.SetActive(false);
        rightRotateButton.gameObject.SetActive(false);
        //un-hide gameTiles
        gamePhaseTiles.SetActive(true);
        //start Game
        gamePhase.StartGame(currentPlayerShipPos, currentEnemyShipPos);
    }

    void ResetCurrentSetup()
    {
        leftRotateButton.gameObject.SetActive(false);
        rightRotateButton.gameObject.SetActive(false);
        EnableRandomizeButton();

        isRandomizeButtonPressed = false;

        shipIndex = 0;
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].transform.position = boatStartingLocation[i];
            ships[i].gameObject.GetComponent<ShipScript>().ResetRotation();
        }
        currentTile = null;
        shipScript = ships[0].GetComponent<ShipScript>();
        //reset array
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                currentPlayerShipPos[j, i] = 0;
            }
        }
    }

    void NextShipClicked()
    {
        if (!currentTile && !isRandomizeButtonPressed) return;
        //Debug.Log("Ship Index: " + shipIndex + ",Ship Length" + ships.Length);

        if (shipIndex < ships.Length - 1)
        {
            //---Check if a Ship Collide with another ship, or Out of Bounds
            //Ship Collide
            PrintShipToArray(shipScript, currentTile);
            //PrintArray(currentPlayerShipPos);
            shipIndex++;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            currentTile = null;
            // shipScript.FlashColor(Color.yellow);
        }
        else
        {
            //Done placing all ships
            //Hide CurrentShip Type
            shipTypeText.gameObject.SetActive(false);
            shipModelText.SetActive(false);
            //SPAWNING ALL ENEMIES
            CreateEnemy();
            StartGame();
            return;
        }


        //Debug.Log("Current ship Index: " + shipIndex);

    }


    private bool CheckRotateLeftShipCollide(GameObject tile)
    {
        ShipScript currentShip = ships[shipIndex].GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;

        bool checkAvailableforSpawn = false;
        int tempX = tilePos % 10;
        int tempY = tilePos / 10;
        int currPosY = tilePos;

        int tempLeft = (int)currentShip.currentRotation + 90;

        if (tempLeft == 0 || tempLeft == 360) //Check Kebawah
        {
            bool verticalCheckBawah = tilePos - (10 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;

            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
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
        else if (tempLeft == 90 || tempLeft == -270) //Check kanan
        {
            bool horizontalCheckKanan = 9 - (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;

            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX++;
            }
        }
        else if (tempLeft == -90 || tempLeft == 270) //Check kiri
        {
            bool horizontalCheckKiri = (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;

            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX--;
            }
        }
        else if (tempLeft == 180 || tempLeft == -180) // check Atas
        {
            bool verticalCheckAtas = tilePos + (10 * (currentShip.shipSize - 1)) <= 99;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;

            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY += 10;

                }
                tempY++;
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

    private bool CheckRotateRightShipCollide(GameObject tile)
    {
        ShipScript currentShip = ships[shipIndex].GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
        
        bool checkAvailableforSpawn = false;
        int tempX = tilePos % 10;
        int tempY = tilePos / 10;
        int currPosY = tilePos;

        int tempRight = (int)currentShip.currentRotation - 90;

        if (tempRight == 0 || tempRight == -360) //Check Kebawah
        {
            bool verticalCheckBawah = tilePos - (10 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;

            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
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
        else if (tempRight == 90 || tempRight == -270) //Check kanan
        {
            bool horizontalCheckKanan = 9 - (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;

            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX++;
            }
        }
        else if (tempRight == -90 || tempRight == 270) //Check kiri
        {
            bool horizontalCheckKiri = (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;

            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX--;
            }
        }
        else if (tempRight == 180 || tempRight == -180) // check Atas
        {
            bool verticalCheckAtas = tilePos + (10 * (currentShip.shipSize - 1)) <= 99;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;

            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY += 10;

                }
                tempY++;
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

    public bool CheckShipOutOfbounds(GameObject tile)
    {
        if (shipIndex > 4) return false;
        ShipScript currentShip = ships[shipIndex].GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
        bool checkAvailableforSpawn = false;
        int tempX = tilePos % 10;
        int tempY = tilePos / 10;
        int currPosY = tilePos;


        if (shipScript.currentRotation == 0) //Check MAX Placed Area (BAWAH)
        {
            bool verticalCheckBawah = tilePos - (10 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;
        }
        else if (shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Check MAX Placed Area (Kanan)
        {
            bool horizontalCheckKanan = 9 - (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;
        }
        else if (shipScript.currentRotation == -90 || shipScript.currentRotation == 270) //Check MAX Placed Area (Kiri)
        {
            bool horizontalCheckKiri = (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;
        }
        else if (shipScript.currentRotation == 180 || shipScript.currentRotation == -180) //Check MAX Placed Area (Atas)
        {
            bool verticalCheckAtas = tilePos + (10 * (currentShip.shipSize - 1)) <= 99;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;
        }



        if (shipScript.currentRotation == 0) //Check Kebawah
        {
            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
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
        else if (shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Check kanan
        {
            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX++;
            }
        }
        else if (shipScript.currentRotation == -90 || shipScript.currentRotation == 270) //Check kiri
        {
            for (int i = 1; i <= currentShip.shipSize; i++)
            {
                //check ada ship ?
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                tempX--;
            }
        }
        else if (shipScript.currentRotation == 180 || shipScript.currentRotation == -180) // check Atas
        {
            for (int i = 0; i < currentShip.shipSize; i++)
            {
                //Check ada Ship? 
                if (currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerShipPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY += 10;

                }
                tempY++;
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

    void RotateLeftCurrentShip()
    { 
        shipScript.RotateLeftCurrentShip();
        CheckShipForNextRotation();
    }

    void RotateRightCurrentShip()
    {
        shipScript.RotateRightCurrentShip();
        CheckShipForNextRotation();
    }


    public void TileClicked(GameObject tile)
    {
        if (!setupComplete)
        {
            currentTile = tile;
            //Debug.Log(currentTile.GetComponent<TileScript>().tileNumber);
            //Dont spawn if there's a ship there and out of bounds
            if(CheckShipPlace(tile) && CheckShipOutOfbounds(tile))
            {
                //hide randomize button if a ship has been placed once
                if(!firstShipSpawned)
                {
                    DisableRandomizeButton();
                    firstShipSpawned = true;
                }         
                shipTile = tile;
                CheckShipForNextRotation();
                PlaceShip(tile);
                SetShipClickedTile(tile);
            }
        }
    }

    bool CheckShipPlace(GameObject tile)
    {
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
        int tempX = tilePos % 10;
        int tempY = tilePos / 10;

        if(currentPlayerShipPos[tempX, tempY] > 0) return false;
        else return true;
    }



    void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        ships[shipIndex].transform.SetParent(tile.transform);
        ships[shipIndex].transform.localPosition = Vector3.zero + shipScript.GettOffset();
        
    }

    void PrintShipToArray(ShipScript shipScript, GameObject tile)
    {
        int currPos = tile.GetComponent<TileScript>().tileNumber;
        int tempY = currPos / 10;
        int tempX = currPos % 10;


        if (shipScript.currentRotation == 0) //Save Kebawah
        {
            //SaveVerticalToArray();

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipID;
                playerShipHealth[shipScript.shipID, 0] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY--;
            }
        }
        else if(shipScript.currentRotation == -90 || shipScript.currentRotation == 270) // Save kekiri
        {
            //saveHorizontalToArray();

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipID;
                playerShipHealth[shipScript.shipID, 0] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX--;
            }
        }
        else if(shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Save Kekanan
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipID;
                playerShipHealth[shipScript.shipID, 0] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX++;
            }
        }
        else if(shipScript.currentRotation == 180 || shipScript.currentRotation == -180) //Save Keatas
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipID;
                playerShipHealth[shipScript.shipID, 0] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY++;
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

            //Debug.Log("Current Choosed Pos: " + locationRanX + " " + locationRanY);
            // Debug.Log("CURRENT ENEMY SHIP SIZE: " + currentEnemyShipSize[currEnemyIndex]);
            for (int i = 0; i < tempSize; i++)
            {
                currentEnemyShipPos[locationRanX, locationRanY] = enemyShipID;
                enemyShipHealth[enemyShipID, 0] = tempSize;
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

            //Debug.Log("Current Choosed Pos: " + locationRanX + " " + locationRanY);
            for (int i = 0; i < tempSize; i++)
            {
                currentEnemyShipPos[locationRanX, locationRanY] = enemyShipID;
                enemyShipHealth[enemyShipID, 0] = tempSize;
                //Debug.Log("Loc: " + locationRanX + " " + locationRanY); ;
                Instantiate(tempProj, new Vector2(locationRanX * 1.2f, locationRanY * 1.2f), Quaternion.identity, gameObject.transform);
                locationRanY++;
            }


        }
        //Debug.Log(enemyShipHealth[enemyShipID, 0]);

        currEnemyIndex++;
        enemyShipID++;
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
                //Debug.Log("X: " + temp);
                if (temp < 9f) checkForSize = true;
            }
            else if (orientationRan == 1)
            {
                float temp = locationRanY + currentEnemyShipSize[currEnemyIndex];
                //Debug.Log("Y: " + temp);
                if (temp < 9f) checkForSize = true;
            }
        }
    }

    void CreateEnemy()
    {      
        for (int i = 0; i < 5; i++)
        {
            int tempSize = currentEnemyShipSize[currEnemyIndex];
            CheckForEnemySpawnAndWrite(tempSize);
        }
    }

    void DisplayCurrentSelectedShip(GameObject ship)
    {
        shipTypeText.text = ship.gameObject.name + " , " + ship.GetComponent<ShipScript>().shipSize;
        
    }

    public int GetTotalEnemySpot()
    {
        return totalEnemySpot;
    }

    public int GetTotalPlayerSpot()
    {
        return totalPlayerSpot;
    }

    public bool GetTileColorCheck()
    {
        return TileColorCheck;
    }


    void DisableRandomizeButton()
    {
        randomizeButton.gameObject.SetActive(false);
    }

    void EnableRandomizeButton()
    {
        randomizeButton.gameObject.SetActive(true);
    }
}
