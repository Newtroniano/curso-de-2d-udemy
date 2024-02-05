using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthManager : MonoBehaviour
{
    public UnityEngine.UI.Image healthBar;
    public float healthAmount = 100f;
    public static BossHealthManager Instance;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {



        //logic to take damge here 
        if (Input.GetKeyUp(KeyCode.Space))
        {

            TakeDamage(20);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            Heal(5);
        }


    }



    public void TakeDamage(float damage)
    {

        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;

    }


    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
     
    }

}
