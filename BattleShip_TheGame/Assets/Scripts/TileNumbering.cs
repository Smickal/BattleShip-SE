using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNumbering : MonoBehaviour
{
    // Start is called before the first frame update
    public TileScript[] tileNumber;
    public string tileParent;

    int count = 0;
    private void Awake()
    {
        foreach(TileScript tile in tileNumber)
        {
            tile.tileNumber = count;
            count++;
        }
    }

    public void ResetAllColor()
    {
        foreach (TileScript tile in gameObject.GetComponentsInChildren<TileScript>())
        {

            if (tile.isTileRed)
            {
                tile.SetTilePlayerDamaged();
                continue;
            }
            else if (tile.isTileGrey)
            {
                tile.SetTilePlayerMissed();
                continue;
            }
            tile.ResetColor();
            tile.isMoveAble = false;
            
       }
    }

    public void ResetForDamageAllColor()
    {
        foreach (TileScript tile in gameObject.GetComponentsInChildren<TileScript>())
        {
            if (!tile.isTileRed && !tile.isTileGrey) tile.SetDefaultColor();
            if (tile.isTileRed && tile.isTileGrey)
            {
                tile.SetTilePlayerMissed();
                tile.isTileRed = false;
            }
            if (tile.isTileRed) tile.isTileRed = false;
            if (tile.isTileGrey)
            {
                tile.SetTilePlayerMissed();
            }

            tile.ResetColor();
            tile.isMoveAble = false;
        }
    }

}
