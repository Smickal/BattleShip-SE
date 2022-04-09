using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    GameManager gameManager;
    GamePhase gamePhase;
    Ray ray;
    RaycastHit2D hit;
    SpriteRenderer spriteRenderer;

    bool missileHit = false;
    bool isTileInteractAble = true;

    bool gameplayPhase = false;
    bool thisTileSelected = false;

    public bool isThisTileUsed = false;
    public bool isThisTileChecked = false;
    //public Color32 hoverColor;
    Color32 defaultColor;
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


    private void OnMouseEnter()
    {
        if (isTileInteractAble && !thisTileSelected)
            spriteRenderer.color = Color.white;
    }

    private void OnMouseExit()
    {
        if (isTileInteractAble && !thisTileSelected)
            spriteRenderer.color = defaultColor;
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
