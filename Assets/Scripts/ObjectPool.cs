using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    List<GameObject> _freeObjects;
    List<GameObject> _usedObjects;

    GameObject prefabReference;

    // Start is called before the first frame update
    void Start()
    {
        _freeObjects = new List<GameObject>();
        _usedObjects = new List<GameObject>();
    }

    public void CreatePool(GameObject prefabObject, int capacity)
    {
        prefabReference = prefabObject;

        for (int i = 0; i < capacity; ++i)
        {
            GameObject instantiated = Instantiate(prefabReference, Vector3.one * -1000, Quaternion.identity, transform);

            _freeObjects.Add(instantiated);

            instantiated.SetActive(false);
        }
    }

    public GameObject GetObject()
    {
        if(_freeObjects.Count > 0)
        {
            GameObject selectedObject = _freeObjects[0];

            _freeObjects.RemoveAt(0);

            _usedObjects.Add(selectedObject);

            selectedObject.SetActive(true);

            return selectedObject;
        }
        else
        {
            GameObject instantiated = Instantiate(prefabReference, Vector3.one * -1000, Quaternion.identity, transform);

            _usedObjects.Add(instantiated);

            instantiated.SetActive(true);

            return instantiated;
        }
    }

    public void ReleaseObject(GameObject poolObject)
    {
        _usedObjects.Remove(poolObject);

        _freeObjects.Add(poolObject);

        poolObject.SetActive(false);
    }
}
