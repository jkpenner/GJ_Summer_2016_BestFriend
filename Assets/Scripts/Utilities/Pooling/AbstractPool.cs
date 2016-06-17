using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The basic class for any gameobject pools. The pool uses
/// a Dictionary that stores all objects in Lists that are
/// separated by the object's Id.
/// 
/// Also automatically moves all objects created in the pool
/// left or right when the generator resets its position.
/// </summary>
public abstract class AbstractPool<T> : Singleton<T> where T : MonoBehaviour {
    private Dictionary<int, List<GameObject>> _objects;
    public Dictionary<int, List<GameObject>> Objects {
        get {
            if (_objects == null) {
                _objects = new Dictionary<int, List<GameObject>>();
            }
            return _objects;
        }
    }

    public void Awake() {
        InitializePool();
    }

    public abstract void InitializePool();
    public abstract IPoolableObject GetObject(int id);
    public abstract bool IsObjectRestricted(int id);

    public GameObject ActivateObject(int id) {
        if (Objects.ContainsKey(id)) {
            if (Objects[id].Count > 0) {
                for (int i = 0; i < Objects[id].Count; i++) {
                    if (Objects[id][i].activeInHierarchy == false) {
                        Objects[id][i].SetActive(true);
                        return Objects[id][i];
                    }
                }
            }
        }

        if (!IsObjectRestricted(id)) {
            return GenerateObject(id);
        } else {
            Debug.LogWarningFormat("[{0}]: Trying Create Instance of Id: {1}, " +
                "but the Object generation is restricted. Can not create a new instance.", name, id);
            return null;
        }
    }

    public void DeactivateObject(GameObject gameObject) {
        // Check if the gameobject is within the pool
        foreach (var pair in Objects) {
            if (pair.Value.Count > 0) {
                for (int i = 0; i < pair.Value.Count; i++) {
                    if (pair.Value[i] == gameObject) {
                        gameObject.transform.SetParent(GetObjectParent(gameObject.name));
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public GameObject GenerateObject(int id) {
        // Pool does not contains a block with given id
        // Generate a new object with the id
        var asset = GetObject(id);
        if (asset != null) {
            var parent = GetObjectParent(asset.Name);
            AddAssetToPool(asset.Id, asset.Prefab, parent);
            Objects[asset.Id][Objects[asset.Id].Count - 1].SetActive(true);
            return Objects[asset.Id][Objects[asset.Id].Count - 1];
        }
        // No object exsists within the database
        else {
            Debug.LogWarningFormat("[{0}]: Trying Create Instance of Block Id: {1}, " +
                "but the BlockDatabase does not contain block with given id", name, id);
            return null;
        }
    }

    public Transform GetObjectParent(string name) {
        var parent = transform.Find(name.Replace("(Clone)", ""));
        if (parent == null) {
            parent = new GameObject(name).transform;
            parent.SetParent(transform);
        }
        return parent;
    }

    public int GetObjectIdFromName(string name) {
        for (int i = 0; i < SpawnDatabase.Instance.Count; i++) {
            if (SpawnDatabase.Instance.name == name) {
                return i;
            }
        }
        return -1;
    }

    public void AddAssetToPool(IPoolableObject asset) {
        if (asset != null && asset.Prefab != null && asset.PoolCount > 0) {
            var parent = GetObjectParent(asset.Name);
            AddAssetToPoolMulti(asset.Id, asset.Prefab, parent, asset.PoolCount);
        }
    }

    public void AddAssetToPool(int id, GameObject prefab, Transform parent) {
        if (!Objects.ContainsKey(id)) {
            Objects.Add(id, new List<GameObject>());
        }

        var instance = Instantiate(prefab);
        if (instance != null) {
            Objects[id].Add(instance);
            instance.transform.SetParent(parent.transform);
            instance.SetActive(false);
        }
    }

    public void AddAssetToPoolMulti(int id, GameObject prefab, Transform parent, int count) {
        for (int i = 0; i < count; i++) {
            AddAssetToPool(id, prefab, parent);
        }
    }
}
