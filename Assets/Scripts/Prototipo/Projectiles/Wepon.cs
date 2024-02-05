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
        chargeShoot();
      
    }

    private void Start()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
    }
    public void OnShot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            holdBotton = true;

        
            //Instantiate(shot, shotPoint.position, shotPoint.rotation).moveDir = new Vector2(playerController.Direction, 0f);
            GameObject bullet = ObjectPool.instance.GetNormalShootPool();
            if(bullet != null)
            {
                //bullet.GetComponent<BulletController>().profile.moveDir = new Vector2(playerController.Direction, 0f);
              
                bullet.transform.position = shotPoint.position;
                bullet.transform.rotation = shotPoint.rotation;
            
                bullet.SetActive(true);
            }
        }
        else if (context.canceled)
        {
            holdBotton = false;
        }


    }

    void chargeShoot()
    {
        if (holdBotton && chargeTime < chargeLimit && CanChergeShoot)
        {
            chargeTime += Time.deltaTime * chargeSpeed;
        }else if (!holdBotton && chargeTime >= chargeLimit)
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
  
