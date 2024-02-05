using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxC : MonoBehaviour
{
    private HealthManager _healthManager;

    // Start is called before the first frame update
    void Start()
    {
        _healthManager = gameObject.GetComponent<HealthManager>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        HealthManager.Instance.HealCorse();
        BossHealthManager.Instance.TakeDamage(2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
