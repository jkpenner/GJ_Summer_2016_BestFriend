using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScenesOnStart : MonoBehaviour {
    private bool loadOnce = true;
    public int[] scenesToLoad;

	void Start () {
        if (loadOnce) {
            Debug.LogFormat("[{0}]: Loading External Scenes", this.name);
            for (int i = 0; i < scenesToLoad.Length; i++) {
                SceneManager.LoadScene(scenesToLoad[i], LoadSceneMode.Additive);
            }
            loadOnce = false;
        }
    }
}
