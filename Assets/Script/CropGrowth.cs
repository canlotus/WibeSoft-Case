using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CropGrowth : MonoBehaviour
{
    [Header("Crop Settings")]
    public string cropType;           // "Wheat" veya "Corn"
    public float totalGrowthTime;     // Toplam büyüme süresi (saniye cinsinden)
    public float stage1Duration;      // Stage 1 süresi (saniye cinsinden)
    // Stage2, stage1Duration'dan totalGrowthTime'a kadar; stage3, totalGrowthTime tamamlandığında

    [Header("Growth Sprites")]
    public Sprite stage1Sprite;       // Stage 1 sprite'ı (ilk görüntü)
    public Sprite stage2Sprite;       // Stage 2 sprite'ı (orta aşama)
    public Sprite stage3Sprite;       // Stage 3 sprite'ı (tam olgunluk)

    [Header("Growth UI")]
    public GameObject growthUIPanel;  // Büyüme UI paneli (slider ve kalan süre TMP'si)
    public Slider growthSlider;       // Büyüme ilerleme slider'ı (0-1 arası)
    public TextMeshProUGUI timeRemainingText; // Kalan süreyi gösteren TMP text

    private SpriteRenderer sr;
    private long plantingTicks;       // Ekim zamanı (UTC ticks)
    private bool planted = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (growthUIPanel != null)
            growthUIPanel.SetActive(false);
    }

    void Start()
    {
        // Crop eklendiğinde ekim zamanını kaydet (her crop prefab'ı kendine özgü bir ID ile kaydedilebilir)
        string key = gameObject.name + "_PlantTime";
        if (PlayerPrefs.HasKey(key))
        {
            plantingTicks = long.Parse(PlayerPrefs.GetString(key));
        }
        else
        {
            plantingTicks = DateTime.UtcNow.Ticks;
            PlayerPrefs.SetString(key, plantingTicks.ToString());
            PlayerPrefs.Save();
        }
        planted = true;
    }

    void Update()
    {
        if (!planted)
            return;

        // Geçen süreyi hesapla (saniye cinsinden)
        long currentTicks = DateTime.UtcNow.Ticks;
        float elapsed = (currentTicks - plantingTicks) / 10000000f; // 1 saniye = 10 milyon tick

        // Büyüme aşamasına göre sprite'ı ayarla
        if (elapsed < stage1Duration)
        {
            sr.sprite = stage1Sprite;
        }
        else if (elapsed < totalGrowthTime)
        {
            sr.sprite = stage2Sprite;
        }
        else
        {
            sr.sprite = stage3Sprite;
        }

        // Eğer büyüme UI paneli açıksa slider ve kalan süreyi güncelle
        if (growthUIPanel != null && growthUIPanel.activeSelf)
        {
            float progress = Mathf.Clamp01(elapsed / totalGrowthTime);
            growthSlider.value = progress;
            float remaining = Mathf.Max(0, totalGrowthTime - elapsed);
            TimeSpan ts = TimeSpan.FromSeconds(remaining);
            timeRemainingText.text = string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
        }
    }

    // Crop üzerine tıklandığında büyüme UI panelini aç
    void OnMouseDown()
    {
        if (growthUIPanel != null)
            growthUIPanel.SetActive(true);
    }

    // UI'yi kapatmak için dışarıdan çağrılabilir
    public void HideGrowthUI()
    {
        if (growthUIPanel != null)
            growthUIPanel.SetActive(false);
    }
}