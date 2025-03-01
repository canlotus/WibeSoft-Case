using UnityEngine;
using UnityEngine.UI;

public class ModeToggleManager : MonoBehaviour
{
    public TilemapClickHandler tilemapClickHandler;
    public Button buildingModeButton;
    public Button cropModeButton;

    void Start()
    {
        tilemapClickHandler.buildingModeActive = false;
        cropModeButton.interactable = false;
        buildingModeButton.interactable = true;
    }

    public void EnableBuildingMode()
    {
        tilemapClickHandler.buildingModeActive = true;
        buildingModeButton.interactable = false;
        cropModeButton.interactable = true;
    }

    public void EnableCropMode()
    {
        tilemapClickHandler.buildingModeActive = false;
        cropModeButton.interactable = false;
        buildingModeButton.interactable = true;
    }
}