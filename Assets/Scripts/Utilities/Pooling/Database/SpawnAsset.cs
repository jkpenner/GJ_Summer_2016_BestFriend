using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnAsset : BaseDatabaseAsset, IPoolableObject {
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private int _poolCount = 0;

    [SerializeField]
    private bool _poolRestricted = false;

    public GameObject Prefab {
        get { return _prefab; }
        set { _prefab = value; }
    }

    public override string Name {
        get {
            if (Prefab != null) {
                return Prefab.name;
            }
            return string.Empty;
        }
    }

    public bool PoolRestricted {
        get { return _poolRestricted; }
        set { _poolRestricted = value; }
    }

    public int PoolCount {
        get { return _poolCount; }
        set { _poolCount = value; }
    }

    public SpawnAsset(int id) : base(id) {

    }
}
