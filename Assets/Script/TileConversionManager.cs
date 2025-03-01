using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.UI;

public enum ConversionType
{
    Farmland,
    House,
    Factory
}

public class TileConversionManager : MonoBehaviour
{
    [Header("Tilemap & Tile Settings")]
    public Tilemap tilemap;
    public TileBase farmlandTile;
    public TileBase roadTile;
    public TileBase houseTile;
    public TileBase factoryTile;
    public TileBase grassTile;

    [Header("Conversion Costs & Gold")]
    public int conversionCostPerTile = 50;
    public int houseCost = 100;
    public int factoryCost = 150;
    public int currentGold = 1000;
    public TextMeshProUGUI goldDisplay;

    [Header("UI Panel")]
    public GameObject conversionPanel;
    public GameObject moveBuildingButton;

    [Header("Highlight Settings")]
    public Color highlightColor = new Color(1f, 1f, 0f, 0.5f);
    private Vector3Int lastHoveredCell;
    private bool isHovering = false;

    [Header("Conversion Mode")]
    public bool conversionModeActive = false;
    public ConversionType conversionType = ConversionType.Farmland;

    [Header("Building Move Settings")]
    private bool isMovingBuilding = false;
    private Vector3Int buildingOriginalPos;
    private TileBase buildingTileToMove;

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
        if (moveBuildingButton != null)
        {
            Button btn = moveBuildingButton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(BeginMoveBuilding);
            moveBuildingButton.SetActive(false);
        }
    }

    void Update()
    {
        if (isMovingBuilding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
                Vector3Int targetTilePos = tilemap.WorldToCell(mouseWorldPos);
                TileBase targetTile = tilemap.GetTile(targetTilePos);
                if (targetTile != null && targetTile != roadTile && IsTileEmptyForBuilding(targetTilePos))
                {
                    tilemap.SetTile(buildingOriginalPos, grassTile);
                    tilemap.SetTile(targetTilePos, buildingTileToMove);
                    PlayerPrefs.DeleteKey($"{buildingOriginalPos.x}_{buildingOriginalPos.y}");
                    if (buildingTileToMove == houseTile)
                        PlayerPrefs.SetString($"{targetTilePos.x}_{targetTilePos.y}", "House");
                    else if (buildingTileToMove == factoryTile)
                        PlayerPrefs.SetString($"{targetTilePos.x}_{targetTilePos.y}", "Factory");
                    PlayerPrefs.Save();
                    isMovingBuilding = false;
                }
            }
            return;
        }

        if (!conversionModeActive)
        {
            if (isHovering && tilemap.HasTile(lastHoveredCell))
            {
                TileBase tile = tilemap.GetTile(lastHoveredCell);
                if (tile != null && tile != roadTile)
                    tilemap.SetColor(lastHoveredCell, Color.white);
                isHovering = false;
            }
            return;
        }

        Vector3 mouseWorldPosConv = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosConv.z = 0;
        Vector3Int hoveredCell = tilemap.WorldToCell(mouseWorldPosConv);

        if (!isHovering || hoveredCell != lastHoveredCell)
        {
            if (isHovering && tilemap.HasTile(lastHoveredCell))
            {
                TileBase prevTile = tilemap.GetTile(lastHoveredCell);
                if (prevTile != null && prevTile != roadTile)
                    tilemap.SetColor(lastHoveredCell, Color.white);
            }
            if (tilemap.HasTile(hoveredCell))
            {
                TileBase currentTile = tilemap.GetTile(hoveredCell);
                if (currentTile != null && currentTile != roadTile)
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
                if (tileToConvert == roadTile)
                    return;
                int cost = 0;
                TileBase newTile = null;
                string tileType = "";
                switch (conversionType)
                {
                    case ConversionType.Farmland:
                        if (tileToConvert == farmlandTile)
                            return;
                        cost = conversionCostPerTile;
                        newTile = farmlandTile;
                        tileType = "Farmland";
                        break;
                    case ConversionType.House:
                        if (tileToConvert == houseTile)
                            return;
                        cost = houseCost;
                        newTile = houseTile;
                        tileType = "House";
                        break;
                    case ConversionType.Factory:
                        if (tileToConvert == factoryTile)
                            return;
                        cost = factoryCost;
                        newTile = factoryTile;
                        tileType = "Factory";
                        break;
                }
                if (currentGold >= cost)
                {
                    currentGold -= cost;
                    UpdateGoldDisplay();
                    tilemap.SetTile(hoveredCell, newTile);
                    tilemap.SetColor(hoveredCell, Color.white);
                    PlayerPrefs.SetString($"{hoveredCell.x}_{hoveredCell.y}", tileType);
                    PlayerPrefs.Save();
                    conversionModeActive = false;
                    SaveGold();
                }
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

    public void SetConversionToFarmland()
    {
        conversionType = ConversionType.Farmland;
    }

    public void SetConversionToHouse()
    {
        conversionType = ConversionType.House;
    }

    public void SetConversionToFactory()
    {
        conversionType = ConversionType.Factory;
    }

    public void PrepareBuildingMove(Vector3Int buildingPos, TileBase buildingTile)
    {
        buildingOriginalPos = buildingPos;
        buildingTileToMove = buildingTile;
        if (moveBuildingButton != null)
            moveBuildingButton.SetActive(true);
    }

    public void BeginMoveBuilding()
    {
        isMovingBuilding = true;
        if (moveBuildingButton != null)
            moveBuildingButton.SetActive(false);
    }

    bool IsTileEmptyForBuilding(Vector3Int pos)
    {
        TileBase tileAtPos = tilemap.GetTile(pos);
        if (tileAtPos == grassTile)
            return true;
        string tileKey = $"{pos.x}_{pos.y}_IsEmpty";
        bool emptyFromPrefs = PlayerPrefs.HasKey(tileKey) && PlayerPrefs.GetInt(tileKey) == 1;
        return emptyFromPrefs;
    }

    void UpdateGoldDisplay()
    {
        if (goldDisplay != null)
            goldDisplay.text = "Money " + currentGold.ToString();
    }

    void SaveGold()
    {
        PlayerPrefs.SetInt(GoldKey, currentGold);
        PlayerPrefs.Save();
    }

    void LoadGold()
    {
        if (PlayerPrefs.HasKey(GoldKey))
            currentGold = PlayerPrefs.GetInt(GoldKey);
    }

    void LoadTilemapState()
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (PlayerPrefs.HasKey($"{pos.x}_{pos.y}"))
            {
                string tileType = PlayerPrefs.GetString($"{pos.x}_{pos.y}");
                if (tileType == "Farmland")
                    tilemap.SetTile(pos, farmlandTile);
                else if (tileType == "House")
                    tilemap.SetTile(pos, houseTile);
                else if (tileType == "Factory")
                    tilemap.SetTile(pos, factoryTile);
            }
        }
    }
}