using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Text;

public class TileConversionManager : MonoBehaviour
{
    [Header("Tilemap & Tile Settings")]
    public Tilemap tilemap;              // Çalıştığınız tilemap
    public TileBase farmlandTile;        // Dönüştürülecek (tarla) tile asset'i
    public TileBase roadTile;            // Dönüştürülemeyecek yol tile asset'i

    [Header("Conversion Settings")]
    public int conversionCostPerTile = 50;   // Her tile dönüşüm maliyeti
    public int currentGold = 1000;           // Başlangıç altın miktarı
    public TextMeshProUGUI goldDisplay;      // Altın göstergesi (TMP)

    [Header("UI Panel")]
    public GameObject conversionPanel;       // Dönüştürme paneli (UI)

    [Header("Highlight Settings")]
    public Color highlightColor = new Color(1f, 1f, 0f, 0.5f); // Yarı şeffaf sarı
    private Vector3Int lastHoveredPos;
    private bool isHovering = false;

    [Header("Conversion Mode")]
    public bool conversionModeActive = false; // Conversion modu aktif olduğunda dönüşüm işlemi yürütülür

    // Anahtar isimleri
    private const string GoldKey = "Gold";
    private const string TilemapStateKey = "TilemapState";

    void Start()
    {
        LoadGold();
        UpdateGoldDisplay();
        LoadTilemapState(); // Sahne yüklendiğinde kaydedilmiş dönüşüm durumunu uygula
        if (conversionPanel != null)
            conversionPanel.SetActive(false);
    }

    void Update()
    {
        if (!conversionModeActive)
        {
            // Conversion modu kapalıyken varsa önceki highlight'ı temizle
            if (isHovering && tilemap.HasTile(lastHoveredPos))
            {
                TileBase tile = tilemap.GetTile(lastHoveredPos);
                // Sadece eligible tile'lar (road ve farmland hariç) için temizle
                if (tile != null && tile != roadTile && tile != farmlandTile)
                    tilemap.SetColor(lastHoveredPos, Color.white);
                isHovering = false;
            }
            return;
        }

        // Conversion modu aktifken mouse hover ile eligible (dönüştürülebilir) tile'ı highlight et
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int hoveredPos = tilemap.WorldToCell(mouseWorldPos);

        if (!isHovering || hoveredPos != lastHoveredPos)
        {
            if (isHovering && tilemap.HasTile(lastHoveredPos))
            {
                TileBase previousTile = tilemap.GetTile(lastHoveredPos);
                if (previousTile != null && previousTile != roadTile && previousTile != farmlandTile)
                    tilemap.SetColor(lastHoveredPos, Color.white);
            }
            if (tilemap.HasTile(hoveredPos))
            {
                TileBase currentTile = tilemap.GetTile(hoveredPos);
                // Sadece, yol veya zaten farmland olmayan tile'lar highlight edilsin.
                if (currentTile != null && currentTile != roadTile && currentTile != farmlandTile)
                {
                    tilemap.SetTileFlags(hoveredPos, TileFlags.None);
                    tilemap.SetColor(hoveredPos, highlightColor);
                    isHovering = true;
                    lastHoveredPos = hoveredPos;
                }
                else
                {
                    isHovering = false;
                }
            }
            else
            {
                isHovering = false;
            }
        }

        // Fareye tıklandığında, dönüşüm işlemini dene
        if (Input.GetMouseButtonDown(0))
        {
            TileBase tileToConvert = tilemap.GetTile(hoveredPos);
            if (tileToConvert != null)
            {
                // Eğer tile zaten farmland ise dönüşüm yapılmaz
                if (tileToConvert == farmlandTile)
                {
                    Debug.Log("Tile is already farmland. Conversion not needed.");
                    return;
                }
                // Eğer tile eligible ise (yol tile'ı değil)
                if (tileToConvert != roadTile)
                {
                    if (currentGold >= conversionCostPerTile)
                    {
                        currentGold -= conversionCostPerTile;
                        UpdateGoldDisplay();
                        tilemap.SetTile(hoveredPos, farmlandTile);
                        tilemap.SetColor(hoveredPos, Color.white);
                        Debug.Log("Tile converted at " + hoveredPos);
                        conversionModeActive = false; // Sadece tek bir tile dönüşümü için mod kapatılır
                        SaveGold();
                        SaveTilemapState(); // Dönüştürülen tile pozisyonlarını kaydet
                    }
                    else
                    {
                        Debug.Log("Not enough gold!");
                    }
                }
                else
                {
                    Debug.Log("Tile is not eligible for conversion (road tile).");
                }
            }
            else
            {
                Debug.Log("Tile is not eligible for conversion (null).");
            }
        }
    }

    public void ActivateConversionMode()
    {
        conversionModeActive = true;
        if (conversionPanel != null)
            conversionPanel.SetActive(false);
    }

    public void OpenConversionPanel()
    {
        conversionModeActive = false;
        if (conversionPanel != null)
            conversionPanel.SetActive(true);
    }

    public void CloseConversionPanel()
    {
        if (conversionPanel != null)
            conversionPanel.SetActive(false);
    }

    void UpdateGoldDisplay()
    {
        if (goldDisplay != null)
            goldDisplay.text = "Gold: " + currentGold.ToString();
    }

    void SaveGold()
    {
        PlayerPrefs.SetInt(GoldKey, currentGold);
        PlayerPrefs.Save();
    }

    void LoadGold()
    {
        if (PlayerPrefs.HasKey(GoldKey))
        {
            currentGold = PlayerPrefs.GetInt(GoldKey);
        }
    }

    // Kaydedilen tilemap durumunu, sadece farmland tile'larının pozisyonlarını PlayerPrefs'e kaydeder
    void SaveTilemapState()
    {
        StringBuilder sb = new StringBuilder();
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == farmlandTile)
            {
                // Pozisyonu "x,y,z;" şeklinde sakla
                sb.Append(pos.x).Append(",").Append(pos.y).Append(",").Append(pos.z).Append(";");
            }
        }
        PlayerPrefs.SetString(TilemapStateKey, sb.ToString());
        PlayerPrefs.Save();
        Debug.Log("Tilemap state saved: " + sb.ToString());
    }

    // PlayerPrefs'den kaydedilmiş tilemap durumunu yükler ve farmland tile'larını yeniden uygular 
    void LoadTilemapState()
    {
        if (PlayerPrefs.HasKey(TilemapStateKey))
        {
            string data = PlayerPrefs.GetString(TilemapStateKey);
            if (!string.IsNullOrEmpty(data))
            {
                string[] entries = data.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    string[] values = entry.Split(',');
                    if (values.Length == 3)
                    {
                        int x = int.Parse(values[0]);
                        int y = int.Parse(values[1]);
                        int z = int.Parse(values[2]);
                        Vector3Int pos = new Vector3Int(x, y, z);
                        tilemap.SetTile(pos, farmlandTile);
                    }
                }
                Debug.Log("Tilemap state loaded.");
            }
        }
    }
}