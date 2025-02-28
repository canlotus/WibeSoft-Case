using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapClickHandler : MonoBehaviour
{
    public Tilemap tilemap;                   // Tilemap referansı
    public TileBase farmlandTile;               // Farmland (tarla) tile asset'i
    public CropSelectionManager ekinSecimManager;   // Ekim panelini yöneten manager
    public TileConversionManager tileConversionManager; // Conversion modunu yöneten manager

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = tilemap.GetTile(tilePos);
            if (clickedTile == null) return;

            // Conversion modu aktifse, conversion işlemleri geçerli; aksi halde normal mod
            if (tileConversionManager != null && tileConversionManager.conversionModeActive)
            {
                // Eğer tıklanan tile farmland tile olarak belirlenmişse (isimleri karşılaştırıyoruz)
                if (clickedTile.name == farmlandTile.name)
                {
                    ekinSecimManager.OpenCropSelectionPanel(tilePos);
                }
                return;
            }
            // Normal modda, farmland tile ise ekim panelini aç
            if (clickedTile.name == farmlandTile.name)
            {
                ekinSecimManager.OpenCropSelectionPanel(tilePos);
            }
        }
    }
}