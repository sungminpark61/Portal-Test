using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌을 감지했습니다.");
    }
}
// 필요하면 if 문을 통해서 조건을 걸어주면 됨