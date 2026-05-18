using UnityEngine;

public interface ICanvasSlot
{
    public void OnClick();
    public bool CanSelect();
    public Transform GetTransform();
}