using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapClickHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase farmlandTile;
    public CropSelectionManager ekinSecimManager;
    public TileConversionManager tileConversionManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = tilemap.GetTile(tilePos);
            if (clickedTile == null) return;

            string tileKey = $"{tilePos.x}_{tilePos.y}_IsEmpty";

            //  **Eğer tarla boş olarak işaretlenmişse, ekim yapılabilir.**
            if (PlayerPrefs.HasKey(tileKey) && PlayerPrefs.GetInt(tileKey) == 1)
            {
                Debug.Log($" Bu tarla boş, ekime uygun! (Key: {tileKey}, Value: {PlayerPrefs.GetInt(tileKey)})");
                ekinSecimManager.OpenCropSelectionPanel(tilePos);
                return;
            }

            //  **Farmland kontrolü, tarlaya ekim için uygunsa paneli aç.**
            if (clickedTile.name == farmlandTile.name)
            {
                Debug.Log(" Farmland bulundu, ekim paneli açılıyor.");
                ekinSecimManager.OpenCropSelectionPanel(tilePos);
            }
        }
    }
}