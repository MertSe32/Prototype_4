using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static PowerUp;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public bool hasPowerup = false;
    public bool bulletPowerup = false;
    public GameObject powerupIndicator;
    public GameObject bulletPowerupIndicator;
    public GameObject smashPowerUpIndicator;

    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerupStrenght = 20.0f;
    private float enemyFastPower = 5.0f;

    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountDown;

    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;

    //private Enemy enemyScript;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        //enemyScript = GameObject.Find("Enemy").GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        /*Player Input kısmı. Focal Point noktasını merkez alarak topun ileri geri gitmesini sağladık.*/
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed);

        //Güçlendirici göstergesinin topun altında kalmasını sağladık.
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
        bulletPowerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerUp == PowerUpType.Rocket && Input.GetKeyDown(KeyCode.Q))
        {
            FireBullet();
        }

        if(currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Oyuncu güçlendirici aldıktan efekt devreye girer. Son satırda zamanlayıcı çağırıldı.
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.gameObject.SetActive(true);
            StartCoroutine (PowerupCountdownRoutine());
            Destroy(other.gameObject);

            if(powerupCountDown != null)
            {
                StopCoroutine(powerupCountDown);
            }
            powerupCountDown = StartCoroutine(PowerupCountdownRoutine());
        }
        else if (other.CompareTag("BulletPowerUp"))
        {
            
            currentPowerUp = PowerUpType.Rocket;
            Destroy(other.gameObject);
            bulletPowerupIndicator.gameObject.SetActive(true);
            StartCoroutine(BulletPowerTimer());
        }
        else if (other.CompareTag("SmashPowerUp"))
        {
            currentPowerUp = PowerUpType.Smash;
            Destroy(other.gameObject);
            smashPowerUpIndicator.gameObject.SetActive(true);
            StartCoroutine(Smash());
        }
    }

    //Güçlendiriciyi sonlandırıcak 7 saniyelik zamanlayıcı 
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator BulletPowerTimer()
    {
        yield return new WaitForSeconds(5);
        bulletPowerup = false;
        bulletPowerupIndicator.gameObject.SetActive(false);

    }

    IEnumerator Smash()
    {
        Debug.Log("Smash Çalıştı!");
        var enemies = FindObjectsOfType<Enemy>();
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        //Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }
        //Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position.
            if (enemies[i] != null)
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,
                transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        //We are no longer smashing, so set the boolean to false
        smashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyFast"))
            && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            enemyRb.AddForce(awayFromPlayer * powerupStrenght, ForceMode.Impulse);

            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to "
                      + currentPowerUp.ToString());
        }
        else if (collision.gameObject.CompareTag("EnemyFast"))
        {
            Vector3 playerFly = transform.position - collision.gameObject.transform.position;

            playerRb.AddForce(playerFly * enemyFastPower, ForceMode.Impulse);
        }
        else
        {
            
        }
    }

    void FireBullet()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up,Quaternion.identity);
    
            tmpRocket.GetComponent<Bullet>().Fire(enemy.transform);
        }
    }
}

 /*private void OnCollisionEnter(Collision collision)
    {
        //Oyuncu düşmanla karşılaştığında güçlendirici aktifse
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            //Çarpıştığımız nesnenin rigidbody bileşenini aldık.
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            /*Çarpıştığımız nesnenin konumundan kendi konumumuzu çıkararak kendinden
            uzaklaşan yönde bir vektör oluşturduk
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            //Çarpışınca güçlendirmenin etkisiyle geriye uçacak bir kuvvet uyguladık.
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrenght, ForceMode.Impulse);
        }

        else if (collision.gameObject.CompareTag("EnemyFast") && hasPowerup)
        {
            Rigidbody enemyFastRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            enemyFastRigidbody.AddForce(awayFromPlayer * powerupStrenght, ForceMode.Impulse);
        }
        else 
        {
            Vector3 playerFly = transform.position - collision.transform.position;

            playerRb.AddForce(playerFly * enemyFastPower, ForceMode.Impulse);
        }
    }*/

/*if (collision.gameObject.CompareTag("EnemyFast"))
{
    Rigidbody enemyFastRigidbody = collision.gameObject.GetComponent<Rigidbody>();

    

    enemyFastRigidbody.AddForce(playerFly * enemyFastPower, ForceMode.Impulse);
}*/