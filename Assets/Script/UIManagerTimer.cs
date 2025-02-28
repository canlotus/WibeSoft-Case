using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Global crop growth panel reference
    public GameObject globalGrowthUIPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Eğer sahneler arası taşınması gerekiyorsa
        }
        else
        {
            Destroy(gameObject);
        }
    }
}