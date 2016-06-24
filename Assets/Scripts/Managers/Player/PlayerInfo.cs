using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerInfo {
    [SerializeField]
    private PlayerId _id;

    [SerializeField]
    private Color _color;

    [SerializeField]
    private string _postfix;

    [SerializeField]
    private bool _canAutoDisconnect;

    public PlayerId Id {
        get { return _id; }
        set { _id = value; }
    }

    public Color Color {
        get { return _color; }
        set { _color = value; }
    }

    public string Postfix {
        get { return _postfix; }
        set { _postfix = value; }
    }

    public bool CanAutoDisconnect {
        get { return _canAutoDisconnect; }
        set { _canAutoDisconnect = value; }
    }

    public bool IsConnected { get; set; }

    public float AutoDisconnectCounter { get; set; }

    public int CharacterSelectionId { get; set; }

    public GameObject CharacterInstance { get; set; }
}