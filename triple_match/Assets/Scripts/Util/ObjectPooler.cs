using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;

    Dictionary<int, Queue<GameObject>> pooledObjects = new Dictionary<int, Queue<GameObject>>();
    Dictionary<int, int> retrievedObjectKeys = new Dictionary<int, int>();

    void Awake()
    {
        if (SharedInstance != null && this != SharedInstance)
        {
            Destroy(this);
        }
        else if (SharedInstance == null)
        {
            SharedInstance = this;
        }
    }

    public static GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int prefabKey = prefab.GetInstanceID();
        GameObject result = null;

        if (SharedInstance.pooledObjects.ContainsKey(prefabKey))
        {
            Queue<GameObject> objectQueue = SharedInstance.pooledObjects[prefabKey];
            if (objectQueue.Count > 0)
            {
                result = objectQueue.Dequeue();
                result.SetActive(true);
            }
            else
            {
                result = Instantiate(prefab, SharedInstance.transform);
            }
        }
        else
        {
            SharedInstance.pooledObjects.Add(prefabKey, new Queue<GameObject>());
            result = Instantiate(prefab, SharedInstance.transform);
        }

        SharedInstance.retrievedObjectKeys.Add(result.GetInstanceID(), prefabKey);

        result.transform.position = position;
        result.transform.rotation = rotation;

        return result;
    }

    public static bool Return(GameObject obj)
    {
        if (SharedInstance == null)
        {
            return false;
        }

        int instanceKey = obj.GetInstanceID();
        if (SharedInstance.retrievedObjectKeys.ContainsKey(instanceKey))
        {
            int poolKey = SharedInstance.retrievedObjectKeys[instanceKey];

            SharedInstance.retrievedObjectKeys.Remove(instanceKey);
            SharedInstance.pooledObjects[poolKey].Enqueue(obj);

            //在这里消除
            obj.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }
}
