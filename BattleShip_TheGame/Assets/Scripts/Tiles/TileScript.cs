using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    GameManager gameManager;
    Ray ray;
    RaycastHit2D hit;

    private bool missileHit = false;
    Color32[] hitColor = new Color32[2];
    public int tileNumber;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePos, Vector3.forward, float.MaxValue);
        if (hit.collider && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider.gameObject.name == gameObject.name)
            {
                if (!missileHit)
                {
                    gameManager.TileClicked(hit.collider.gameObject);
                }
            }
        }
    }


}
