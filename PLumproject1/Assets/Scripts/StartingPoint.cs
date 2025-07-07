using UnityEngine;

public class StartingPoint : MonoBehaviour
{
    public string startPoint;
    private ClearSky.Player thePlayer;

    void Start()
    {
        if (thePlayer == null) {
            
            thePlayer = FindAnyObjectByType<ClearSky.Player>();
            }
        
        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = transform.position;
            
        }
    }
}