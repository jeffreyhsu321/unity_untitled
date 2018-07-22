using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AcceptQuest : MonoBehaviour {

    public void btnAcceptQuest(string sceneName)
    {
        StartCoroutine(Play( sceneName));
    }

    IEnumerator Play(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
