using UnityEngine;
using UnityEngine.Tilemaps;

public class CropSelectionManager : MonoBehaviour
{
    public GameObject cropSelectionPanel; // UI panel for crop selection
    public Tilemap tilemap;               // The tilemap
    private Vector3Int selectedTilePos;   // Selected tile position

    public GameObject wheatPrefab;        // Wheat crop prefab
    public GameObject cornPrefab;         // Corn crop prefab

    void Start()
    {
        cropSelectionPanel.SetActive(false);
    }

    public void OpenCropSelectionPanel(Vector3Int tilePos)
    {
        selectedTilePos = tilePos;
        cropSelectionPanel.SetActive(true);
    }

    public void PlantWheat()
    {
        Vector3 cropPosition = tilemap.GetCellCenterWorld(selectedTilePos) + new Vector3(0, 0.3f, 0);
        Instantiate(wheatPrefab, cropPosition, Quaternion.identity);
        cropSelectionPanel.SetActive(false);
    }

    public void PlantCorn()
    {
        Vector3 cropPosition = tilemap.GetCellCenterWorld(selectedTilePos) + new Vector3(0, 0.3f, 0);
        Instantiate(cornPrefab, cropPosition, Quaternion.identity);
        cropSelectionPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        cropSelectionPanel.SetActive(false);
    }
}