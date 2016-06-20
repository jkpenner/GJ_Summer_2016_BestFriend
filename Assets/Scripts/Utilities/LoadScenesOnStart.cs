using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScenesOnStart : MonoBehaviour {
    public int[] scenesToLoad;

	void Awake () {
        for (int i = 0; i < scenesToLoad.Length; i++) {
            SceneManager.LoadScene(scenesToLoad[i], LoadSceneMode.Additive);
        }
	}
}
