using UnityEngine;
using TMPro;

public class ReviewLabel : MonoBehaviour
{
    [SerializeField] TMP_Text labelText;
    [SerializeField] GameObject[] starIcons;

    public void UpdateLabel(string context, int starCount)
    {
        labelText.text = context;

        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].SetActive(i < starCount);
        }
    }
}
