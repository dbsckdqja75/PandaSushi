using UnityEngine;

public class CameraView : MonoBehaviour
{
    [Header("Camera Info")]
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Vector3 offsetRot;
    [SerializeField] float offsetFOV;
    float originFOV;

    [Header("Camera Follow")]
    [SerializeField] bool isLerp = true;
    [SerializeField] Transform targetTrf;
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float perspectiveClipFar = 300;
    [SerializeField] float perspectiveClampX = 1.5f;

    bool isClampView = true;
    
    Camera mainCamera;
    Vector3 targetPos, resultPos, extraPos;

    void Awake()
    {
        Application.targetFrameRate = 60;

        mainCamera = Camera.main;
        originFOV = mainCamera.fieldOfView;
    }

    void FixedUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
        UpdateCameraFOV();
    }

    void UpdateCameraPosition()
    {
        if (targetTrf != null)
        {
            targetPos.Set(targetTrf.position);
        }

        extraPos.z = 0;

        resultPos.Set(targetPos + offsetPos + extraPos);

        if(isClampView && Mathf.Abs(resultPos.x) > perspectiveClampX)
        {
            resultPos.x = (resultPos.x > 0) ? perspectiveClampX : -perspectiveClampX;
        }

        if (isLerp)
        {
            transform.position = Vector3.Lerp(transform.position, resultPos, followSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.SetPosition(resultPos);
        }
    }

    void UpdateCameraRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(offsetRot), followSpeed * Time.fixedDeltaTime);
    }

    void UpdateCameraFOV()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, (originFOV + offsetFOV), followSpeed * Time.fixedDeltaTime);
    }

    public void UpdateTarget(Transform newTarget)
    {
        targetTrf = newTarget;
    }

    public void UpdateOffset(Vector3 newPos, Vector3 newRot, float newFOV, bool isClampPos = true)
    {
        isClampView = isClampPos;
        
        offsetPos = newPos;
        offsetRot = newRot;
        offsetFOV = newFOV;
    }
}
