using UnityEngine;

public class GamepadListSelector : MonoBehaviour
{
    [SerializeField] Transform listTrf;
    [SerializeField] int horizontalColumn = 4;
    [SerializeField] int startIdx = 0;
    [SerializeField] GamepadGuideElement guide;

    ICanvasSlot[] slotList;

    int selectedIdx = -1;

    void Awake()
    {
        slotList = listTrf.GetComponentsInChildren<ICanvasSlot>();
    }

    void OnEnable()
    {
        if (startIdx < 0)
        {
            CancelInvoke();
            Invoke("AutoSelect", 0.1f);           
        }
        else
        {
            SelectSlot(startIdx);
        }
    }

    void OnDisable()
    {
        selectedIdx = -1;
        
        EventManager.GetEvent<GamepadGuideElement>(EGameEvent.OnChangeGamepadFoucs).Invoke(null);
    }

    void AutoSelect()
    {
        selectedIdx = 0;
        
        SelectSlot(0);
    }

    bool SelectSlot(int targetIdx)
    {
        if (slotList[targetIdx].CanSelect())
        {
            selectedIdx = targetIdx;
            
            guide.transform.SetParent(slotList[targetIdx].GetTransform());
            guide.transform.localPosition = Vector3.zero;

            return true;
        }

        return false;
    }

    public void SwapHorizontalSlot(bool isNext)
    {
        selectedIdx = (int)Mathf.Repeat(selectedIdx + (isNext ? 1 : -1), slotList.Length);
        selectedIdx = Mathf.Clamp(selectedIdx, 0, slotList.Length - 1);
        if (SelectSlot(selectedIdx) == false)
        {
            if (isNext)
            {
                for (int i = selectedIdx + 1; i < slotList.Length; i++)
                {
                    if (SelectSlot(i))
                    {
                        return;
                    }
                }
                
                SelectSlot(0);
                return;
            }

            for (int i = selectedIdx - 1; i > 0; i--)
            {
                if (SelectSlot(i))
                {
                    return;
                }
            }
            
            SelectSlot(0);
        }
    }
    
    public void SwapVerticalSlot(bool isUp)
    {
        selectedIdx = (int)Mathf.Repeat(selectedIdx + (isUp ? -horizontalColumn : horizontalColumn), slotList.Length);
        selectedIdx = Mathf.Clamp(selectedIdx, 0, slotList.Length - 1);
        if (SelectSlot(selectedIdx) == false)
        {
            if (isUp)
            {
                for (int i = selectedIdx - 1; i > 0; i--)
                {
                    if (SelectSlot(i))
                    {
                        return;
                    }
                }
            }
            
            for (int i = selectedIdx + 1; i < slotList.Length; i++)
            {
                if (SelectSlot(i))
                {
                    return;
                }
            }
            
            for (int i = selectedIdx - 1; i > 0; i--)
            {
                if (SelectSlot(i))
                {
                    return;
                }
            }
        }
    }

    public void Click()
    {
        if (selectedIdx >= 0)
        {
            slotList[selectedIdx].OnClick();
        }
    }
}
