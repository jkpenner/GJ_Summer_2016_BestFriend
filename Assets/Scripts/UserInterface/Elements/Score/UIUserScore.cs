using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIUserScore : MonoBehaviour {
    public Text txtUserName;
    public Text txtUserScore;

    public string UserName {
        get { return txtUserName.text; }
        set { txtUserName.text = value; }
    }

    public string UserScore {
        get { return txtUserScore.text; }
        set { txtUserScore.text = value; }
    }
}
