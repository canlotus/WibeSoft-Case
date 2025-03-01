using UnityEngine;

public class BuildingSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            // Y eksenine göre sorting order belirleme
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}