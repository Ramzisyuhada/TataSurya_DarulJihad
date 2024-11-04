using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance;
    public List<GameObject> objects;
    public List<GameObject> objectsl;

    public GameObject objectToPool;
    public int amountToPool;
    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
        GameObject lsr;
        objects = new List<GameObject>();
        objectsl = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            lsr = Instantiate(objectToPool);
            lsr.SetActive(false);
            objects.Add(lsr);
        }
        GameObject lsr1;

        for (int i = 0; i < amountToPool; i++)
        {
            lsr1 = Instantiate(objectToPool);
            lsr1.SetActive(false);
            objectsl.Add(lsr1);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!objects[i].activeInHierarchy)
            {
                return objects[i];
            }
        }
        return null;

    }

    public GameObject GetPooledObjectl()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!objectsl[i].activeInHierarchy)
            {
                return objectsl[i];
            }
        }
        return null;

    }

}
