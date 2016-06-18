using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseDatabaseAsset {
    [SerializeField]
    private int _id;

    public int Id {
        get { return _id; }
        set { _id = value; }
    }

    public virtual string Name {
        get {
            return Id.ToString("D4");
        }
        set {

        }
    }

    public BaseDatabaseAsset() {
        Id = -1;
    }

    public BaseDatabaseAsset(int id) {
        Id = id;
    }
}

