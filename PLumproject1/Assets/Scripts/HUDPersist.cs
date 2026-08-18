using UnityEngine;

public class HUDPersist : MonoBehaviour
{
    private static HUDPersist instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
