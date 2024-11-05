using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private const string ENEMY_PATROL = "Enemy_Patrol";
    Animator animator;
    private bool facingRight = true;
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    [SerializeField] public Transform groundDetection;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.Play(ENEMY_PATROL);

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

        if (groundInfo.collider == false)
        {
            if (facingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                facingRight = false;
            } else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingRight = true;
            }
        }
    }
}
