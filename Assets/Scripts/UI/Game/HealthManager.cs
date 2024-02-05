using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public UnityEngine.UI.Image healthBar;
    public UnityEngine.UI.Image greyHealthBar;
    public float healthAmount = 100f;
    public float greyHealthAmount = 100f;
    public bool isGreyDamage = false;
    public static HealthManager Instance;
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
        if (Input.GetKeyUp(KeyCode.M))
        {
            HealCorse();
        }



        if (Input.GetKeyUp(KeyCode.N))
        {
            TakeCorsedDamage(20);
        }

    


    }

    public void TakeCorsedDamage(float damage)
    {
       


        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
        isGreyDamage = true;


    }

    public void TakeDamage(float damage)
    {

        if (!isGreyDamage)
        {
            healthAmount -= damage;
          
            healthBar.fillAmount = healthAmount / 100f;
            greyHealthBar.fillAmount = healthAmount / 100f;
            greyHealthAmount = healthAmount;
         
        }

        if(isGreyDamage)
        {
            greyHealthBar.fillAmount = healthBar.fillAmount;
            greyHealthAmount = healthAmount;
            isGreyDamage = false;
        }

    }

    public void HealCorse()
    {
      
        if (isGreyDamage)
        {
            healthBar.fillAmount = greyHealthBar.fillAmount;
            healthAmount = greyHealthAmount;

            isGreyDamage = false;
        }
        //greyHealthBar.fillAmount = healthAmount / 100f;

    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        healthBar.fillAmount = healthAmount / 100f;
        if (healthBar.fillAmount > greyHealthBar.fillAmount)
        {
            greyHealthAmount += healingAmount;
            greyHealthAmount = Mathf.Clamp(greyHealthAmount, 0, 100);
            greyHealthBar.fillAmount = healthAmount / 100f;
        }
    }


}
