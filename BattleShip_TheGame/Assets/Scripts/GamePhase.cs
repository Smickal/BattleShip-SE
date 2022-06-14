using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GamePhase : MonoBehaviour
{
    // Start is called before the first frame update
    public int[,] enemyShipHealth = new int[6, 1];
    public int[,] playerShipHealth = new int[6, 1];

    public TileNumbering prepTileNumber;
    public TileNumbering gameTileNumber;

    GameManager gameManager;
    private int[,] currentPlayerPos;
    private int[,] currentEnemyPos;

    private int[,] enemyMarkedSpot = new int[31, 31];
    private int[,] playerMarkedSpot = new int[31, 31];

    public Button skipButton;
    [SerializeField] GameObject playerHitTag;

    private int totalEnemySpot;
    private int totalPlayerSpot;

    RaycastHit2D hit;
    [SerializeField]LayerMask mask;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gridText;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] GameObject textExplanation;
    [SerializeField] TextMeshProUGUI pressAnywhereText;

    [SerializeField] GameObject shipType;
    [SerializeField] TextMeshProUGUI shipTypeText;

    [Header("Tiles")]
    [SerializeField] GameObject gamePhaseTiles;
    [SerializeField] GameObject prepPhaseTiles;

    [Header("End Button")]
    [SerializeField] Button endButton;
    [SerializeField] TextMeshProUGUI EndText;

    [Header("EnemiesTurnUI")]
    [SerializeField] GameObject enemiesTurnUI;
    [SerializeField] TextMeshProUGUI enemiesCurrentGridText;
    [SerializeField] TextMeshProUGUI hitOrMissedText;
    [SerializeField] Slider timeSlider;
    [SerializeField] float enemiesTurnTime = 3f;

    [Header("PopUpText")]
    [SerializeField] GameObject popUptext;
    [SerializeField] TextMeshPro text_popup;
    [SerializeField] float popUpTextTime = 0.5f;

    [Header("PlayerAndEnemyText")]
    [SerializeField] GameObject playerText;
    [SerializeField] TextMeshProUGUI playerShipAliveText;
    [SerializeField] GameObject enemyText;
    [SerializeField] TextMeshProUGUI enemyShipAliveText;
    int currentPlayerShipAlive = 5;
    int currentEnemyShipAlive = 5;

    [Header("ShipAbilitiesButton")]
    [SerializeField] Button shipAbilitiesButton;
    [SerializeField] Button backFromAbilitiesButton;

    [SerializeField] Button moveButton;
    [SerializeField] Button shootButton;
    [SerializeField] Button rotateButton;

    [Header("Abilities Attributes")]
    [SerializeField] GameObject shipAbilitesContainer;
    bool isAShipSelected = false;

    [Header("ShipMoveAbilities")]
    [SerializeField] GameObject moveAbilities;
    [SerializeField] Button forwardButton;
    [SerializeField] Button diagLeftButton;
    [SerializeField] Button diagRightButton;
    bool canShipDiagleft = false;
    bool canShipDiagRight = false;


    [SerializeField] Button resetButton;
    [SerializeField] Button backFromMoveButton;
    [SerializeField] Button confirmMoveButton;

    [Header("ShootAbilites")]
    [SerializeField] GameObject shootAbilities;
    [SerializeField] Button backFromShootButton;
    [SerializeField] Button confirmShootButton;
    RaycastHit2D[] allArea;

    [Header("RotateAbilities")]
    [SerializeField] GameObject rotateAbilities;
    [SerializeField] Button rotateLeftButton;
    [SerializeField] Button rotateRightButton;
    [SerializeField] Button resetRotateButton;

    [SerializeField] Button backFromRotateButton;
    [SerializeField] Button confirmRotateButton;


    GameObject firstPlace;
    GameObject currentPlace;

    TileScript currentClickedTile;

    float currentTime;
    bool isStartTimer = false;

    private GameObject currentTile = null;
    private GameObject currentClickedShip = null;


    bool isPlayerTurn = false;
    int tileX, tileY;


    void Start()
    {
        endButton.gameObject.SetActive(false);
        shipAbilitiesButton.gameObject.SetActive(false);

        gameManager = GetComponent<GameManager>();
        textExplanation.SetActive(false);


        totalEnemySpot = gameManager.GetTotalEnemySpot();
        totalPlayerSpot = gameManager.GetTotalPlayerSpot();

        timeSlider.maxValue = enemiesTurnTime;

        enemiesTurnUI.SetActive(false);

        playerText.SetActive(false);
        enemyText.SetActive(false);
        popUptext.SetActive(false);
    }


    public void StartGame(int[,] playerShipPos, int[,] enemyShipPos)
    {
        playerText.SetActive(true);
        enemyText.SetActive(true);
        
        currentPlayerPos = playerShipPos;
        currentEnemyPos = enemyShipPos;

        skipButton.onClick.AddListener(() => OpponentsTurn());
        shipAbilitiesButton.onClick.AddListener(() => ShipAbilitesButtonPressed());
        backFromAbilitiesButton.onClick.AddListener(() => BackFromAbilitiesPressed());

        //movement buttons
        moveButton.onClick.AddListener(() => MoveButtonPressed());
        backFromMoveButton.onClick.AddListener(() => BackfromMoveButtonPressed());
        forwardButton.onClick.AddListener(() => MoveForwardPressed());
        resetButton.onClick.AddListener(() => ResetCurrentShip());
        diagLeftButton.onClick.AddListener(() => MoveDiagLeftPressed());
        diagRightButton.onClick.AddListener(() => MoveDiagRightPressed());
        confirmMoveButton.onClick.AddListener(() => ConfirmCurrentShipPosition());

        //shooting Button
        shootButton.onClick.AddListener(() => ShootButtonPressed());
        backFromShootButton.onClick.AddListener(() => BackFromShootAbilitiesPressed());
        confirmShootButton.onClick.AddListener(() => PlayerLaunchMissileAtTile());

        //rotate Button
        rotateButton.onClick.AddListener(() => RotateButtonPressed());
        backFromRotateButton.onClick.AddListener(() => BackFromRotateButtonPressed());
        rotateLeftButton.onClick.AddListener(() => RotateLeftButtonPressed());
        rotateRightButton.onClick.AddListener(() => RotateRightButtonPressed());
        backFromRotateButton.onClick.AddListener(() => BackFromRotateButtonPressed());
        confirmRotateButton.onClick.AddListener(() => ConfirmRotateButtonPressed());
        resetRotateButton.onClick.AddListener(() => ResetTofirstRotationButtonPressed());

        textExplanation.SetActive(true);

        enemyShipHealth = gameManager.enemyShipHealth;
        playerShipHealth = gameManager.playerShipHealth;


        RandomizedFirstTurn();
        SetGamePhaseTiles();

        DisablePrepTile();
    }

    public void EnableGameTile()
    {
        foreach (BoxCollider2D col in gamePhaseTiles.GetComponentsInChildren<BoxCollider2D>() )
        {
            
            col.enabled = true;
        }
    }

    public void DisableGameTile()
    {
        foreach (BoxCollider2D col in gamePhaseTiles.GetComponentsInChildren<BoxCollider2D>())
        {
            
            col.enabled = false;
        }
    }

    public void DisablePrepTile()
    {
        foreach (BoxCollider2D col in prepPhaseTiles.GetComponentsInChildren<BoxCollider2D>())
        {
            if (col.gameObject.GetComponent<ShipScript>()) continue;
            col.enabled = !col.enabled;
        }
    }


    IEnumerator PopAText(string text)
    {
        popUptext.SetActive(true);
        text_popup.text = text;
        yield return new WaitForSeconds(popUpTextTime);
        popUptext.SetActive(false);

    }


    void SetPlayerTurnButton(bool temp)
    {
        shipAbilitiesButton.gameObject.SetActive(temp);
        skipButton.gameObject.SetActive(temp);
    }

    void ShipAbilitesButtonPressed()
    {
        if(currentClickedShip == null)
        {
            StartCoroutine(PopAText("Please Choose A Ship"));
            isAShipSelected = false;
            return;
        }
        else
        {
            isAShipSelected = true;
        }

        SetPlayerTurnButton(false);

        TurnOnAllAbilites();
    }

    void TurnOnAllAbilites()
    {
        moveButton.gameObject.SetActive(true);
        shootButton.gameObject.SetActive(true);
        rotateButton.gameObject.SetActive(true);

        backFromAbilitiesButton.gameObject.SetActive(true);
    }

    void TurnOffAllAbilites()
    {
        moveButton.gameObject.SetActive(false);
        shootButton.gameObject.SetActive(false);
        rotateButton.gameObject.SetActive(false);

        backFromAbilitiesButton.gameObject.SetActive(false);
    }

    void BackFromAbilitiesPressed()
    {
        SetPlayerTurnButton(true);

        TurnOffAllAbilites();
        ResetColor();

        isAShipSelected = false;
        prepTileNumber.ResetAllColor();
    }

    void MoveButtonPressed()
    {
        TurnOffAllAbilites();

        forwardButton.gameObject.SetActive(true);
        diagLeftButton.gameObject.SetActive(true);
        diagRightButton.gameObject.SetActive(true);

        TurnOnOrOffMoveAbilites(true);
        DisplayShipMoveTile();



        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();
        int currentHeadPos = ship.GetShipHeadPos();
        int tempX = currentHeadPos % 30;
        int tempY = (Mathf.CeilToInt(currentHeadPos / 30));

        Debug.Log(currentHeadPos + " " + tempX + " " + tempY);

        //Debug.Log(currentClickedShip);

        firstPlace = currentClickedShip.GetComponent<ShipScript>().GetClickedTile();
        currentPlace = firstPlace;      
    }

    
    void ShootButtonPressed()
    {
        currentTile = null;

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();
        DisplayAttackArea(ship);

        TurnOffAllAbilites();
        EnableGameTile();

        TurnOnOrOffShootAbilities(true);
    }
   

    void RotateButtonPressed()
    {
        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();


        TurnOffAllAbilites();

        TurnOnOrOffRotateAbilities(true);
        CheckShipForNextRotation();

        ship.SaveCurrentRotation(ship.currentRotation);

        firstPlace = currentClickedShip.GetComponent<ShipScript>().GetClickedTile();
        currentPlace = firstPlace;
    }

    void BackFromRotateButtonPressed()
    {
        TurnOnAllAbilites();
        TurnOnOrOffRotateAbilities(false);

        currentClickedShip.GetComponent<ShipScript>().ResetToFirstRotation();
    }

    void RotateLeftButtonPressed()
    {
        currentClickedShip.GetComponent<ShipScript>().RotateLeftCurrentShip();
        rotateLeftButton.gameObject.SetActive(false);
        rotateRightButton.gameObject.SetActive(false);
    }

    void ResetTofirstRotationButtonPressed()
    {
        currentClickedShip.GetComponent<ShipScript>().ResetToFirstRotation();

        rotateLeftButton.gameObject.SetActive(true);
        rotateRightButton.gameObject.SetActive(true);

        CheckShipForNextRotation();
    }

    void RotateRightButtonPressed()
    {
        currentClickedShip.GetComponent<ShipScript>().RotateRightCurrentShip();
        rotateRightButton.gameObject.SetActive(false);
        rotateLeftButton.gameObject.SetActive(false);
    }


    void ConfirmRotateButtonPressed()
    {
        ConfirmCurrentShipPosition();

        TurnOnOrOffRotateAbilities(false);
    }

    void CheckShipForNextRotation()
    {
        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();
        GameObject shipTile = ship.GetClickedTile();

        //Debug.Log(shipTile);

            if (CheckRotateLeftShipCollide(shipTile))
            {
                rotateLeftButton.gameObject.SetActive(true);
            }
            else
            {
                rotateLeftButton.gameObject.SetActive(false);
            }

            if (CheckRotateRightShipCollide(shipTile))
            {
                rotateRightButton.gameObject.SetActive(true);
            }
            else
            {
                rotateRightButton.gameObject.SetActive(false);
            }

    }

    private bool CheckRotateLeftShipCollide(GameObject tile)
    {
        ShipScript currentShip = currentClickedShip.GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;

        bool checkAvailableforSpawn = false;
        int tempX = tilePos % 30;
        int tempY = Mathf.CeilToInt(tilePos / 30);
        int currPosY = tilePos;

        int tempLeft = (int)currentShip.currentRotation + 90;

        if (tempLeft == 0 || tempLeft == 360) //Check Kebawah
        {
            bool verticalCheckBawah = tilePos - (30 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;

            for (int i = 0; i < currentShip.shipSize -1; i++)
            {
                tempY--;
                //Check ada Ship? 
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY -= 30;

                }
                
            }
        }
        else if (tempLeft == 90 || tempLeft == -270) //Check kanan
        {
            bool horizontalCheckKanan = 29 - (tilePos % 30) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;

            for (int i = 1; i <= currentShip.shipSize - 1; i++)
            {
                tempX++;
                //check ada ship ?
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                
            }
        }
        else if (tempLeft == -90 || tempLeft == 270) //Check kiri
        {
            bool horizontalCheckKiri = (tilePos % 30) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;

            for (int i = 1; i <= currentShip.shipSize - 1; i++)
            {
                tempX--;
                //check ada ship ?
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                
            }
        }
        else if (tempLeft == 180 || tempLeft == -180) // check Atas
        {
            bool verticalCheckAtas = tilePos + (30 * (currentShip.shipSize - 1)) <= 899;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;

            for (int i = 0; i < currentShip.shipSize - 1; i++)
            {
                tempY++;
                //Check ada Ship? 
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY += 30;

                }
                
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
        ShipScript currentShip = currentClickedShip.GetComponent<ShipScript>();
        int tilePos = tile.GetComponent<TileScript>().tileNumber;

        bool checkAvailableforSpawn = false;
        int tempX = tilePos % 30;
        int tempY = Mathf.CeilToInt(tilePos / 30);
        int currPosY = tilePos;

        int tempRight = (int)currentShip.currentRotation - 90;

        if (tempRight == 0 || tempRight == -360) //Check Kebawah
        {
            bool verticalCheckBawah = tilePos - (30 * (currentShip.shipSize - 1)) >= 0;
            //Debug.Log("BawahCheck= " + verticalCheckBawah);
            if (!verticalCheckBawah) return false;

            for (int i = 0; i < currentShip.shipSize - 1; i++)
            {
                tempY--;
                //Check ada Ship? 
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY -= 30;

                }
                
            }
        }
        else if (tempRight == 90 || tempRight == -270) //Check kanan
        {
            bool horizontalCheckKanan = 29 - (tilePos % 30) >= currentShip.shipSize - 1;
            //Debug.Log("KiriCheck= " + horizontalCheckKiri);
            if (!horizontalCheckKanan) return false;

            for (int i = 1; i <= currentShip.shipSize - 1; i++)
            {
                tempX++;
                //check ada ship ?
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                
            }
        }
        else if (tempRight == -90 || tempRight == 270) //Check kiri
        {
            bool horizontalCheckKiri = (tilePos % 30) >= currentShip.shipSize - 1;
            //Debug.Log("KananCheck= " + horizontalCheckKanan);
            if (!horizontalCheckKiri) return false;

            for (int i = 1; i <= currentShip.shipSize - 1; i++)
            {
                tempX--;
                //check ada ship ?
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;

                }
                
            }
        }
        else if (tempRight == 180 || tempRight == -180) // check Atas
        {
            bool verticalCheckAtas = tilePos + (30 * (currentShip.shipSize - 1)) <= 899;
            //Debug.Log("AtasCheck= " + verticalCheckAtas);
            if (!verticalCheckAtas) return false;

            for (int i = 0; i < currentShip.shipSize - 1; i++)
            {
                tempY++;
                //Check ada Ship? 
                if (currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                //Check Overflow Y-axis ?
                if (currPosY < 0 && currentPlayerPos[tempX, tempY] > 0)
                {
                    checkAvailableforSpawn = false;
                    break;
                }
                else
                {
                    //Debug.Log("Checkin:" + tempX + tempY);
                    checkAvailableforSpawn = true;
                    currPosY += 30;

                }
                
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


    void DisplayAttackArea(ShipScript ship)
    {
        //get attack range
        DisablePrepTile();

        ship.SearchForAttackArea();
        DisablePrepTile();

        allArea = ship.GetAttackArea();
        //Display attack Range
        int idxCounter = 0;

        for(int i = 0; i < allArea.Length; i++)
        {
            TileScript attTile = allArea[idxCounter].collider.gameObject.GetComponent<TileScript>();
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.isTileGrey) continue;
                if (tile.isTileRed) continue; 
                if (tile == attTile)
                {
                    tile.SetToGreenColor();                  
                    break;
                }
            }

            foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == attTile.tileNumber)
                {
                    if (tile.isTileRed) break;
                    if (tile.isTileGrey) break;
                    tile.isTileinAttackRange = true;
                    tile.SetToGreenColor();
                    break;
                }
            }
            idxCounter++;
        }
        
    }


    void BackFromShootAbilitiesPressed()
    {
        TurnOnOrOffShootAbilities(false);
        DisableGameTile();
        ResetColor();

        currentTile = null;

        TurnOnAllAbilites();

        prepTileNumber.ResetAllColor();
        gameTileNumber.ResetAllColor();

        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.isTileinAttackRange = false;
        }
    }

    void TurnOnOrOffShootAbilities(bool temp)
    {
        shootAbilities.SetActive(temp);
    }


    void TurnOnOrOffRotateAbilities(bool temp)
    {
        rotateAbilities.SetActive(temp);
    }

    void TurnOnOrOffDiagButton(bool temp)
    {
        diagLeftButton.gameObject.SetActive(temp);
        diagRightButton.gameObject.SetActive(temp);
    }


    void MoveForwardPressed()
    {
        TurnOnOrOffDiagButton(false);

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();

        int shipRotation = ship.currentRotation;
        int maxLengthMovement = ship.GetMoveLength();
        int tileNumber = currentPlace.GetComponent<TileScript>().tileNumber;

        int firstHeadPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;
        

        TileScript tileScript = null;

        if (shipRotation == 0 || shipRotation == 360)//bawa
        {
            int posY = Mathf.CeilToInt(tileNumber / 30);
            int posX = tileNumber % 30;

            int maxTileNumber = ((maxLengthMovement - 1) * 30) + firstHeadPos;

            if (posY + 1 > 29) return;
            if (tileNumber >= maxTileNumber || currentPlayerPos[posX, ++posY] > 0 ) return;
            else
            {
                //search for tile
                tileNumber += 30;
                foreach(TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if(tile.tileNumber == tileNumber)
                    {
                        tileScript = tile;
                        break;
                    }
                }
                //move the ship
                ship.transform.SetParent(tileScript.gameObject.transform);
                ship.SetPosition();
                currentPlace = tileScript.gameObject;
            }

        }
        else if (shipRotation == 90 || shipRotation == -270)//kanan
        {
            int posX = tileNumber % 30;
            int posY = Mathf.CeilToInt(tileNumber / 30);

            int maxTileNumber = firstHeadPos - maxLengthMovement + 1;
            //Debug.Log(tileNumber + ", " + maxTileNumber);
            if (posX - 1 < 0) return;
            if (tileNumber <= maxTileNumber || currentPlayerPos[--posX, posY] > 0) return;
            else
            {
                //search for tile
                tileNumber --;
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == tileNumber)
                    {
                        tileScript = tile;
                        break;
                    }
                }
                //move the ship
                ship.transform.SetParent(tileScript.gameObject.transform);
                ship.SetPosition();
                currentPlace = tileScript.gameObject;
            }
        }
        else if (shipRotation == 180 || shipRotation == -180)//atas
        {
            int posX = tileNumber % 30;
            int posY = Mathf.CeilToInt(tileNumber / 30);
            

            int maxTileNumber = firstHeadPos - ((maxLengthMovement - 1) * 30);
            //Debug.Log(tileNumber + ", " + maxTileNumber);
            if (posY - 1 < 0) return;
            if (tileNumber <= maxTileNumber || currentPlayerPos[posX, --posY] > 0 ) return;
            else
            {
                //search for tile
                tileNumber -= 30;
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == tileNumber)
                    {
                        tileScript = tile;
                        break;
                    }
                }
                //move the ship
                ship.transform.SetParent(tileScript.gameObject.transform);
                ship.SetPosition();
                currentPlace = tileScript.gameObject;
            }
        }
        else if (shipRotation == 270 || shipRotation == -90)//kiri
        {
            int posX = tileNumber % 30;
            int posY = Mathf.CeilToInt(tileNumber / 30);


            int maxTileNumber = firstHeadPos + maxLengthMovement - 1;
            //Debug.Log(tileNumber + ", " + maxTileNumber);
            if (posX + 1 > 29) return;
            if (tileNumber >= maxTileNumber || currentPlayerPos[++posX, posY] > 0  ) return;
            else
            {
                //search for tile
                tileNumber++;
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == tileNumber)
                    {
                        tileScript = tile;
                        break;
                    }
                }
                //move the ship
                ship.transform.SetParent(tileScript.gameObject.transform);
                ship.SetPosition();
                currentPlace = tileScript.gameObject;
            }
        }
    }
    

    void MoveDiagRightPressed()
    { 
        if (!canShipDiagRight) return;
        TurnOnOrOffMoveForwardButton(false);

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();

        int shipRotation = ship.currentRotation;
        int tempX, tempY;


        if (shipRotation == 0 || shipRotation == 360)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKanan = (tempX + 1) + (tempY * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if (shipRotation == 90 || shipRotation == -270)//kanan
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKanan = (tempX - 1) + (tempY * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if (shipRotation == 180 || shipRotation == -180)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKanan = (tempX - 1) + ((tempY - 2) * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if (shipRotation == 270 || shipRotation == -90)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKanan = (tempX + 1) + ((tempY - 2) * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
    }


    void MoveDiagLeftPressed()
    {
        if (!canShipDiagleft) return;
        TurnOnOrOffMoveForwardButton(false);

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();

        int shipRotation = ship.currentRotation;
        int tempX, tempY;

        if (shipRotation == 0 || shipRotation == 360)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKiri = (tempX - 1) + (tempY * 30);

            foreach(TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if(tile.tileNumber == tempPosDiagKiri)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if(shipRotation == 90 || shipRotation == -270)//kanan
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKiri = (tempX - 1) + ((tempY - 2) * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKiri)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if(shipRotation == 180 ||shipRotation == -180)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKiri = (tempX + 1) + ((tempY - 2) * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKiri)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }
        else if(shipRotation == 270 || shipRotation == -90)
        {
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            int tempPosDiagKiri = (tempX + 1) + ((tempY) * 30);

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKiri)
                {
                    currentPlace = tile.gameObject;
                }
            }

            ship.transform.SetParent(currentPlace.transform);
            ship.SetPosition();
        }


    }



    void ResetCurrentShip()
    {
        TurnOnOrOffDiagButton(true);
        TurnOnOrOffMoveForwardButton(true);

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();

        ship.transform.SetParent(ship.GetClickedTile().transform);
        ship.SetPosition();
        currentPlace = ship.GetClickedTile();
    }
    
    void ConfirmCurrentShipPosition()
    {
        ShipScript currentShip = currentClickedShip.GetComponent<ShipScript>();
        int currentID = currentShip.shipID;
        int currentRt = currentShip.currentRotation;
        //update ship position
        //delete all id dlu
        int[] tempShipArray = new int[5];
        int counter;
        if (currentRt == 0 || currentRt == 360 || currentRt == -90 || currentRt == 270)
            counter = currentShip.shipSize - 1;
        else counter = 0;
        for (int i = 0; i < 30; i++)
        {
            for(int j = 0; j < 30; j++)
            {
                if (currentID == currentPlayerPos[i, j] || -currentID == currentPlayerPos[i, j])
                {
                    tempShipArray[counter] = currentPlayerPos[i, j];
                    currentPlayerPos[i, j] = 0;
                    if (currentRt == 0 || currentRt == 360 || currentRt == -90 || currentRt == 270)
                        counter--;
                    else 
                        counter++;
                }
            }
        }

        //foreach(int temp in tempShipArray)
        //{
        //    Debug.Log(temp);
        //}

        //find all id dan move mereka
        PrintShipToArray(currentShip, currentPlace, tempShipArray);

        currentShip.SetClickedTile(currentPlace);
        //move yg kena demeg
        foreach(TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tile.shipTileId == currentClickedShip.GetComponent<ShipScript>().shipID)
            {
                tile.ResetColor();
            }
        }

        int shipHeadPos = currentShip.GetShipHeadPos();
        TileScript tempTile = null;

        foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tile.tileNumber == shipHeadPos)
                tempTile = tile;
        }

        int tempTileNum = tempTile.tileNumber;

        counter = 0;

        prepTileNumber.ResetForDamageAllColor();


        //move yg kena titik point DARI HEAD
        if (currentShip.currentRotation == 0 || currentShip.currentRotation == 360)
        {
            for (int i = 0; i < currentShip.shipSize; i++)
            {
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if(tile.tileNumber == tempTileNum && tempShipArray[counter] == -currentID)
                    {
                        tile.SetTilePlayerDamaged();
                        break;
                    }
                }
                counter++;
                tempTileNum -= 30;
            }

        }
        else if(currentShip.currentRotation == -90 || currentShip.currentRotation == 270)//kiri
        {
            for (int i = 0; i < currentShip.shipSize; i++)
            {
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == tempTileNum && tempShipArray[counter] == -currentID)
                    {
                        tile.SetTilePlayerDamaged();
                        break;
                    }
                }
                counter++;
                tempTileNum--;
            }

        }
        else if(currentShip.currentRotation == 180 || currentShip.currentRotation == -180)
        {
            for (int i = 0; i < currentShip.shipSize; i++)
            {
                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == tempTileNum && tempShipArray[counter] == -currentID)
                    {
                        tile.SetTilePlayerDamaged();
                        break;
                    }
                }
                counter++;
                tempTileNum += 30;
            }
        }
        else if(currentShip.currentRotation == 90 || currentShip.currentRotation == -270)//kanan
        {
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (counter > currentShip.shipSize - 1) return;
                if (tile.tileNumber == tempTileNum && tempShipArray[counter] == -currentID)
                {
                    tile.SetTilePlayerDamaged();
                    tempTileNum++;
                    counter++;
                }
                if (counter > currentShip.shipSize) break;
            }
        }


        TurnOffAllAbilites();
        TurnOnOrOffMoveAbilites(false);


        



        OpponentsTurn();
        forwardButton.gameObject.SetActive(true);
    }

    void PrintShipToArray(ShipScript shipScript, GameObject tile, int[] tempShipArray)
    {
        int currPos = tile.GetComponent<TileScript>().tileNumber;
        shipScript.SetHeadShipPos(currPos);
        int tempY = Mathf.CeilToInt(currPos / 30);
        int tempX = currPos % 30;

        //Debug.Log("Printship: " + currPos);
        //Debug.Log("X pos: " + tempX + ", Y pos: " + tempY);
        int counter = 0;

        if (shipScript.currentRotation == 0) //Save Kebawah
        {
            //SaveVerticalToArray();

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerPos[tempX, tempY] = tempShipArray[counter++];
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY--;
            }
        }
        else if (shipScript.currentRotation == -90 || shipScript.currentRotation == 270) // Save kekiri
        {
            //saveHorizontalToArray();

            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerPos[tempX, tempY] = tempShipArray[counter++];
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX--;
            }
        }
        else if (shipScript.currentRotation == 90 || shipScript.currentRotation == -270) //Save Kekanan
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerPos[tempX, tempY] = tempShipArray[counter++];
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempX++;
            }
        }
        else if (shipScript.currentRotation == 180 || shipScript.currentRotation == -180) //Save Keatas
        {
            for (int i = 1; i <= shipScript.shipSize; i++)
            {
                currentPlayerPos[tempX, tempY] = tempShipArray[counter++];
                //Debug.Log("Saving: " + tempX + " " + tempY);
                tempY++;
            }
        }
    }



    void DisplayShipMoveTile()
    {
        prepTileNumber.ResetAllColor();

        ShipScript ship = currentClickedShip.GetComponent<ShipScript>();
        int maxShipMove = ship.GetMoveLength();
        int shipRotation = ship.currentRotation;

        int currentHeadPos = ship.GetShipHeadPos();
        int tempX = currentHeadPos % 30;
        int tempY = (Mathf.CeilToInt(currentHeadPos / 30) + 1);

        canShipDiagleft = false;
        canShipDiagRight = false;

        //Debug.Log(currentHeadPos);

        if (shipRotation == 0 || shipRotation == 360)//bawa
        {
            //check depan
            if (tempY > 30) return;

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {

                if (tile.tileNumber == currentHeadPos)
                {
                    tile.isMoveAble = true;
                    //if(!tile.isTileGrey)
                        tile.SetToGreenColor();
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    currentHeadPos += 30;
                    maxShipMove--;

                    if (maxShipMove <= 0) break;
                    if (tempY++ > 30) break;
                }
            }
            //check DIAG
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            //check Diag Kiri 
            int tempPosDiagKiri = (tempX - 1) + (tempY * 30);
            foreach(TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if(tile.tileNumber == tempPosDiagKiri)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for(int i = 0; i < ship.shipSize; i++)
                    {
                        int tileNumber = tile.tileNumber;

                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posY--;
                    }

                    if(check)
                    {
                        //if(!tile.isTileGrey) 
                              tile.SetToGreenColor();
                        canShipDiagleft = true;
                        break;
                    } 
                    else
                    {
                        canShipDiagleft = false;
                        break;
                    }

                }
            }

            //check Diag kanan
            int tempPosDiagKanan = (tempX + 1) + (tempY * 30);
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    bool check = true;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posY--;
                    }

                    if (check)
                    {
                        //if(!tile.isTileGrey)
                            tile.SetToGreenColor();

                        canShipDiagRight = true;
                        break;
                    }
                    else
                    {
                        canShipDiagRight = false;
                        break;
                    }

                }
            }

        }
        else if (shipRotation == 90 || shipRotation == -270) //kanan
        {
            bool checkShipPos = false;
            for (int i = 0; i < 5; i++)
            {
                if (maxShipMove <= 0) break;
                if (tempX < -1) break;

                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    
                    if (tile.tileNumber == currentHeadPos)
                    {
                        tempX--;
                        tile.isMoveAble = true;
                        //if(!tile.isTileGrey)
                            tile.SetToGreenColor();
                        if (tempX < 0) break;
                        if (currentPlayerPos[tempX, tempY] > 0)
                        {
                            checkShipPos = true;
                            break;
                        }
                        break;
                    }
                }
                if (tempX < 0) break;

                if (checkShipPos) break;

                currentHeadPos--;
                maxShipMove--;
            }

            //CHECK DIAG
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);
            int tempPosDiagKiri = (tempX-1) + ((tempY-2) * 30);
            tempX -= 1;
            tempY -= 2;

            //CheckForDiagLeft
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if(tile.tileNumber == tempPosDiagKiri)
                {
                    if (tempX < 0) break;
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posX++;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagleft = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            //check diagKanan
            int tempPosDiagKanan = (tempX-1) + ((tempY) * 30);
            tempX -= 1;


            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    if (tempX < 0) break;
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posX++;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagRight = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }


        }
        else if (shipRotation == 180 || shipRotation == -180)//atas
        {
            //if (tempY < 0) return;
            bool checkShipPos = false;
            tempY--;
            for (int i = 0; i < 5; i++)
            {
                //Debug.Log(tempY);
                if (tempY < -1) break;
                if (maxShipMove <= 0) break;
                

                foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
                {
                    if (tile.tileNumber == currentHeadPos)
                    {
                        tempY--;
                        
                        tile.isMoveAble = true;
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        if (tempY < 0) break;

                        if (currentPlayerPos[tempX, tempY] > 0)
                        {
                            checkShipPos = true;
                            break;
                        }
                            
                        //Debug.Log(tile.tileNumber);
                        break;
                    }
                }
                
                if (checkShipPos) break;

                currentHeadPos -= 30;
                maxShipMove--;                
            }

            //CHECK DIAG
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            //check diagKiri
            int tempPosDiagKiri = (tempX + 1) + ((tempY-2) * 30);

            tempX += 1;
            tempY -= 2;


            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKiri)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posY++;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagleft = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);
            int tempPosDiagKanan = (tempX - 1) + ((tempY - 2) * 30);

            tempX -= 1;
            tempY -= 2;

            //CheckForDiagRight
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posY++;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagRight = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

         
        }
        else if (shipRotation == 270 || shipRotation == -90)//kiri
        {
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tempX > 30) break;
                if (tile.tileNumber == currentHeadPos)
                {
                    if (++tempX > 30) break;

                    tile.isMoveAble = true;
                    //if (!tile.isTileGrey)
                        tile.SetToGreenColor();

                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    currentHeadPos++;
                    maxShipMove--;

                    if (maxShipMove <= 0) break;
                    if (+tempX > 30) break;
                }
            }

            //CHECK DIAG
            int tempPos = ship.GetClickedTile().GetComponent<TileScript>().tileNumber;

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);
            if (tempX > 29) return;

            //check diagkanan
            int tempPosDiagKanan= (tempX + 1) + ((tempY - 2) * 30);

            tempX += 1;
            tempY -= 2;


            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKanan)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posX--;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagRight = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

            }

            tempX = tempPos % 30;
            tempY = (Mathf.CeilToInt(tempPos / 30) + 1);

            //check diagKiri
            int tempPosDiagKiri= (tempX + 1) + ((tempY) * 30);

            tempX += 1;

            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if (tile.tileNumber == tempPosDiagKiri)
                {
                    if (currentPlayerPos[tempX, tempY] > 0) break;
                    int posX = tile.tileNumber % 30;
                    int posY = Mathf.CeilToInt(tile.tileNumber / 30);
                    bool check = true;

                    for (int i = 0; i < ship.shipSize; i++)
                    {
                        if (currentPlayerPos[posX, posY] > 0)
                        {
                            check = false;
                            break;
                        }
                        posX--;
                    }

                    if (check)
                    {
                        //if (!tile.isTileGrey)
                            tile.SetToGreenColor();
                        canShipDiagleft = true;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

            }

        }
    }



    void TurnOnOrOffMoveAbilites(bool temp)
    {
        moveAbilities.SetActive(temp);
    }

    void TurnOnOrOffMoveForwardButton(bool temp)
    {
        forwardButton.gameObject.SetActive(temp);

    }

    void BackfromMoveButtonPressed()
    {
        moveAbilities.SetActive(false);

        TurnOnOrOffMoveAbilites(false);
        TurnOnAllAbilites();

        ResetCurrentShip();
        prepTileNumber.ResetAllColor();
    }


    void OpponentsTurn()
    {
        textExplanation.SetActive(false);
        SetPlayerTurnButton(false);
        enemiesTurnUI.SetActive(true);

        gridText.text = "";

        topText.text = "It's Enemies Turn!";
        isPlayerTurn = false;
        DisableGameTile();

        EnemyLaunchMissileAtPlayer();

        StartEnemyTimer();
    }

    void StartEnemyTimer()
    {
        currentTime = enemiesTurnTime;
        timeSlider.value = enemiesTurnTime;

        isStartTimer = true;
    }

    private void Update()
    {
        CheckForCoundownEnemyTimer();
        SelectCurrentShip();
       
        if(Input.GetKey(KeyCode.Space))
        {
            OpponentsTurn();
        }
    }

    void SelectCurrentShip()
    {
        if (Input.GetMouseButton(0) && isPlayerTurn && !isAShipSelected)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos, Vector3.forward, float.MaxValue, mask);
            if (!hit) return;
            if (hit.collider.gameObject.GetComponent<ShipScript>())
            {
                currentClickedShip = hit.collider.gameObject;
                shipType.SetActive(true);
                shipTypeText.text = currentClickedShip.gameObject.name;
            }

           


        }
    }

    void CheckForCoundownEnemyTimer()
    {
        if (isStartTimer)
        {
            timeSlider.value = currentTime;
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                isStartTimer = false;
                enemiesTurnUI.SetActive(false);
                PlayersTurn();
            }
        }
    }


    void PlayersTurn()
    {
        
        currentClickedShip = null;
        textExplanation.SetActive(true);
        topText.text = "It's Your Turn!";
        isAShipSelected = false;
        isPlayerTurn = true;

        shipTypeText.text = " ";

        DisableGameTile();

        SetPlayerTurnButton(true);

        CheckForWinCondition();
    }

    private void CheckForWinCondition()
    {
        if(totalEnemySpot == 0)
        {
            DisableGameTile();
            SetWinText();
            endButton.gameObject.SetActive(true);
        }
    }

    private void CheckForLoseCondition()
    {
        if(totalPlayerSpot == 0)
        {
            DisableGameTile();
            SetLoseText();
            endButton.gameObject.SetActive(true);
        }

    }

    void SetWinText()
    {
        EndText.text = "You Win!!";
        EndText.color = Color.green;
        pressAnywhereText.color = Color.green;
    }

    void SetLoseText()
    {
        EndText.text = "You Lose!!";
        EndText.color = Color.red;
        pressAnywhereText.color = Color.red;
    }

    void PlayerLaunchMissileAtTile()
    {
        ResetColor();
        prepTileNumber.ResetAllColor();

        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.isTileinAttackRange = false;
        }

        //Temporary   
        if (currentTile == null)
        {
            StartCoroutine(PopAText("Please choose A grid"));
            return;
        }

        TurnOnOrOffShootAbilities(false);

        TileScript tileScript = currentTile.GetComponent<TileScript>();
        tileScript.ThisTileUsed();

        if (playerMarkedSpot[tileX, tileY] == 1)
        {
            return;
        }

        playerMarkedSpot[tileX, tileY] = 1;

        if (currentEnemyPos[tileX, tileY] == 0)
        {
            //Missile Missed
            tileScript.SetTilePlayerMissed();
            
        }
        else
        {
            //Missile Hit
            int id = currentEnemyPos[tileX, tileY];
            tileScript.SetTilePlayerHit(id);

            CheckAndUpdateEnemyLives(tileScript);

            totalEnemySpot--;

        }

        
        CheckForWinCondition();

        if (totalEnemySpot <= 0) return;

        OpponentsTurn();

        Debug.Log("Total Enemy Spot: " + totalEnemySpot);
        currentTile = null;

    }

    void EnemyLaunchMissileAtPlayer()
    {
        TileScript tileScript = prepPhaseTiles.GetComponentInChildren<TileScript>();
        int randX, randY;
        int tileNum;

        //make sure to not rand a same place twice
        do
        {
            randX = Random.Range(0, 30);
            randY = Random.Range(0, 30);

            if (enemyMarkedSpot[randX, randY] == 0)
            {
                enemyMarkedSpot[randX, randY] = 1;
                break;
            }

        } while (true);

        tileNum = (randY * 30) + randX;
        Debug.Log(randX + " " + randY + "== " + tileNum);

        enemiesCurrentGridText.text = "[" + randX + ", " + (randY + 0) + "]";

        //Search for desired tile
        foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if(tile.tileNumber == tileNum)
            {
                tileScript = tile;
                break;
            }
        }

        

        tileScript.ThisTileSelected();
        if (currentPlayerPos[randX, randY] == 0)
        {
            tileScript.SetTilePlayerMissed();
            SetEnemyMissedText();
        }
        else
        {
            int shipId = currentPlayerPos[randX, randY];
            tileScript.SetTilePlayerHit(shipId);
            SetEnemyHitText();

            //Instantiate(playerHitTag, tileScript.transform);

            CheckAndUpdatePlayerLives(randX, randY);

            totalPlayerSpot--;
        }

        CheckForLoseCondition();

        Debug.Log("Total Player Spot: " + totalPlayerSpot);
    }
    
    void SetEnemyMissedText()
    {
        hitOrMissedText.text = "MISSED!!";
        hitOrMissedText.color = Color.green;
    }

    void SetEnemyHitText()
    {
        hitOrMissedText.text = "HIT!!";
        hitOrMissedText.color = Color.red;
    }

    public void TileCliked(GameObject tile)
    {
        

        if(currentTile != null)
            if(currentTile != tile)
            {
                currentTile.GetComponent<TileScript>().thisTileSelected = false;
                ResetColor();
                DisplayAttackArea(currentClickedShip.GetComponent<ShipScript>());
            }

        currentTile = tile;
        TileScript tileScript = tile.GetComponent<TileScript>();

        
        tileScript.SetColorForSelected();
        tileScript.ThisTileSelected();



        int tileNumber = tileScript.tileNumber;
        tileX = tileNumber % 30;
        tileY = Mathf.CeilToInt(tileNumber / 30f);

        gridText.text = "[" + tileX + ", " + (tileY - 0) + "]";   

    }

    void ResetColor()
    {
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tile.isThisTileUsed && tile.isTileinAttackRange) continue;

            if (tile.isTileGrey)
            {
                tile.SetTilePlayerMissed();
            }
            else if(tile.isTileRed)
            {
                tile.SetTilePlayerDamaged();
            }
            else    
                tile.ResetColor();
        }
    }


    void SetGamePhaseTiles()
    {
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.isGamePhaseStarted = true;
        }
    }

    void RandomizedFirstTurn()
    {
        int random = Random.Range(0, 2);
        if (random == 1)
        {
            PlayersTurn();
        }
        else
        {
            OpponentsTurn();
        }
    }






    void CheckAndUpdatePlayerLives(int posX, int posY)
    {
        int currentId = currentPlayerPos[posX, posY];
        currentPlayerPos[posX, posY] = currentId * -1;

        playerShipHealth[currentId, 0] -= 1;

        if (playerShipHealth[currentId, 0] <= 0)
        {
            currentPlayerShipAlive -= 1;
            playerShipAliveText.text = currentPlayerShipAlive.ToString();

            ShipScript[] ships = FindObjectsOfType<ShipScript>();
            foreach (ShipScript ship in ships)
            {
                if (ship.shipID == currentId)
                {
                    Destroy(ship.gameObject);
                }
            }
            foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
            {
                if(tile.shipTileId == currentId)
                {
                    tile.isTileRed = false;
                    tile.ResetColor();
                }
            }
        }
    }

    void CheckAndUpdateEnemyLives(TileScript tile)
    {
        int tempX = tile.tileNumber % 30;
        int tempY = (int) Mathf.Ceil ((float)tile.tileNumber / 30f);

        int currentID = currentEnemyPos[tempX, tempY];
        enemyShipHealth[currentID, 0] -= 1;
        

        if(enemyShipHealth[currentID, 0] <= 0)
        {
            currentEnemyShipAlive -= 1;
            enemyShipAliveText.text = currentEnemyShipAlive.ToString();
        }
    }

    



}
