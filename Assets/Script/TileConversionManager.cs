using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Text;

public class TileConversionManager : MonoBehaviour
{
    [Header("Tilemap & Tile Settings")]
    public Tilemap tilemap;             
    public TileBase farmlandTile;       
    public TileBase roadTile;          

    [Header("Conversion Settings")]
    public int conversionCostPerTile = 50;   
    public int currentGold = 1000;          
    public TextMeshProUGUI goldDisplay;     

    [Header("UI Panel")]
    public GameObject conversionPanel;      

    [Header("Highlight Settings")]
    public Color highlightColor = new Color(1f, 1f, 0f, 0.5f); 
    private Vector3Int lastHoveredCell;
    private bool isHovering = false;

    [Header("Conversion Mode")]
    public bool conversionModeActive = false; 

    private const string GoldKey = "Gold";
    private const string TilemapStateKey = "TilemapState";

    void Start()
    {
        tilemap.CompressBounds();
        LoadGold();
        UpdateGoldDisplay();
        LoadTilemapState(); 
        if (conversionPanel != null)
            conversionPanel.SetActive(false);
    }

    void Update()
    {
        if (!conversionModeActive)
        {
            if (isHovering && tilemap.HasTile(lastHoveredCell))
            {
                TileBase tile = tilemap.GetTile(lastHoveredCell);
                if (tile != null && tile != roadTile && tile != farmlandTile)
                    tilemap.SetColor(lastHoveredCell, Color.white);
                isHovering = false;
            }
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int hoveredCell = tilemap.WorldToCell(mouseWorldPos);

        if (!isHovering || hoveredCell != lastHoveredCell)
        {
            if (isHovering && tilemap.HasTile(lastHoveredCell))
            {
                TileBase previousTile = tilemap.GetTile(lastHoveredCell);
                if (previousTile != null && previousTile != roadTile && previousTile != farmlandTile)
                    tilemap.SetColor(lastHoveredCell, Color.white);
            }
            if (tilemap.HasTile(hoveredCell))
            {
                TileBase currentTile = tilemap.GetTile(hoveredCell);
                if (currentTile != null && currentTile != roadTile && currentTile != farmlandTile)
                {
                    tilemap.SetTileFlags(hoveredCell, TileFlags.None);
                    tilemap.SetColor(hoveredCell, highlightColor);
                    isHovering = true;
                    lastHoveredCell = hoveredCell;
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

        if (Input.GetMouseButtonDown(0))
        {
            TileBase tileToConvert = tilemap.GetTile(hoveredCell);
            if (tileToConvert != null)
            {
                if (tileToConvert == farmlandTile)
                {
                    Debug.Log("Tile is already farmland. Conversion not needed.");
                    return;
                }
                if (tileToConvert != roadTile)
                {
                    if (currentGold >= conversionCostPerTile)
                    {
                        currentGold -= conversionCostPerTile;
                        UpdateGoldDisplay();
                        tilemap.SetTile(hoveredCell, farmlandTile);
                        tilemap.SetColor(hoveredCell, Color.white);
                        Debug.Log("Tile converted at " + hoveredCell);
                        conversionModeActive = false; 
                        SaveGold();
                        SaveTilemapState();
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

    void SaveTilemapState()
    {
        StringBuilder sb = new StringBuilder();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == farmlandTile)
            {
                sb.Append(pos.x).Append(",").Append(pos.y).Append(",").Append(pos.z).Append(";");
            }
        }
        PlayerPrefs.SetString(TilemapStateKey, sb.ToString());
        PlayerPrefs.Save();
        Debug.Log("Tilemap state saved: " + sb.ToString());
    }

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