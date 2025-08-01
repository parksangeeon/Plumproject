using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [HideInInspector] public float speed;

    private Rigidbody2D _myBody;

    private void Awake()
    {
        _myBody = GetComponent<Rigidbody2D>();
        speed = -2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _myBody.linearVelocity = new Vector2(speed, _myBody.linearVelocity.y);
    }
}
