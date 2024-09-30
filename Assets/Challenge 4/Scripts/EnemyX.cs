using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyX : MonoBehaviour
{
    public float speed;
    public GameObject playerGoal;

    private Rigidbody enemyRb;

    private SpawnManagerX spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManagerX>();

        for (int i = 0; i < spawnManager.waveCount - 1; i++)
        {
            speed += 25f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set enemy direction towards player goal and move there
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

    }

    private void OnCollisionEnter(Collision carpistigiSey)
    {
        // If enemy collides with either goal, destroy it
        if (carpistigiSey.gameObject.name == "Enemy Goal")
        {
            Destroy(gameObject);
        } 
        else if (carpistigiSey.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
        }

    }

}
