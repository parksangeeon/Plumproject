using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuzzleStation : MonoBehaviour
{
    public RhythmPuzzleManager puzzle; // 퍼즐 패널 오브젝트를 드래그해 할당
    public string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && Input.GetKeyDown(KeyCode.Z))
        {
            puzzle?.StartPuzzle();
        }
    }
}
