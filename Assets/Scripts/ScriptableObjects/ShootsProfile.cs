using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;


[CreateAssetMenu(fileName = "ShotProfile", menuName = "ShootControler/ShotProfile")]

public class ShootsProfile : ScriptableObject
{
    public float bulletSpeed = 10f;
    public Vector2 moveDir;
    public float lifeTime = 5f;
    public float damege;
    public float raycastDistance = 0.2f;
    public ShootType shootType;

}
