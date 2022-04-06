using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNumbering : MonoBehaviour
{
    // Start is called before the first frame update
    public TileScript[] tileNumber;
    int count = 0;
    private void Awake()
    {
        foreach(TileScript tile in tileNumber)
        {
            tile.tileNumber = count;
            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
