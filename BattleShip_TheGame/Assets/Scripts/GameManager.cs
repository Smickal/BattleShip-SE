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
    public int[] currentEnemyShipSize = new int[5];
    public int[,] currentEnemyShipPos = new int[11, 11];
    public int totalEnemySpot;
    public GameObject tempProj;
    int currEnemyIndex = 0;
    int orientationRan, locationRanX, locationRanY;


    [Header("HUD")]
    public Button nextButton;
    public Button leftRotateButton;
    public Button rightRotateButton;
    public Button resetButton;

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
    private bool playerTurn = true;
    private int shipIndex = 0;
    private bool hasPlacedShip = false;
    private bool gameIsStarting = false;

    private ShipScript shipScript;
    private GameObject currentTile = null;
    private GameObject PinPointPos = null;

    private Vector2[] boatStartingLocation;
    GamePhase gamePhase;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => NextShipClicked());
        leftRotateButton.onClick.AddListener(() => RotateLeftCurrentShip());
        rightRotateButton.onClick.AddListener(() => RotateRightCurrentShip());
        resetButton.onClick.AddListener(() => ResetCurrentSetup());

        foreach (int shipSize in currentEnemyShipSize)
        {
            totalEnemySpot += shipSize;
        }

        foreach(GameObject ship in ships)
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

        //Count all enemy Ship
        

    }

    void Update()
    {
        if (gameIsStarting)
        {
            nextButton.onClick.RemoveAllListeners();
            PrepGamePhase();
            setupComplete = true;
            gameIsStarting = false;
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
        shipIndex = 0;
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].transform.position = boatStartingLocation[i];
            ships[i].gameObject.GetComponent<ShipScript>().ResetRotation();
        }
        hasPlacedShip = false;
        currentTile = null;
        shipScript = ships[0].GetComponent<ShipScript>();
        PinPointPos = null;
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
        if (!currentTile) return; 
        
        if (shipIndex <= ships.Length - 1)
        {
            //---Check if a Ship Collide with another ship, or Out of Bounds
            //Ship Collide


            PrintShipToArray(shipScript, currentTile);
            
            //PrintArray(currentPlayerShipPos);

            if (shipIndex == ships.Length - 1)
            {
                //Done placing all ships
                //Hide CurrentShip Type
                shipTypeText.gameObject.SetActive(false);
                shipModelText.SetActive(false);
                //SPAWNING ALL ENEMIES
                CreateEnemy();
                gameIsStarting = true;
                return;
            }
            shipIndex++;
            shipScript = ships[shipIndex].GetComponent<ShipScript>();
            DisplayCurrentSelectedShip(ships[shipIndex].gameObject);
            hasPlacedShip = false;
            currentTile = null;
            PinPointPos = null;
            // shipScript.FlashColor(Color.yellow);
        }


        //Debug.Log("Current ship Index: " + shipIndex);
        
    }


    void PrintArray(int[,] miaw)
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (miaw[i, j] != 0)
                    Debug.Log("(" + i + "," + j + ")= " + miaw[i, j].ToString());
            }
        }
    }



    void RotateLeftCurrentShip()
    {
        shipScript.RotateLeftCurrentShip();

        if (CheckForHideNextButton(currentTile))
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    void RotateRightCurrentShip()
    {
        shipScript.RotateRightCurrentShip();

        if (CheckForHideNextButton(currentTile))
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
        }
    }


    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            //drop a missile
        }
        else if (!setupComplete)
        {
            currentTile = tile;

            //Dont spawn if there's a ship there
            if(CheckShipPlace(tile))
            {
                PlaceShip(tile);
                SetShipClickedTile(tile);
                hasPlacedShip = true;
            }

            if(CheckForHideNextButton(tile))
            {
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(false);
            }


        }
    }

    bool CheckShipPlace(GameObject tile)
    {
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
        int tempX = tilePos % 10;
        int tempY = tilePos / 10;

        if(currentPlayerShipPos[tempX, tempY] > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }




    bool CheckForHideNextButton(GameObject tile)
    {
        ShipScript currentShip = ships[shipIndex].GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;
        //Debug.Log(tilePos);

       // Debug.Log("CurrentShipRotation = "  + shipScript.currentRotation);
        //check for max
        if (shipScript.currentRotation == 0) //Check MAX Placed Area (BAWAH)
        {
            bool verticalCheckBawah = tilePos - (10 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;
        }
        else if(shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Check MAX Placed Area (Kanan)
        {
            bool horizontalCheckKanan = 10 - (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;
        }
        else if(shipScript.currentRotation == -90 || shipScript.currentRotation == 270) //Check MAX Placed Area (Kiri)
        {
            bool horizontalCheckKiri = (tilePos % 10) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;
        }
        else if (shipScript.currentRotation == 180 || shipScript.currentRotation == -180) //Check MAX Placed Area (Atas)
        {
            bool verticalCheckAtas = tilePos + (10 * (currentShip.shipSize - 1)) <= 100;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;
        }

        //checkfor Ship collide with other Ship
        bool checkAvailableforSpawn = false;
        int currPosY = tilePos;
        int tempY = tilePos / 10;
        int tempX = tilePos % 10;


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
        else if(shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Check kanan
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
        else if(shipScript.currentRotation == 180 || shipScript.currentRotation == -180) // check Atas
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

    void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        ships[shipIndex].transform.SetParent(tile.transform);
        ships[shipIndex].transform.localPosition = Vector3.zero + shipScript.GettOffset();
        

        PinPointPos = tile;
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
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY--;
            }
        }
        else if(shipScript.currentRotation == -90 || shipScript.currentRotation == 270) // Save kekiri
        {
            //saveHorizontalToArray();

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX--;
            }
        }
        else if(shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Save Kekanan
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX++;
            }
        }
        else if(shipScript.currentRotation == 180 || shipScript.currentRotation == -180) //Save Keatas
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerShipPos[tempX, tempY] = shipScript.shipSize;
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

            //Debug.Log("Current Choosed Pos: " + locationRanX + " " + locationRanY);
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
}
