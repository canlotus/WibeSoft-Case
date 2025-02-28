using UnityEngine;
using UnityEngine.Tilemaps;

public class EkinSecimManager : MonoBehaviour
{
    public GameObject ekinSecimPaneli; // UI paneli
    public Tilemap tilemap;            // Çalıştığınız tilemap
    private Vector3Int aktifTilePos;   // Seçilen tile pozisyonu

    public GameObject bugdayPrefab;    // Buğday prefab
    public GameObject misirPrefab;     // Mısır prefab

    void Start()
    {
        ekinSecimPaneli.SetActive(false);
    }

    public void EkinSecimiAc(Vector3Int tilePos)
    {
        aktifTilePos = tilePos;
        ekinSecimPaneli.SetActive(true);
    }

    public void BugdayEkle()
    {
        Vector3 ekinPozisyonu = tilemap.GetCellCenterWorld(aktifTilePos) + new Vector3(0, 0.3f, 0);
        Instantiate(bugdayPrefab, ekinPozisyonu, Quaternion.identity);
        ekinSecimPaneli.SetActive(false);
    }

    public void MisirEkle()
    {
        Vector3 ekinPozisyonu = tilemap.GetCellCenterWorld(aktifTilePos) + new Vector3(0, 0.3f, 0);
        Instantiate(misirPrefab, ekinPozisyonu, Quaternion.identity);
        ekinSecimPaneli.SetActive(false);
    }

    public void PaneliKapat()
    {
        ekinSecimPaneli.SetActive(false);
    }
}