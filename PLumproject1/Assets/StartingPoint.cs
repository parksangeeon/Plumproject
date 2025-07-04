using UnityEngine;

public class StartingPoint : MonoBehaviour
{
    public string startPoint;
    private ClearSky.Player thePlayer;

    void Start()
    {
        if (thePlayer == null) {
            Debug.LogError("Player not found in StartingPoint!");
            thePlayer = FindAnyObjectByType<ClearSky.Player>();
            }
        Debug.Log($"Player currentMapName: {thePlayer.currentMapName}, this startPoint: {startPoint}");
        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = transform.position;
            Debug.Log("Player position set by StartingPoint.");
        }
    }
}