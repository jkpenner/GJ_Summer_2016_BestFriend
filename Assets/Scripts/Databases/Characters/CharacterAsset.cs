using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterAsset : BaseDatabaseAsset {
    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private Sprite _icon;

    public GameObject Prefab {
        get { return _prefab; }
        set { _prefab = value; }
    }

    public Sprite Icon {
        get { return _icon; }
        set { _icon = value; }
    }

    public override string Name {
        get {
            if (Prefab != null) {
                return Prefab.name;
            }
            return string.Empty;
        }
    }

    public CharacterAsset(int id) : base(id) {

    }
}
