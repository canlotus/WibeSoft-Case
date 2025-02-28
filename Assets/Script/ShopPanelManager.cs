using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopPanel;           // The shop panel UI object
    public Button closeButton;             // Button to close the shop panel
    public TextMeshProUGUI goldText;       // Displays current gold
    public TextMeshProUGUI wheatSeedText;  // Displays wheat seed count
    public TextMeshProUGUI cornSeedText;   // Displays corn seed count

    [Header("Purchase Settings")]
    public int bundleCount = 3;            // Seeds are bought in bundles of 3
    public int wheatSeedPrice = 30;        // Price for a bundle of wheat seeds
    public int cornSeedPrice = 40;         // Price for a bundle of corn seeds

    [Header("Player Data")]
    public int currentGold = 1000;         // Starting gold
    private int wheatSeedCount = 0;        // Wheat seed inventory count
    private int cornSeedCount = 0;         // Corn seed inventory count

    // Keys for saving with PlayerPrefs
    private const string GoldKey = "Gold";
    private const string WheatSeedKey = "WheatSeedCount";
    private const string CornSeedKey = "CornSeedCount";

    void Start()
    {
        LoadShopData();
        UpdateUI();
        shopPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseShopPanel);
        }
    }

    public void OpenShopPanel()
    {
        shopPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }

    public void BuyWheatSeeds()
    {
        if (currentGold >= wheatSeedPrice)
        {
            currentGold -= wheatSeedPrice;
            wheatSeedCount += bundleCount;
            UpdateUI();
            SaveShopData();
            Debug.Log("Purchased " + bundleCount + " wheat seeds for " + wheatSeedPrice + " gold.");
        }
        else
        {
            Debug.Log("Not enough gold to purchase wheat seeds.");
        }
    }

    public void BuyCornSeeds()
    {
        if (currentGold >= cornSeedPrice)
        {
            currentGold -= cornSeedPrice;
            cornSeedCount += bundleCount;
            UpdateUI();
            SaveShopData();
            Debug.Log("Purchased " + bundleCount + " corn seeds for " + cornSeedPrice + " gold.");
        }
        else
        {
            Debug.Log("Not enough gold to purchase corn seeds.");
        }
    }

    void UpdateUI()
    {
        if (goldText != null)
            goldText.text = "Gold: " + currentGold.ToString();
        if (wheatSeedText != null)
            wheatSeedText.text = "Price:" + wheatSeedPrice;
        if (cornSeedText != null)
            cornSeedText.text = "Price: " + cornSeedPrice;

        // Optionally disable purchase buttons if not enough gold
        if (currentGold < wheatSeedPrice && wheatSeedText != null) { }
        // (You can add additional logic here if you have button references)
    }

    void SaveShopData()
    {
        PlayerPrefs.SetInt(GoldKey, currentGold);
        PlayerPrefs.SetInt(WheatSeedKey, wheatSeedCount);
        PlayerPrefs.SetInt(CornSeedKey, cornSeedCount);
        PlayerPrefs.Save();
    }

    void LoadShopData()
    {
        if (PlayerPrefs.HasKey(GoldKey))
            currentGold = PlayerPrefs.GetInt(GoldKey);
        if (PlayerPrefs.HasKey(WheatSeedKey))
            wheatSeedCount = PlayerPrefs.GetInt(WheatSeedKey);
        if (PlayerPrefs.HasKey(CornSeedKey))
            cornSeedCount = PlayerPrefs.GetInt(CornSeedKey);
    }

    // Public getters and decrement methods for use in the crop selection panel
    public int GetWheatSeedCount() { return wheatSeedCount; }
    public int GetCornSeedCount() { return cornSeedCount; }
    public void DecreaseWheatSeedCount(int amount)
    {
        wheatSeedCount = Mathf.Max(0, wheatSeedCount - amount);
        UpdateUI();
        SaveShopData();
    }
    public void DecreaseCornSeedCount(int amount)
    {
        cornSeedCount = Mathf.Max(0, cornSeedCount - amount);
        UpdateUI();
        SaveShopData();
    }
}