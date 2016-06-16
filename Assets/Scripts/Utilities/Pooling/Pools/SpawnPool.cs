using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpawnPool : AbstractPool<SpawnPool> {
    public override void InitializePool() {
        for (int i = 0; i < SpawnDatabase.Instance.Count; i++) {
            Debug.Log("Adding Object to Spawn Pool");
            AddAssetToPool(SpawnDatabase.Instance.Get(i) as IPoolableObject);
        }
    }

    public override IPoolableObject GetObject(int id) {
        return SpawnDatabase.GetAsset(id) as IPoolableObject;
    }

    public override bool IsObjectRestricted(int id) {
        var asset = SpawnDatabase.GetAsset(id) as IPoolableObject;
        if (asset != null) {
            return asset.PoolRestricted;
        }
        return false;
    }

    public GameObject ActivateObject(GameObject prefab) {
        return ActivateObject(prefab.name);
    }

    public GameObject ActivateObject(string name) {
        return Activate(GetObjectIdFromName(name));
    }

    static public GameObject Activate(GameObject prefab) {
        return Instance.ActivateObject(prefab.name);
    }

    static public GameObject Activate(string name) {
        return Instance.ActivateObject(name);
    }

    static public GameObject Activate(int id) {
        return Instance.ActivateObject(id);
    }

    static public void Deactivate(GameObject gameObject) {
        Instance.DeactivateObject(gameObject);
    }
}