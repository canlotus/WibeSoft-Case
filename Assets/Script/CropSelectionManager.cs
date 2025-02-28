using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class CropSelectionManager : MonoBehaviour
{
    public GameObject cropSelectionPanel; 
    public Tilemap tilemap;               
    private Vector3Int selectedCell;      

    public GameObject wheatPrefab;        
    public GameObject cornPrefab;         

    public Button plantWheatButton;       
    public Button plantCornButton;        

    public TextMeshProUGUI wheatInventoryText;
    public TextMeshProUGUI cornInventoryText;  

    public ShopPanelManager shopManager;

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
        if (!IsTileEmpty(selectedCell))
        {
            Debug.Log("Bu tarla dolu, ekim yapılamaz!");
            return;
        }

        Vector3 cropPos = tilemap.GetCellCenterWorld(selectedCell) + new Vector3(0, 0.3f, 0);

        if (shopManager.GetWheatSeedCount() <= 0)
        {
            Debug.Log("No wheat seeds available!");
            return;
        }

        shopManager.DecreaseWheatSeedCount(1);
        UpdateSeedUI();

        Instantiate(wheatPrefab, cropPos, Quaternion.identity);

        //  Bu alan artık dolu olarak işaretleniyor!
        string tileKey = $"{selectedCell.x}_{selectedCell.y}_IsEmpty";
        PlayerPrefs.SetInt(tileKey, 0);
        PlayerPrefs.Save();
        Debug.Log($"🌾 Bu tarla ekildi! (Key: {tileKey}, Value: {PlayerPrefs.GetInt(tileKey)})");

        SaveCrop(selectedCell, "Wheat");
        cropSelectionPanel.SetActive(false);
    }

    public void PlantCorn()
    {
        if (!IsTileEmpty(selectedCell))
        {
            Debug.Log("Bu tarla dolu, ekim yapılamaz!");
            return;
        }

        Vector3 cropPos = tilemap.GetCellCenterWorld(selectedCell) + new Vector3(0, 0.3f, 0);

        if (shopManager.GetCornSeedCount() <= 0)
        {
            Debug.Log("No wheat seeds available!");
            return;
        }

        shopManager.DecreaseCornSeedCount(1);
        UpdateSeedUI();

        Instantiate(cornPrefab, cropPos, Quaternion.identity);

        // Bu alan artık dolu olarak işaretleniyor!
        string tileKey = $"{selectedCell.x}_{selectedCell.y}_IsEmpty";
        PlayerPrefs.SetInt(tileKey, 0);
        PlayerPrefs.Save();
        Debug.Log($"🌾 Bu tarla ekildi! (Key: {tileKey}, Value: {PlayerPrefs.GetInt(tileKey)})");

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
    bool IsTileEmpty(Vector3Int cell)
    {
        string tileKey = $"{cell.x}_{cell.y}_IsEmpty";

        // Eğer tarla boş olarak kaydedilmişse, ekime izin ver
        if (PlayerPrefs.HasKey(tileKey) && PlayerPrefs.GetInt(tileKey) == 1)
        {
            return true;
        }

        // Fiziksel olarak ekin olup olmadığını kontrol et
        Vector3 cropPos = tilemap.GetCellCenterWorld(cell) + new Vector3(0, 0.3f, 0);
        Collider2D existingCrop = Physics2D.OverlapCircle(cropPos, tilemap.cellSize.x * 0.2f);
        if (existingCrop != null && existingCrop.CompareTag("Crop"))
        {
            return false;
        }

        // Varsayılan olarak boş kabul et
        return true;
    }
}