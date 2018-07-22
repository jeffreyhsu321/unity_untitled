using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Animator anim;

    public void btnPlay() {
        anim.SetTrigger("play");
        StartCoroutine(Play(1f));
    }

    public void btnQuit() {
        Application.Quit();
    }

    IEnumerator Play(float sec) {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene("room_bio");
    }

}
