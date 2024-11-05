using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersBehaviour : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            RestartLevel();
        }
    }
    public void RestartLevel()
    {
        if (Application.loadedLevelName == "MainScene")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } 
        if (Application.loadedLevelName == "SecondLevel")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}