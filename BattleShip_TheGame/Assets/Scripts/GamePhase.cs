using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GamePhase : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager;
    private int[,] currentPlayerPos;
    private int[,] currentEnemyPos;

    private int[,] currentPlayerShipPos = new int[11, 11];
    private int[,] currentEnemyShipPos = new int[11, 11];

    private int[,] enemyMarkedSpot = new int[11, 11];

    public Button nextButton;
    [SerializeField] GameObject playerHitTag;

    private int totalEnemySpot;
    private int totalPlayerSpot;


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


    private GameObject currentTile = null;

    bool isPlayerTurn = false;
    int tileX, tileY;


    void Start()
    {
        endButton.gameObject.SetActive(false);

        gameManager = GetComponent<GameManager>();
        textExplanation.SetActive(false);

        currentPlayerShipPos = gameManager.currentPlayerShipPos;
        currentEnemyShipPos = gameManager.currentEnemyShipPos;

        totalEnemySpot = gameManager.GetTotalEnemySpot();
        totalPlayerSpot = gameManager.GetTotalPlayerSpot();
    }


    public void StartGame(int[,] playerShipPos, int[,] enemyShipPos)
    {
        currentPlayerPos = playerShipPos;
        currentEnemyPos = enemyShipPos;
        nextButton.onClick.AddListener(() => OpponentsTurn());
        textExplanation.SetActive(true);

        RandomizedFirstTurn();
    }

    void OpponentsTurn()
    {
        topText.text = "It's Enemies Turn!";
        isPlayerTurn = false;
        DisableTiles();



        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => EnemyLaunchMissileAtPlayer());
    }

    void PlayersTurn()
    {
        topText.text = "It's Your Turn!";
        isPlayerTurn = true;
        
        EnableTiles();
        
        nextButton.onClick.RemoveAllListeners();
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
        if (!currentTile)
        {
            OpponentsTurn();
            return;
        }
        
        //Temporary   
        TileScript tileScript = currentTile.GetComponent<TileScript>();
        tileScript.ThisTileUsed();

        if (currentEnemyPos[tileX, tileY] == 0)
        {
            //Missile Missed
            tileScript.SetTilePlayerMissed();
        }
        else
        {
            //Missile Hit
            tileScript.SetTilePlayerHit();
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

        

        //Search for desired tile
        foreach(TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if(tile.tileNumber == tileNum)
            {
                tileScript = tile;
                break;
            }
        }

        if (currentPlayerPos[randX, randY] == 0)
        {
            tileScript.SetTilePlayerMissed();
        }
        else
        {
            tileScript.SetTilePlayerHit();
            Instantiate(playerHitTag, tileScript.transform);
            totalPlayerSpot--;
        }

        CheckForLoseCondition();

        PlayersTurn();


        Debug.Log("Total Player Spot: " + totalPlayerSpot);
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
}
