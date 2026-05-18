using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float h, v;
    float lerpX, lerpZ;
    float latestH, latestV;

    Vector3 inputPos, lookAtPoint;

    [Space(10)] [SerializeField] float lerpMove = 5;

    [Space(10)] [SerializeField] float lerpSpeed = 8;
    [SerializeField] float minMoveSpeed = 4, maxMoveSpeed = 6;
    [SerializeField] GlobalState globalState;

    [Space(10)] [SerializeField] LayerMask rayTargetLayer;
    [SerializeField] LayerMask forwardRayTargetLayer;
    [SerializeField] LayerMask floorRayTargetLayer;

    public static GameObject rayHitObj = null;
    GameObject prevHitObj, wallObj;

    InteractionObject hitInteractionObj = null;

    Player player;
    PlayerMotion playerMotion;

    CustomPlayerActions playerActions;
    CapsuleCollider col;
    Rigidbody rb;

    void Awake()
    {
        player = this.GetComponent<Player>();
        playerMotion = this.GetComponent<PlayerMotion>();
        col = this.GetComponent<CapsuleCollider>();
        rb = this.GetComponent<Rigidbody>();

        rayHitObj = null;
    }

    void OnEnable()
    {
        playerActions = new CustomPlayerActions();
        playerActions.Player.MousePointer.performed += OnInputPosition;
        playerActions.Player.PadPointer.performed += OnInputPosition;
        playerActions.Player.AimPointer.performed += OnAimPosition;
        
        playerActions.Player.PointerSelect.started += (i) => OnInputSelect();
        playerActions.Player.Interaction.performed += (i) => OnInputKeyInteraction();
        playerActions.Player.Eat.started += (i) => OnInputEat();

        playerActions.Player.Aim.started += (i) => OnInputAim(true);
        playerActions.Player.Aim.canceled += (i) => OnInputAim(false);

        playerActions.Player.Shoot.started += (i) => OnInputThrow();

        playerActions.Player.Enable();
    }

    void OnDisable()
    {
        playerActions.Player.Disable();
    }

    void Update()
    {
        UpdateHover();
    }

    void FixedUpdate()
    {
        if (player.CanMove() == false)
        {
            return;
        }

        h = -Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        v = -Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (player.IsAiming())
        {
            h = v = 0;
        }

        if (h != 0 || v != 0)
        {
            latestH = h;
            latestV = v;
            
            UpdateMovePosition();

            playerMotion.OnWalk();
        }
        else
        {
            lerpMove = minMoveSpeed;

            playerMotion.OnIdle();
        }

        rb.linearVelocity = Vector3.zero;

        UpdateLookAtPosition();
    }

    void OnInputPosition(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>();
        if (InputDetector.Instance.IsInputGamepad() == false)
        {
            inputPos = inputValue;
        }
        else if(player.IsAiming() == false)
        {
            inputPos = Camera.main.WorldToScreenPoint(transform.position + transform.forward);
        }
    }

    void OnAimPosition(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>().normalized;
        if (player.IsAiming() && inputValue.magnitude != 0)
        {
            latestH = inputValue.x;
            latestV = inputValue.y;

            Vector3 stickValue = new Vector3(inputValue.x, 0, inputValue.y) * 10f;
            inputPos = Camera.main.WorldToScreenPoint(transform.position + stickValue);
        }
    }

    void OnInputSelect()
    {
        if(player.CanMove() == false || player.IsAiming())
        {
            return;
        }

        if (InputDetector.Instance.IsInputGamepad() == false)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(inputPos);
        InteractionObject iObject = null;
        
        rayHitObj = Physics.Raycast(ray, out hit, Mathf.Infinity, rayTargetLayer) ? hit.collider.gameObject : null;
        if (rayHitObj != null && rayHitObj.TryGetComponent(out iObject))
        {
            iObject.OnSelect(player);

            prevHitObj = null;
            ForceClearHoverObject();
        }
    }

    public void UpdateHover()
    {
        if (InputDetector.Instance.IsInputGamepad() == false && EventSystem.current.IsPointerOverGameObject())
        {
            ForceClearHoverObject();
            return;
        }

        if (player.CanMove() == false || player.IsAiming())
        {
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(inputPos);

        rayHitObj = Physics.Raycast(ray, out hit, Mathf.Infinity, rayTargetLayer) ? hit.collider.gameObject : null;
        if(rayHitObj != prevHitObj)
        {
            if (hitInteractionObj != null)
            {
                hitInteractionObj.OnMouseOut();
                hitInteractionObj = null;
                
                EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(null);
                EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Invoke(null);
            }
            
            prevHitObj = rayHitObj;
            if (rayHitObj != null)
            {
                if (rayHitObj.TryGetComponent(out InteractionObject iObject))
                {
                    hitInteractionObj = iObject;
                    iObject.OnMouseHover();
                    
                    EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Invoke(hitInteractionObj.GetHoverFoucsPivot());
                }
            }
            else
            {
                EventManager.GetEvent<HoverPlateInfo>(EGameEvent.OnHoverPlate).Invoke(null);
            }
        }

        if (hitInteractionObj != null)
        {
            EventManager.GetEvent<bool>(EGameEvent.OnCloserHoverObject).Invoke(hitInteractionObj.CanSelect(this.transform));
        }
    }

    public void OnInputAim(bool onStart)
    {
        if(player.CanMove() == false || player.IsHolding())
        {
            return;
        }

        if (onStart)
        {
            if (player.IsHolding() == false && player.IsAiming() == false)
            {
                player.StartAim();
            }

            return;
        }

        player.FinishAim(Vector3.zero);
    }

    public void OnInputThrow()
    {
        if(player.CanMove() == false)
        {
            return;
        }

        if (player.IsAiming())
        {
            player.FinishAim(lookAtPoint);
        }
    }

    public void OnInputEat()
    {
        if(player.CanMove() == false)
        {
            return;
        }

        if (player.IsHolding() && player.CanEat())
        {
            player.ManuallyEat();
        }
    }

    public void OnInputKeyInteraction()
    {
        if(player.CanMove() == false)
        {
            return;
        }

        if(hitInteractionObj != null)
        {
            if(hitInteractionObj.TryGetComponent(out PrepTable table) && player.IsIdle())
            {
                table.TryPrep();

                prevHitObj = null;
            }
        }
    }

    void UpdateMovePosition()
    {
        lerpMove = Mathf.Lerp(lerpMove, maxMoveSpeed, minMoveSpeed * Time.fixedDeltaTime);

        Vector3 moveToPoint = Vector3.zero;
        if(IsMoveable())
        {
            moveToPoint = new Vector3(h, 0, v);
        }

        rb.MovePosition(transform.position + moveToPoint * (lerpMove * globalState.playerSpeedMultiple * Time.fixedDeltaTime));
    }

    void UpdateLookAtPosition()
    {
        if (latestH != 0 || latestV != 0)
        {
            lerpX = Mathf.Lerp(lerpX, latestH * 90, lerpSpeed * Time.fixedDeltaTime);
            lerpZ = Mathf.Lerp(lerpZ, latestV * 90, lerpSpeed * Time.fixedDeltaTime);
            
            lookAtPoint = new Vector3(lerpX, transform.position.y, lerpZ);
        }

        if (player.IsAiming())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            Vector3 hitPos = Physics.Raycast(ray, out hit, Mathf.Infinity, floorRayTargetLayer) ? hit.point : Vector3.zero;

            if (hitPos != Vector3.zero)
            {
                hitPos.y = transform.position.y;
                lookAtPoint = hitPos;
                latestH = latestV = 0;
            }
        }

        Vector3 directionToTarget = (lookAtPoint - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpSpeed * 2 * Time.fixedDeltaTime);
    }

    public void ForceClearHoverObject()
    {
        if (hitInteractionObj)
        {
            hitInteractionObj.OnMouseOut();
            
            rayHitObj = null;
            hitInteractionObj = null;
            
            EventManager.GetEvent<Transform>(EGameEvent.OnHoverFocus).Invoke(null);
        }
    }

    public void ForceUpdateTransform(Vector3 pos, Vector3 rot)
    {
        rb.MovePosition(pos);
        rb.MoveRotation(Quaternion.Euler(rot));
    }

    bool IsMoveable()
    {
        float currentX = Mathf.Abs(transform.position.x);
        float currentZ = Mathf.Abs(transform.position.z);

        if(currentX > 8 || currentZ < 15 || currentZ > 20)
        {
            transform.position = new Vector3(-0.5f, 0, -17.5f);
        }

        RaycastHit hit;
        wallObj = Physics.Raycast(transform.position + col.center, transform.forward, out hit, col.radius * 1.5f, forwardRayTargetLayer) ? hit.collider.gameObject : null;
        if(wallObj != null)
        {
            return false;
        }

        return true;
    }

    
    
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle labelStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 16;
        labelStyle.normal.textColor = Color.green;

        Gizmos.color = Color.blue; // NOTE: FORWARD
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position + Vector3.forward, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.forward, "FORWARD", labelStyle);

        Gizmos.color = Color.red; // NOTE: BACK
        Gizmos.DrawLine(transform.position, transform.position + Vector3.back);
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position + Vector3.back, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.back, "BACK", labelStyle);

        Gizmos.color = Color.green; // NOTE: LEFT
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left);
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position + Vector3.left, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.left, "LEFT", labelStyle);

        Gizmos.color = Color.yellow; // NOTE: RIGHT
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position + Vector3.right, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.right, "RIGHT", labelStyle);

        Gizmos.color = Color.cyan; // NOTE: CURRENT DIR
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(latestH, 0, latestV));
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position + new Vector3(latestH, 0, latestV), 0.2f);
        labelStyle.normal.textColor = Color.red;
        UnityEditor.Handles.Label(transform.position  + new Vector3(latestH, 0.5f, latestV), "CUR", labelStyle);

        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(inputPos);;

        Gizmos.color = Color.gray; // NOTE: MOUSE POINT
        Gizmos.DrawLine(transform.position, mousePoint);
        labelStyle.normal.textColor = Color.red;
        UnityEditor.Handles.Label(mousePoint, "MOUSE", labelStyle);

        if(rayHitObj != null)
        {
            Gizmos.color = Color.red; // NOTE: MOUSE POINT
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.8f);
            Gizmos.DrawCube(rayHitObj.transform.position, Vector3.one);
        }

        if(col != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + col.center, transform.position + col.center + (transform.forward * col.radius));
        }

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.2f, 0.9f, 0.9f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1, 0.01f, 1));
        Gizmos.DrawSphere(Vector3.zero, 0.1f);
        Gizmos.matrix = oldMatrix;
    }
    #endif
}
