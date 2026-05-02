using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] Vector3 rotateAxis = Vector3.forward;
    [SerializeField] float rotateSpeed = 90;

    void Update()
    {
        transform.Rotate(rotateAxis * (rotateSpeed * Time.smoothDeltaTime));       
    }
}
