using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAtrributes 
{

    private Vector2 _moveAmount;
    private Vector2 _currentPosition;
    private Vector2 _lastPosition;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;
    //Variables Get and Set Player Atributes
    [SerializeField]
    public  Vector2 MoveAmount { get => _moveAmount; set => _moveAmount = value; }
    public  Vector2 CurrentPosition { get => _currentPosition; set => _currentPosition = value; }
    public  Vector2 LastPosition { get => _lastPosition; set => _lastPosition = value; }

   
}
