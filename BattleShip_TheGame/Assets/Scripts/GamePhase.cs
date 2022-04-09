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

    public Button nextButton;
    [SerializeField] GameObject playerHitTag;
        
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI gridText;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] GameObject textExplanation;

    [Header("Tiles")]
    [SerializeField] GameObject gamePhaseTiles;
    [SerializeField] GameObject prepPhaseTiles;


    private GameObject currentTile = null;

    bool isPlayerTurn = false;
    int tileX, tileY;
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        textExplanation.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


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
    }

    void PlayerLaunchMissileAtTile()
    {
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
        }

        OpponentsTurn();
    }


    void EnemyLaunchMissileAtPlayer()
    {
        TileScript tileScript = prepPhaseTiles.GetComponentInChildren<TileScript>();
        int tileNum;
        int randX, randY;
        bool check = false;
        TileScript[] allTiles = prepPhaseTiles.GetComponentsInChildren<TileScript>();
        //make sure to not rand a same place twice
            randX = Random.Range(0, 10);
            randY = Random.Range(0, 10);

            tileNum = randX + (randY * 10);
        //do
        //{

        //    foreach (TileScript tile in allTiles)
        //    {
        //        if (tile.tileNumber == tileNum)
        //        {
        //            tileScript = tile.GetComponent<TileScript>();
        //            if (tile.isThisTileChecked)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                check = true;
        //            }
        //        }
        //    }

        //} while (check);

        foreach (TileScript tile in allTiles)
        {
            if (tile.tileNumber == tileNum)
            {
                tileScript = tile.GetComponent<TileScript>();
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
        }

        PlayersTurn();

    }


    bool CheckPlace(int tileNum)
    {
        foreach (TileScript tile in prepPhaseTiles.GetComponentsInChildren<TileScript>())
        {
            if (tile.tileNumber == tileNum)
            {
                if(tile.isThisTileChecked)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
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
        int random = 1; //Random.Range(0, 2);
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
