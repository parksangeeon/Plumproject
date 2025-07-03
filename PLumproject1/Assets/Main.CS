using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float Speed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Speed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
