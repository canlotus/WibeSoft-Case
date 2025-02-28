using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class CropSelectionManager : MonoBehaviour
{
    public GameObject cropSelectionPanel; // UI panel for crop selection
    public Tilemap tilemap;               // The tilemap reference
    private Vector3Int selectedCell;      // Selected cell position

    public GameObject wheatPrefab;        // Wheat crop prefab
    public GameObject cornPrefab;         // Corn crop prefab

    public Button plantWheatButton;       // Button to plant wheat
    public Button plantCornButton;        // Button to plant corn

    public TextMeshProUGUI wheatInventoryText; // Displays wheat seed count
    public TextMeshProUGUI cornInventoryText;  // Displays corn seed count

    // Reference to the shop manager (which holds seed counts)
    public ShopPanelManager shopManager;

    // Key for saving planted crop state
    private const string CropStateKey = "CropState";

    void Start()
    {
        cropSelectionPanel.SetActive(false);
        UpdateSeedUI();
        LoadCropState();
    }

    public void OpenCropSelectionPanel(Vector3Int cellPos)
    {
        selectedCell = cellPos;
        cropSelectionPanel.SetActive(true);
        UpdateSeedUI();
    }

    public void PlantWheat()
    {
        Vector3 cropPos = tilemap.GetCellCenterWorld(selectedCell) + new Vector3(0, 0.3f, 0);
        // Check if a crop is already planted at this cell using OverlapBox.
        Collider2D existingCrop = Physics2D.OverlapCircle(cropPos, tilemap.cellSize.x * 0.2f);
        if (existingCrop != null && existingCrop.CompareTag("Crop"))
        {
            Debug.Log("A crop is already planted at this cell!");
            return;
        }

        if (shopManager.GetWheatSeedCount() <= 0)
        {
            Debug.Log("No wheat seeds available!");
            return;
        }
        shopManager.DecreaseWheatSeedCount(1);
        UpdateSeedUI();

        Instantiate(wheatPrefab, cropPos, Quaternion.identity);
        SaveCrop(selectedCell, "Wheat");
        cropSelectionPanel.SetActive(false);
    }

    public void PlantCorn()
    {
        Vector3 cropPos = tilemap.GetCellCenterWorld(selectedCell) + new Vector3(0, 0.3f, 0);
        Collider2D existingCrop = Physics2D.OverlapCircle(cropPos, tilemap.cellSize.x * 0.2f);
        if (existingCrop != null && existingCrop.CompareTag("Crop"))
        {
            Debug.Log("A crop is already planted at this cell!");
            return;
        }

        if (shopManager.GetCornSeedCount() <= 0)
        {
            Debug.Log("No corn seeds available!");
            return;
        }
        shopManager.DecreaseCornSeedCount(1);
        UpdateSeedUI();

        Instantiate(cornPrefab, cropPos, Quaternion.identity);
        SaveCrop(selectedCell, "Corn");
        cropSelectionPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        cropSelectionPanel.SetActive(false);
    }

    void UpdateSeedUI()
    {
        if (wheatInventoryText != null)
            wheatInventoryText.text = "Wheat Seeds: " + shopManager.GetWheatSeedCount();
        if (cornInventoryText != null)
            cornInventoryText.text = "Corn Seeds: " + shopManager.GetCornSeedCount();

        if (plantWheatButton != null)
            plantWheatButton.interactable = shopManager.GetWheatSeedCount() > 0;
        if (plantCornButton != null)
            plantCornButton.interactable = shopManager.GetCornSeedCount() > 0;
    }

    // Save a newly planted crop to PlayerPrefs
    void SaveCrop(Vector3Int cell, string cropType)
    {
        // Format: "x,y,cropType;"
        string newEntry = cell.x + "," + cell.y + "," + cropType + ";";
        string existing = PlayerPrefs.GetString(CropStateKey, "");
        existing += newEntry;
        PlayerPrefs.SetString(CropStateKey, existing);
        PlayerPrefs.Save();
        Debug.Log("Crop saved: " + newEntry);
    }

    // Load saved crops from PlayerPrefs and instantiate them
    void LoadCropState()
    {
        if (PlayerPrefs.HasKey(CropStateKey))
        {
            string data = PlayerPrefs.GetString(CropStateKey);
            if (!string.IsNullOrEmpty(data))
            {
                string[] entries = data.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    string[] parts = entry.Split(',');
                    if (parts.Length == 3)
                    {
                        int x = int.Parse(parts[0]);
                        int y = int.Parse(parts[1]);
                        string cropType = parts[2];
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
                        Vector3 pos = tilemap.GetCellCenterWorld(cellPos) + new Vector3(0, 0.3f, 0);
                        if (cropType == "Wheat")
                        {
                            Instantiate(wheatPrefab, pos, Quaternion.identity);
                        }
                        else if (cropType == "Corn")
                        {
                            Instantiate(cornPrefab, pos, Quaternion.identity);
                        }
                    }
                }
                Debug.Log("Crop state loaded.");
            }
        }
    }
}