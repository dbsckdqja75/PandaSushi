using UnityEngine;

public class UtensilBullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 40f;
    [SerializeField] float despawnTime = 2f;
 
    [Space(10)]
    [SerializeField] GameObject[] models;
    [SerializeField] GameObject fx;
    
    Collider col;
    Rigidbody rb;

    void Awake()
    {
        col = this.GetComponent<Collider>();
        rb = this.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        col.enabled = true;
        
        foreach (GameObject model in models)
        {
            model.SetActive(false);
        }
        
        models[Random.Range(0, models.Length)].SetActive(true);
        
        CancelInvoke();
        Invoke("Despawn", despawnTime);
    }
    
    void Update()
    {
        rb.MovePosition(rb.position + (transform.forward * (bulletSpeed * Time.smoothDeltaTime)));
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Guest"))
        {
            Debug.Log("[조리도구 던지기] 손님 맞음");

            Despawn();
        }

        if (col.gameObject.CompareTag("Wall"))
        {
            Debug.Log("[조리도구 던지기] 벽 맞음");
            
            Despawn();
        }
        
        if (col.gameObject.CompareTag("Event"))
        {
            Debug.Log("[조리도구 던지기] 이벤트 개체 맞음");
            
            Despawn();
        }
    }

    void Despawn()
    {
        CancelInvoke();
        
        col.enabled = false;
        this.gameObject.SetActive(false);
    }
}
