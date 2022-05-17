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

    GameManager gameManager;
    private int[,] currentPlayerPos;
    private int[,] currentEnemyPos;

    private int[,] enemyMarkedSpot = new int[11, 11];
    private int[,] playerMarkedSpot = new int[11, 11];

    public Button nextButton;
    [SerializeField] GameObject playerHitTag;

    private int totalEnemySpot;
    private int totalPlayerSpot;

    RaycastHit2D hit;


    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gridText;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] GameObject textExplanation;
    [SerializeField] TextMeshProUGUI pressAnywhereText;

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
    float currentTime;
    bool isStartTimer = false;

    [Header("PlayerAndEnemyText")]
    [SerializeField] GameObject playerText;
    [SerializeField] TextMeshProUGUI playerShipAliveText;
    [SerializeField] GameObject enemyText;
    [SerializeField] TextMeshProUGUI enemyShipAliveText;
    int currentPlayerShipAlive = 5;
    int currentEnemyShipAlive = 5;


    private GameObject currentTile = null;

    bool isPlayerTurn = false;
    int tileX, tileY;


    void Start()
    {
        endButton.gameObject.SetActive(false);

        gameManager = GetComponent<GameManager>();
        textExplanation.SetActive(false);


        totalEnemySpot = gameManager.GetTotalEnemySpot();
        totalPlayerSpot = gameManager.GetTotalPlayerSpot();

        timeSlider.maxValue = enemiesTurnTime;

        enemiesTurnUI.SetActive(false);

        playerText.SetActive(false);
        enemyText.SetActive(false);
    }


    public void StartGame(int[,] playerShipPos, int[,] enemyShipPos)
    {
        playerText.SetActive(true);
        enemyText.SetActive(true);
        
        currentPlayerPos = playerShipPos;
        currentEnemyPos = enemyShipPos;

        nextButton.onClick.AddListener(() => PlayerLaunchMissileAtTile());
        textExplanation.SetActive(true);

        enemyShipHealth = gameManager.enemyShipHealth;
        playerShipHealth = gameManager.playerShipHealth;


        RandomizedFirstTurn();
        SetGamePhaseTiles();
    }

    void OpponentsTurn()
    {
        textExplanation.SetActive(false);
        nextButton.gameObject.SetActive(false);
        enemiesTurnUI.SetActive(true);

        gridText.text = "";

        topText.text = "It's Enemies Turn!";
        isPlayerTurn = false;
        DisableTiles();

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
        textExplanation.SetActive(true);
        topText.text = "It's Your Turn!";
        isPlayerTurn = true;
        
        EnableTiles();

        nextButton.gameObject.SetActive(true);
        nextButton.onClick.AddListener(() => PlayerLaunchMissileAtTile());

        CheckForWinCondition();
    }

    private void CheckForWinCondition()
    {
        if(totalEnemySpot == 0)
        {
            DisableTiles();
            SetWinText();
            endButton.gameObject.SetActive(true);

        }
    }

    private void CheckForLoseCondition()
    {
        if(totalPlayerSpot == 0)
        {
            DisableTiles();
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


        //Temporary   
        if (currentTile == null) return;
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
            tileScript.SetTilePlayerHit();

            CheckAndUpdateEnemyLives(tileScript);

            totalEnemySpot--;

        }

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
            randX = Random.Range(0, 10);
            randY = Random.Range(0, 10);

            if (enemyMarkedSpot[randX, randY] == 0)
            {
                enemyMarkedSpot[randX, randY] = 1;
                break;
            }

        } while (true);

        tileNum = (randY * 10) + randX;
        Debug.Log(randX + " " + randY + "== " + tileNum);

        string stringX = ((char)(randX + 65)).ToString();
        enemiesCurrentGridText.text = "[" + stringX + ", " + (randY + 1) + "]";

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
            tileScript.SetTilePlayerHit();
            SetEnemyHitText();
            Instantiate(playerHitTag, tileScript.transform);

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
        ResetColor();

        currentTile = tile;

        TileScript tileScript = tile.GetComponent<TileScript>();

        tileScript.SetColorForSelected();
        tileScript.ThisTileSelected();

        int tileNumber = tileScript.tileNumber;
        tileX = tileNumber % 10;
        tileY = tileNumber / 10;

        string stringX = ((char)(tileX + 65)).ToString();

        gridText.text = "[" + stringX + ", " + (tileY + 1) + "]";   

    }

    void ResetColor()
    {
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tile.isThisTileUsed) continue;
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


    void DisableTiles()
    {
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.DisableInteractive();
        }
    }

    void EnableTiles()
    {
        foreach (TileScript tile in gamePhaseTiles.GetComponentsInChildren<TileScript>())
        {
            tile.EnableInteractive();
        }
    }


    void CheckAndUpdatePlayerLives(int posX, int posY)
    {
        int currentId = currentPlayerPos[posX, posY];
        playerShipHealth[currentId, 0] -= 1;

        if (playerShipHealth[currentId, 0] <= 0)
        {
            currentPlayerShipAlive -= 1;
            playerShipAliveText.text = currentPlayerShipAlive.ToString();
        }
    }

    void CheckAndUpdateEnemyLives(TileScript tile)
    {
        int tempX = tile.tileNumber % 10;
        int tempY = tile.tileNumber / 10;

        int currentID = currentEnemyPos[tempX, tempY];
        enemyShipHealth[currentID, 0] -= 1;
        

        if(enemyShipHealth[currentID, 0] <= 0)
        {
            currentEnemyShipAlive -= 1;
            enemyShipAliveText.text = currentEnemyShipAlive.ToString();
        }
    }


}
