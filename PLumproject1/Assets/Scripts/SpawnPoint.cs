using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SceneEntrance entranceId;

    void Start()
    {
        var player = FindAnyObjectByType<ClearSky.Player>();
        if (player != null && player.pendingEntrance == entranceId)
        {
            player.transform.position = transform.position;
            player.pendingEntrance = SceneEntrance.None;
        }
    }
}
