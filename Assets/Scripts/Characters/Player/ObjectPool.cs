using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("NormalShoot")]
    public int amountToPoolnormalShot = 3;
    public static ObjectPool instance;
    public List<GameObject> normalShootpol = new List<GameObject>();
    [SerializeField] public GameObject normalShootprefab;


    //chargeShoot
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
       for (int i =0; i < amountToPoolnormalShot; i++)
        {
            GameObject obj = Instantiate(normalShootprefab);
            obj.SetActive(false);
            normalShootpol.Add(obj);
            obj.transform.parent = transform;
        }


    }

    public GameObject GetNormalShootPool()
    {
        for(int i = 0; i< normalShootpol.Count; i++)
        {
            if (!normalShootpol[i].activeInHierarchy)
            {
                return normalShootpol[i];
            }
        }
        return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
