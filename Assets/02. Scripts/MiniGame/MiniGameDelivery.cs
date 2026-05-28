using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniGameDelivery : MiniGame
{
    bool isPlayingMotion = false;
    
    [SerializeField] RectTransform riderTrf;
    [SerializeField] RectTransform goalTrf;
    [SerializeField] RectTransform clearFxTrf;

    [SerializeField] GameObject guide;
    [SerializeField] GameObject[] guideArrows;
    [SerializeField] MiniGameWayPoint[] endPoints;

    MiniGameWayPoint curPoint, startPoint, goalPoint;

    public override void Init(Action<bool> finishCallback)
    {
        base.Init(finishCallback);
        
        var shuffledPointList = endPoints.OrderBy(x => Guid.NewGuid()).ToList();
        
        startPoint = shuffledPointList[0];
        goalPoint = shuffledPointList[1];
        curPoint = startPoint;

        riderTrf.position = startPoint.GetPosition();
        goalTrf.position = goalPoint.GetPosition();
        clearFxTrf.gameObject.SetActive(false);
        
        isPlayingMotion = false;
        UpdateGuide();
    }

    void OnEnable()
    {
        playerActions = new CustomPlayerActions();
        playerActions.Player.MenuMove.performed += InputDirection;
        playerActions.Player.PadPointer.performed += InputDirection;
        
        playerActions.Player.MenuNext.started += (i) => MoveRider(3);
        playerActions.Player.MenuPrevious.started += (i) => MoveRider(2);
        playerActions.Player.MenuUp.started += (i) => MoveRider(0);
        playerActions.Player.MenuDown.started += (i) => MoveRider(1);
        playerActions.Player.Enable();
    }
    
    void OnDisable()
    {
        playerActions.Player.Disable();
    }

    protected override void FinishGame(bool isSuccess)
    {
        if (isSuccess)
        {
            StageManager.Instance.StartBuff(EBuffType.FastRider);
        }
        
        base.FinishGame(isSuccess);
    }

    protected override void Update()
    {
        base.Update();
        
        if(Input.GetButton("Horizontal"))
        {
            int h = (int)Input.GetAxisRaw("Horizontal");
            if (h != 0)
            {
                MoveRider(h == -1 ? 2 : 3); // 좌우
            }
        }
        
        if(Input.GetButton("Vertical"))
        {
            int v = (int)Input.GetAxisRaw("Vertical");
            if (v != 0)
            {
                MoveRider(v == 1 ? 0 : 1); // 상하
            }
        }
    }

    void InputDirection(InputAction.CallbackContext inputContext)
    {
        Vector2 inputValue = inputContext.ReadValue<Vector2>();
        inputValue.x = Mathf.RoundToInt(inputValue.x);
        inputValue.y = Mathf.RoundToInt(inputValue.y);

        int direction = 0;
        if (inputValue.x != 0)
        {
            direction = (inputValue.x < 0) ? 2 : 3;
        }
        
        if (inputValue.y != 0)
        {
            direction = (inputValue.y < 0) ? 1 : 0;
        }

        MoveRider(direction);
    }

    void MoveRider(int direction)
    {
        if (isPlayingMotion)
        {
            return;
        }

        MiniGameWayPoint nextPoint = curPoint.GetNextPoint(direction);
        if (nextPoint != null)
        {
            isPlayingMotion = true;

            UpdateGuide();
            
            this.StopAllCoroutines();
            MoveMotion(nextPoint).Start(this);
        }
    }

    void UpdateGuide()
    {
        if (isPlayingMotion)
        {
            guide.SetActive(false);
            return;
        }

        for (int i = 0; i < guideArrows.Length; i++)
        {
            guideArrows[i].SetActive(curPoint.HasDirectionPoint(i));
        }
        
        guide.SetActive(true);
    }

    IEnumerator MoveMotion(MiniGameWayPoint nextPoint)
    {
        Vector3 startPos = riderTrf.position;
        Vector3 endPos = nextPoint.GetPosition();
        
        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            riderTrf.position = Vector3.Lerp(startPos, endPos, t);
            
            elapsed += Time.smoothDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        riderTrf.position = endPos;

        curPoint = nextPoint;
        
        if(curPoint == goalPoint)
        {
            clearFxTrf.transform.position = goalPoint.GetPosition();
            clearFxTrf.gameObject.SetActive(true);
            
            OnInputted();
            
            SoundManager.Instance.PlaySound("SFX_Notice1");
            
            yield return new WaitForSeconds(0.75f);
            FinishGame(true);
            yield break;
        }
        
        isPlayingMotion = false;
        
        UpdateGuide();
        
        yield break;
    }
}
