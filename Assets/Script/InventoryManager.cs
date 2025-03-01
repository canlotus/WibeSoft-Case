using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject inventoryPanel;
    public TextMeshProUGUI wheatText;
    public TextMeshProUGUI cornText;
    public Button closeButton;

    private const string WheatKey = "WheatInventory";
    private const string CornKey = "CornInventory";

    private int wheatCount = 0;
    private int cornCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadInventory();
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInventory);
        UpdateInventoryUI();
    }

    public void OpenInventory()
    {
        UpdateInventoryUI();
        inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void AddItem(string cropType, int amount)
    {
        if (cropType == "Wheat")
        {
            wheatCount += amount;
            PlayerPrefs.SetInt(WheatKey, wheatCount);
        }
        else if (cropType == "Corn")
        {
            cornCount += amount;
            PlayerPrefs.SetInt(CornKey, cornCount);
        }
        PlayerPrefs.Save();
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        if (wheatText != null)
            wheatText.text = $"Wheat: {wheatCount}";
        if (cornText != null)
            cornText.text = $"Corn: {cornCount}";
    }

    private void LoadInventory()
    {
        wheatCount = PlayerPrefs.GetInt(WheatKey, 0);
        cornCount = PlayerPrefs.GetInt(CornKey, 0);
    }
}