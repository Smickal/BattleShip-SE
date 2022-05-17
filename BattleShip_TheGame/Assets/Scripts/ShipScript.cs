using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> touchTiles = new List<GameObject>();
    [Header("Ship Attributes")]
    public int shipID;
    public int shipHP;

    [Header("Ship Rotation Atrributes")]
    public float offSet = 0f;
    public int shipSize = 5;

    float XOffset = 0f;
    float YOffset = 0f;
    float nextZRotation = 90f;

    public int currentRotation = 0;

    public bool isShipVertical = true;

    GameObject clickedTile;

    private void Start()
    {
        YOffset = offSet;
        XOffset = 0f;

        shipHP = shipSize;
    }

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffSetVec(Vector3 tilePos)
    {
        return new Vector3(tilePos.x + XOffset, tilePos.y + YOffset, 2);
    }

    public Vector3 GettOffset()
    {
        return new Vector3(XOffset, YOffset, 2);
    }



    public void RotateLeftCurrentShip()
    {
        ClearTileList();
        currentRotation += 90;
        transform.localEulerAngles = new Vector3(0, 0, currentRotation);

        if (currentRotation == 360) currentRotation = 0;
        SetCurentOffset();

        SetPosition();
    }

    public void RotateRightCurrentShip()
    {
        ClearTileList();
        currentRotation -= 90;
        transform.localEulerAngles = new Vector3(0, 0, currentRotation);

        if (currentRotation == -360) currentRotation = 0;
        SetCurentOffset();

        SetPosition();
    }

    public void SetShipRotation(int shipRotation)
    {
        ClearTileList();
        currentRotation = shipRotation;
        transform.localEulerAngles = new Vector3(0, 0, shipRotation);
        SetCurentOffset();
        SetPosition();
    }


    public void SetPosition()
    {
        transform.localPosition = new Vector3(XOffset,YOffset, 2);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }


    public void ResetRotation()
    {

        ClearTileList();
        gameObject.
        transform.localEulerAngles = new Vector3(0, 0, 0);
        isShipVertical = !isShipVertical;
        gameObject.transform.SetParent(null);

        currentRotation = 0;
        XOffset = 0f;
        YOffset = offSet;
    }

    void SetCurentOffset()
    {
        if (currentRotation == 90 || currentRotation == -270)
        {
            //left
            XOffset = -offSet;
            YOffset = 0f;
        }
        else if (currentRotation == 180 || currentRotation == -180)
        {
            // up
            XOffset = 0f;
            YOffset = -offSet;
        }
        else if (currentRotation == -90 || currentRotation == 270)
        {
            //right
            XOffset = offSet;
            YOffset = 0f;
        }
        else if (currentRotation == 0)
        {
            XOffset = 0f;
            YOffset = offSet;
            //down
        }

    }


}
