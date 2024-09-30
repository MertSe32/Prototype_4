using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;
    

    public int enemyCount;
    public int waveNumber = 1;

    private int spawnRange = 9;

    // Start is called before the first frame update
    void Start()
    {
        int randomPowerup = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[randomPowerup], GenerateSpawnPosition(),powerupPrefab[randomPowerup].transform.rotation);
        
        SpawnEnemyWave(waveNumber);
        SpawnPowerup();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Düşmanların kaç tane kaldığını sürekli kontrol ediyor.
        enemyCount = FindObjectsOfType<Enemy>().Length;

        //Her dalgada daha fazla düşman çıkarıyor.
        if (enemyCount == 0)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            SpawnPowerup();

            int randomPowerup = Random.Range(0, powerupPrefab.Length);
            Instantiate(powerupPrefab[randomPowerup], GenerateSpawnPosition(),
            powerupPrefab[randomPowerup].transform.rotation);
        }
    }

    //Her yeni wave için 1 tane güçlendiriye ihtiyacımız var.
    void SpawnPowerup()
    {
        int randomPowerup = Random.Range(0, powerupPrefab.Length);

        Instantiate(powerupPrefab[randomPowerup], GenerateSpawnPosition(), powerupPrefab[randomPowerup].transform.rotation);
    }
    /*IDE yaptı!
    private void Instantiate(GameObject[] powerupPrefabs, Vector3 vector3)
    {
        throw new System.NotImplementedException();
    }*/

    //spawnToEnemy değişkeni oluşturduk ve spawnlanama döngüsünü hazırladık.
    void SpawnEnemyWave(int spawnToEnemy)
    {
        List<GameObject> enemyList = new List<GameObject>(enemyPrefab);

        int randomEnemy = Random.Range(0, enemyPrefab.Length);

        for (int i = 0; i < spawnToEnemy; i++)
        {
            Instantiate(enemyList[randomEnemy], GenerateSpawnPosition(), enemyPrefab[randomEnemy].transform.rotation);
        }
    }

    //Random spawn point oluşturma yeri. Return komutu ile tüm değişken return satırındaki değere dönüşüyor.
    //Yani return e atanan bir değer bu metodu çağırdığımızda geliyor.
    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }
    
}
/*```cs
int number = UnityEngine.Random.Range(0, 10);

for (int i = 0; i < 10; ++i)
{
    Debug.Log(number);
}
```

vs.

```cs
for (int i = 0; i < 10; ++i)
{
    int number = UnityEngine.Random.Range(0, 10);
    Debug.Log(number);
}
```*/