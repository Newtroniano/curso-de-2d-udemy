using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsePlatform : GroundEffector
{

    public float fallSpeed = 10f;
    public float delayTime = 0.5f;

    public Vector3 difference;

    private bool _platformCollapsing = false;
    public Rigidbody2D _rigidbody;
    private Vector3 _lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _lastPosition = transform.position;

        if (_platformCollapsing)
        {

            _rigidbody.AddForce(Vector2.down * fallSpeed);


            if (_rigidbody.velocity.y == 0)
            {
                _platformCollapsing = false;
                _rigidbody.bodyType = RigidbodyType2D.Static;
            }
        }
    }



    private void LateUpdate()
    {
        difference = transform.position - _lastPosition;
    }

    public void CollapsedPlatform()
    {
        StartCoroutine("CollapsePlatformCoroutine");
    }

    public IEnumerator CollapsePlatformCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        _platformCollapsing = true;

        _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidbody.freezeRotation = true;
        _rigidbody.gravityScale = 1f;
        _rigidbody.mass = 1000f;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
