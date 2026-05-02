using System.Collections.Generic;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] Transform listTrf;
    [SerializeField] List<GameObject> originalNotificationPrefabs = new();

    Dictionary<ENotificationType, GameObject> notificationPrefabs = new();
    
    void Awake()
    {
        notificationPrefabs.Clear();
        
        for(int i = 0; i < originalNotificationPrefabs.Count; i++)
        {
            notificationPrefabs.Add((ENotificationType)i, originalNotificationPrefabs[i]);
        }
    }

    public void ShowNotification(ENotificationType type)
    {
        if (notificationPrefabs.ContainsKey(type))
        {
            ObjectPool.Instance.ClearPool(notificationPrefabs[type]);
            ObjectPool.Instance.Spawn(notificationPrefabs[type], listTrf);
        }
    }
}
