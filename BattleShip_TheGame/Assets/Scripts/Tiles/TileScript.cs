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

    bool thisTileSelected = false;
    bool gameplayPhase = false;

    public bool isThisTileUsed = false;
    public bool isThisTileChecked = false;
    public bool isGamePhaseStarted = false;

    Color32 defaultHoverColor = Color.white;
    Color32 falseHoverColor = Color.red;

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
        if(isTileInteractAble)
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

                    if(gameplayPhase && !isThisTileUsed)
                    {
                        gamePhase.TileCliked(hit.collider.gameObject);
                    }
                }
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
        
    }

    public void SetTilePlayerMissed()
    {
        spriteRenderer.color = Color.gray;
        isThisTileChecked = true;
    }

    public void SetTilePlayerHit()
    {
        spriteRenderer.color = Color.green;
        isThisTileChecked = true;
    }

    public void ResetColor()
    {
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


    public void ResetColorNode()
    {
        spriteRenderer.color = defaultColor;
    }


    private void OnMouseEnter()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePos, Vector3.forward, float.MaxValue);
        if (hit.collider.gameObject.name == gameObject.name)
        {
            tempTile = hit.collider.gameObject;
        }

        if(!thisTileSelected)
        {
            if (isGamePhaseStarted)
            {
                SetColorNormalSelected();
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
        if (!thisTileSelected)
            ResetColorNode();
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
}
