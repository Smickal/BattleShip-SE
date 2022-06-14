using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> touchTiles = new List<GameObject>();
    [SerializeField] int maxBoatSize = 5;

    [Header("Ship Attributes")]
    public int shipID;
    public int shipHP;
    private int shipHeadPos;


    [Header("Ship Rotation Atrributes")]
    public float offSet = 0f;
    public int shipSize = 5;

    public float XOffset = 0f;
    public float YOffset = 0f;
    float nextZRotation = 90f;

    public bool isMoveAbleTile = false;



    public int currentRotation = 0;
    private int firstRotation;
    public bool isShipVertical = true;

    GameObject clickedTile;

    RaycastHit2D[] ray;
    BoxCollider2D boxCol;

    [Header("Layer Mask")]
    [SerializeField] LayerMask mask;

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();

        boxCol.enabled = false;
    }

    private void Start()
    {
        shipHP = shipSize;

        SetCurentOffset(currentRotation);
        SetPosition();
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


    public void SearchForAttackArea()
    {
        boxCol.enabled = true;
        ray = null;
        ray = Physics2D.BoxCastAll(boxCol.transform.position, boxCol.bounds.size, 0f, Vector3.forward, float.MaxValue, mask);

        //foreach(RaycastHit2D temp in ray)
        //{
        //    Debug.Log(temp.collider.gameObject.name);
        //}

        boxCol.enabled = false;
    }

    public RaycastHit2D[] GetAttackArea()
    {
        return ray;
    }



    public void RotateLeftCurrentShip()
    {
        ClearTileList();
        currentRotation += 90;
        transform.localEulerAngles = new Vector3(0, 0, currentRotation);

        if (currentRotation == 360) currentRotation = 0;
        SetCurentOffset(currentRotation);

        SetPosition();
    }

    public void RotateRightCurrentShip()
    {
        ClearTileList();
        currentRotation -= 90;
        transform.localEulerAngles = new Vector3(0, 0, currentRotation);

        if (currentRotation == -360) currentRotation = 0;
        SetCurentOffset(currentRotation);

        SetPosition();
    }

    public void SetShipRotation(int shipRotation)
    {
        ClearTileList();
        currentRotation = shipRotation;
        transform.localEulerAngles = new Vector3(0, 0, shipRotation);
        SetPosition();
        SetCurentOffset(shipRotation);
    }

    private void Update()
    {
        //SetPosition();
    }

    public void SetPosition()
    {
        transform.localPosition = new Vector3(XOffset,YOffset, 2);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
        shipHeadPos = clickedTile.GetComponent<TileScript>().tileNumber;
    }

    public GameObject GetClickedTile()
    {
        return clickedTile;
    }


    public void ResetRotation()
    {

        ClearTileList();
        transform.localEulerAngles = new Vector3(0, 0, 0);
        isShipVertical = !isShipVertical;
        gameObject.transform.SetParent(null);

        currentRotation = 0;
        SetCurentOffset(currentRotation);
    }

    public void SetCurentOffset(int offset)
    {
        if (offset == 90 || offset == -270)
        {
            //left
            //Debug.Log("left");
            XOffset = -offSet;
            YOffset = 0;
        }
        else if (offset == 180 || offset == -180)
        {
            // up
            //Debug.Log("Up");
            XOffset = 0;
            YOffset = -offSet;
        }
        else if (offset == -90 || offset == 270)
        {
            //right
            //Debug.Log("Right");
            XOffset = offSet;
            YOffset = 0;
        }
        else if (offset == 0)
        {
            //Debug.Log("Down");
            XOffset = 0;
            YOffset = offSet;
            //down
        }
        SetPosition();
    }

    public int GetMoveLength()
    {
        return maxBoatSize - shipSize + 2;
    }

    public void SetHeadShipPos(int pos)
    {
        shipHeadPos = pos;
    }

    public int GetShipHeadPos()
    {
        return shipHeadPos;
    }


    public void SaveCurrentRotation(int rot)
    {
        firstRotation = rot;
    }

    public void ResetToFirstRotation()
    {
        ClearTileList();
        currentRotation = firstRotation;

        transform.localEulerAngles = new Vector3(0, 0, firstRotation);
        isShipVertical = !isShipVertical;

        SetCurentOffset(currentRotation);
    }
}
