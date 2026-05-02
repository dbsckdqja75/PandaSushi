using UnityEngine;
using TMPro;

public class VersionLabel : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<TMP_Text>().text = string.Format("VERSION {0}", Application.version);
        Destroy(this);
    }
}
