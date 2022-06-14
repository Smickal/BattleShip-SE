using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    GameManager gameManager;
    GamePhase gamePhase;
    RaycastHit2D hit;
    SpriteRenderer spriteRenderer;

    bool missileHit = false;
    bool isTileInteractAble = true;


    bool gameplayPhase = false;


    public bool thisTileSelected = false;
    public bool isTileGrey = false;
    public bool isTileRed = false;
    public bool isTileinAttackRange = false;
    public bool isThisTileUsed = false;
    public bool isThisTileChecked = false;
    public bool isGamePhaseStarted = false;
    bool thisTileUsedForMissedOrHit = false;

    Color32 defaultHoverColor = Color.white;
    Color32 falseHoverColor = Color.red;

    public bool isMoveAble = false;
    public int shipTileId;

    Color32 defaultColor;

    GameObject tempTile;

    //public Color32 hoverColor;
    public int tileNumber;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gamePhase = gameManager.gameObject.GetComponent<GamePhase>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        defaultColor = spriteRenderer.color;
    }

    
    void Update()
    {
        if (isTileInteractAble)
        {
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos, Vector3.forward, float.MaxValue);
            if (hit.collider && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (hit.collider.gameObject.name == gameObject.name)
                {
                    if (!missileHit && !gameplayPhase)
                    {
                        gameManager.TileClicked(hit.collider.gameObject);
                    }

                    if (gameplayPhase && !isThisTileUsed && isTileinAttackRange)
                    {
                        gamePhase.TileCliked(hit.collider.gameObject);
                    }

                }
                //Debug.Log("Hit");
            }
        }

        
    }

    public void DisableInteractive()
    {
        isTileInteractAble = false;
    }


    public void EnableInteractive()
    {
        isTileInteractAble = true;
    }

    public void SetTilePlayerDamaged()
    {
        spriteRenderer.color = Color.red;
        isTileRed = true;
    }

    public void SetTilePlayerMissed()
    {
        spriteRenderer.color = Color.gray;
        isTileGrey = true;
        isThisTileChecked = true;
        thisTileUsedForMissedOrHit = true;
    }

    public void SetTilePlayerHit(int shipTile)
    {
        spriteRenderer.color = Color.red;
        isTileRed = true;
        isThisTileChecked = true;
        shipTileId = shipTile;
    }

    public void ResetColor()
    {
        if (thisTileUsedForMissedOrHit) return;
        spriteRenderer.color = defaultColor;
    }


    public void SetColorForSelected()
    {
        spriteRenderer.color = Color.yellow;
    }

    public void SetColorNormalSelected()
    {
        spriteRenderer.color = defaultHoverColor;
    }

    public void setColorFalseSelected()
    {
        spriteRenderer.color = falseHoverColor;
    }

    public void SetToGreenColor()
    {
        spriteRenderer.color = Color.green;
    }


    private void OnMouseEnter()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePos, Vector3.forward, float.MaxValue);
        if (hit.collider.gameObject.name == gameObject.name)
        {
            tempTile = hit.collider.gameObject;
        }
        else return;

        if(!thisTileSelected)
        {
            if (isGamePhaseStarted)
            {
                if(isTileinAttackRange)
                {
                    SetColorNormalSelected();
                }
                else
                {
                    setColorFalseSelected();
                }

            }
            else
            {
                if (isTileInteractAble)
                {
                    if (gameManager.CheckShipOutOfbounds(tempTile))
                    {
                        SetColorNormalSelected();
                    }
                    else
                    {
                        setColorFalseSelected();
                    }
                }
            }
        }
        
    }

    private void OnMouseExit()
    {
        if (!thisTileSelected) ResetColor();
        if (thisTileSelected) return;
        if (isTileinAttackRange && isGamePhaseStarted)
        {
            SetToGreenColor();
        }
        else
        {
            ResetColor();
        }

    }

    public void EnableGamePhase()
    {
        gameplayPhase = true;
    }

    public void ThisTileSelected()
    {
        thisTileSelected = true;
    }

    public void ThisTileUsed()
    {
        isThisTileUsed = true;
        isTileInteractAble = false;
    }

    public void SetDefaultColor()
    {
        spriteRenderer.color = defaultColor;
    }
}
