using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GlobalTypes;


public class Wepon : MonoBehaviour
{

    public BulletController shot;
    public Transform shotPoint;
    PlayerController playerController;
    public float chargeSpeed;
    public float chargeTime;
    public float chargeLimit = 2;
    public bool holdBotton;
    public bool CanChergeShoot;


    // Start is called before the first frame update
    private void Update()
    {
      
      
    }

    private void Start()
    {
        
    }


    void chargeShoot()
    {
        if (holdBotton && chargeTime < chargeLimit && CanChergeShoot)
        {
            chargeTime += Time.deltaTime * chargeSpeed;
        }
        else if (!holdBotton && chargeTime >= chargeLimit)
        {

            GameObject bullet = ObjectPool.instance.GetChargeShootPool();
            if (bullet != null)
            {
                bullet.GetComponent<BulletController>().profile.moveDir = new Vector2(playerController.Direction, 0f);
                bullet.transform.position = shotPoint.position;
                bullet.transform.rotation = shotPoint.rotation;
                bullet.SetActive(true);
            }


        }
        if (!holdBotton)
            chargeTime = 0;
    }
}
  
