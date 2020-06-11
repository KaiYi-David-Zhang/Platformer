using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public Animator animator;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {   // Player hits the change scene place
            // TODO: disable player's movement
            StartCoroutine(loadNextLevel());
        }
    }

    IEnumerator loadNextLevel()
    {
        animator.SetTrigger("startTransition");

        yield return new WaitForSeconds(1); // wait for 1 sceond

        SceneManager.LoadScene(sceneName);

    }

}
