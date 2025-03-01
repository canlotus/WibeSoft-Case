using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System;

public class CropGrowth : MonoBehaviour
{
    public string cropType;
    public float totalGrowthTime;
    public float stage1Duration;

    public Sprite stage1Sprite;
    public Sprite stage2Sprite;
    public Sprite stage3Sprite;

    public GameObject harvestPrefab;
    private GameObject harvestIndicator;

    public GameObject growthUIPanel;
    public Slider growthSlider;
    public TextMeshProUGUI timeRemainingText;
    public Button closeButton;

    private SpriteRenderer sr;
    private long plantingTicks;
    private bool planted = false;
    private bool isFullyGrown = false;
    private bool isHarvested = false;
    private string cropID;
    private Vector3Int tilePos;

    public static CropGrowth activeGrowthInstance;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        tilePos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (UIManager.Instance != null)
        {
            growthUIPanel = UIManager.Instance.globalGrowthUIPanel;
            if (growthUIPanel != null)
            {
                growthSlider = growthUIPanel.transform.Find("Slider")?.GetComponent<Slider>();
                timeRemainingText = growthUIPanel.transform.Find("TimeRemainingText")?.GetComponent<TextMeshProUGUI>();
                closeButton = growthUIPanel.transform.Find("CloseButton")?.GetComponent<Button>();
                if (closeButton != null)
                {
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(HideGrowthUI);
                }
            }
        }
    }

    void Start()
    {
        cropID = GenerateCropID();
        LoadCropData();
        if (isHarvested)
        {
            Destroy(gameObject);
            return;
        }
        planted = true;
        UpdateCropVisual();
        PlayerPrefs.SetInt($"{tilePos.x}_{tilePos.y}_IsEmpty", 0);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (!planted || isFullyGrown || isHarvested) return;

        long currentTicks = DateTime.UtcNow.Ticks;
        float elapsed = (currentTicks - plantingTicks) / 10000000f;

        if (elapsed >= totalGrowthTime)
        {
            sr.sprite = stage3Sprite;
            isFullyGrown = true;
            ShowHarvestIndicator();
            SaveCropData();
        }
        else if (elapsed >= stage1Duration)
        {
            sr.sprite = stage2Sprite;
        }
        else
        {
            sr.sprite = stage1Sprite;
        }

        if (growthUIPanel.activeSelf && activeGrowthInstance == this)
        {
            UpdateUI();
        }
    }

    void OnMouseDown()
    {
        if (isFullyGrown)
        {
            HarvestCrop();
            return;
        }
        if (growthUIPanel == null) return;
        if (activeGrowthInstance != null && activeGrowthInstance != this)
        {
            activeGrowthInstance.HideGrowthUI();
        }
        activeGrowthInstance = this;
        growthUIPanel.SetActive(true);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (!planted) return;
        long currentTicks = DateTime.UtcNow.Ticks;
        float elapsed = (currentTicks - plantingTicks) / 10000000f;
        float remaining = Mathf.Max(0, totalGrowthTime - elapsed);
        if (isFullyGrown)
        {
            timeRemainingText.text = "Ready";
            growthSlider.value = 1f;
        }
        else
        {
            float progress = Mathf.Clamp01(elapsed / totalGrowthTime);
            growthSlider.value = progress;
            TimeSpan ts = TimeSpan.FromSeconds(remaining);
            timeRemainingText.text = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
    }

    void UpdateCropVisual()
    {
        long currentTicks = DateTime.UtcNow.Ticks;
        float elapsed = (currentTicks - plantingTicks) / 10000000f;
        if (elapsed >= totalGrowthTime)
        {
            sr.sprite = stage3Sprite;
            isFullyGrown = true;
            ShowHarvestIndicator();
        }
        else if (elapsed >= stage1Duration)
        {
            sr.sprite = stage2Sprite;
        }
        else
        {
            sr.sprite = stage1Sprite;
        }
    }

    void HarvestCrop()
    {
        isHarvested = true;
        RemoveCropData();
        InventoryManager.Instance.AddItem(cropType, 1);
        string tileKey = $"{tilePos.x}_{tilePos.y}_IsEmpty";
        PlayerPrefs.SetInt(tileKey, 1);
        PlayerPrefs.Save();
        Collider2D cropCollider = GetComponent<Collider2D>();
        if (cropCollider != null)
            Destroy(cropCollider);
        if (harvestIndicator != null)
            Destroy(harvestIndicator);
        Destroy(gameObject);
    }

    void ShowHarvestIndicator()
    {
        if (harvestPrefab != null && harvestIndicator == null)
        {
            Vector3 indicatorPos = transform.position + new Vector3(0, 0.5f, 0);
            harvestIndicator = Instantiate(harvestPrefab, indicatorPos, Quaternion.identity);
        }
    }

    public void HideGrowthUI()
    {
        if (growthUIPanel != null)
            growthUIPanel.SetActive(false);
    }

    private string GenerateCropID()
    {
        return $"{cropType}_{tilePos.x}_{tilePos.y}";
    }

    void SaveCropData()
    {
        PlayerPrefs.SetString(cropID + "_PlantTime", plantingTicks.ToString());
        PlayerPrefs.SetInt(cropID + "_GrowthStatus", isFullyGrown ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadCropData()
    {
        if (PlayerPrefs.HasKey(cropID + "_PlantTime"))
        {
            plantingTicks = long.Parse(PlayerPrefs.GetString(cropID + "_PlantTime"));
            isFullyGrown = PlayerPrefs.GetInt(cropID + "_GrowthStatus", 0) == 1;
        }
        else
        {
            plantingTicks = DateTime.UtcNow.Ticks;
            isFullyGrown = false;
            PlayerPrefs.SetString(cropID + "_PlantTime", plantingTicks.ToString());
            PlayerPrefs.SetInt(cropID + "_GrowthStatus", 0);
            PlayerPrefs.Save();
        }
    }

    void RemoveCropData()
    {
        string tileKey = $"{tilePos.x}_{tilePos.y}_IsEmpty";
        PlayerPrefs.DeleteKey(cropID + "_PlantTime");
        PlayerPrefs.DeleteKey(cropID + "_GrowthStatus");
        const string CropStateKey = "CropState";
        string existing = PlayerPrefs.GetString(CropStateKey, "");
        string entryToRemove = $"{tilePos.x},{tilePos.y},{cropType};";
        if (existing.Contains(entryToRemove))
        {
            existing = existing.Replace(entryToRemove, "");
            PlayerPrefs.SetString(CropStateKey, existing);
        }
        PlayerPrefs.SetInt(tileKey, 1);
        PlayerPrefs.Save();
    }
}