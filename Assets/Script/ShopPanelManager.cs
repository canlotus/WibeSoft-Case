using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanelManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeButton;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI wheatSeedText;
    public TextMeshProUGUI cornSeedText;

    public int bundleCount = 3;
    public int wheatSeedPrice = 30;
    public int cornSeedPrice = 40;

    public int currentGold = 1000;
    private int wheatSeedCount = 0;
    private int cornSeedCount = 0;

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