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
    private bool _canDisconnect;

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

    public bool CanDisconnect {
        get { return _canDisconnect; }
        set { _canDisconnect = value; }
    }

    public bool IsConnected { get; set; }

    public float DisconnectCounter { get; set; }

    public int CharacterSelection { get; set; }
}