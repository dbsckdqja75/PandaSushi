using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] Transform throwPoint;
    [SerializeField] GameObject cookToolBulletPrefab;

    public void ShootUtensil(Vector3 targetPoint)
    {
        targetPoint.y = throwPoint.position.y;
        
        // NOTE: 던진 도구는 생성하자마자 정면으로 직선 이동
        GameObject bullet = ObjectPool.Instance.Spawn(cookToolBulletPrefab, throwPoint.position, Quaternion.LookRotation((targetPoint - throwPoint.position).normalized));
        // bullet.transform.LookAt(bullet.transform.position + (targetPoint - throwPoint.position).normalized);
        // bullet.transform.rotation = Quaternion.LookRotation((targetPoint - throwPoint.position).normalized);
    }
}
