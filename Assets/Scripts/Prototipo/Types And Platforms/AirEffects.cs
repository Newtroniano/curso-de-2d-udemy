using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalTypes;

public class AirEffects : MonoBehaviour
{

    public AirEffectorType airEffectorTypes;
    public float speed;
    public Vector2 direction;
    private BoxCollider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        direction = transform.up;
        _collider = gameObject.GetComponent<BoxCollider2D>();
    }

    public void DeactiveEffector()
    {
        StartCoroutine("DeactiveEfectorCourotine");

    }

    IEnumerator DeactiveEfectorCourotine()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _collider.enabled = true;
    }


}
