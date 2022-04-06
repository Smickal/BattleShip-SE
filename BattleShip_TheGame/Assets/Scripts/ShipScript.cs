using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> touchTiles = new List<GameObject>();
    public float XOffset = 0f;
    public float YOffset = 0f;
    float nextZRotation = 90f;
    public int shipSize = 5;

    public bool isShipVertical = true;

    GameObject clickedTile;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffSetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + XOffset,tilePos.y + YOffset,2);
    }

    public void RotateCurrentShip()
    {
        ClearTileList();
        transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
        nextZRotation *= -1;
        isShipVertical = !isShipVertical;

        float temp = XOffset;
        XOffset = YOffset;
        YOffset = temp;

        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVec)
    {
        transform.localPosition = new Vector3(newVec.x + XOffset, newVec.y + YOffset, 2);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }


    public void ResetRotation()
    {
        if (!isShipVertical)
        {
            ClearTileList();
            transform.localEulerAngles += new Vector3(0, 0, nextZRotation);
            nextZRotation *= -1;
            isShipVertical = !isShipVertical;

            float temp = XOffset;
            XOffset = YOffset;
            YOffset = temp;
        }
    }

}
