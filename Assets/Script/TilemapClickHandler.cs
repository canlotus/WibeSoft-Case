using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapClickHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase farmlandTile;
    public CropSelectionManager ekinSecimManager;
    public TileConversionManager tileConversionManager;
    public bool buildingModeActive = false;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = tilemap.GetTile(tilePos);
            if (clickedTile == null)
            {
                return;
            }
            string tileKey = $"{tilePos.x}_{tilePos.y}_IsEmpty";
            if (buildingModeActive)
            {
                if (clickedTile.name == tileConversionManager.houseTile.name ||
                    clickedTile.name == tileConversionManager.factoryTile.name)
                {
                    tileConversionManager.PrepareBuildingMove(tilePos, clickedTile);
                }
                else
                {
                    if (PlayerPrefs.HasKey(tileKey) && PlayerPrefs.GetInt(tileKey) == 1)
                    {
                        tileConversionManager.ActivateConversionMode();
                    }
                    else
                    {
                    }
                }
                return;
            }
            if (PlayerPrefs.HasKey(tileKey) && PlayerPrefs.GetInt(tileKey) == 1)
            {
                ekinSecimManager.OpenCropSelectionPanel(tilePos);
                return;
            }
            if (clickedTile == farmlandTile)
            {
                ekinSecimManager.OpenCropSelectionPanel(tilePos);
            }
        }
    }
}