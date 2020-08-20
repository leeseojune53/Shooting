using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeimage;
    public Image[] boomimage;
    public GameObject GameOverSet;

    

    public void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        Player PlayerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", PlayerLogic.score);
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            GameRetry();
    }

    void SpawnEnemy()
    {
        int RanEnemy = Random.Range(0, 3);
        int RanPoint = Random.Range(0, 9);

        GameObject enemy = Instantiate(enemyObjs[RanEnemy], 
                                       spawnPoints[RanPoint].position,
                                       spawnPoints[RanPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        Enemy EnemyLogic = enemy.GetComponent<Enemy>();
        EnemyLogic.player = player;

        if (RanPoint == 5 || RanPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back*90);
            rigid.velocity = new Vector2(EnemyLogic.speed * (-1), -1);
        }else if (RanPoint == 7 || RanPoint == 8)
        {
            enemy.transform.Rotate(Vector3.back * -90);
            rigid.velocity = new Vector2(EnemyLogic.speed, -1);
        }
        else
        {
            rigid.velocity = new Vector2(0, EnemyLogic.speed * (-1));
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerEXE", 2.0f);
    }
    void RespawnPlayerEXE()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player PlayerLogic = player.GetComponent<Player>();
        PlayerLogic.isHit = false;

    }

    public void UpdateLifeIcon(int life)
    {
        for(int index = 0;index < 3; index++)
        {
            lifeimage[index].color = new Color(1, 1, 1, 0);
        }

        for(int index = 0; index < life; index++)
        {
            lifeimage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomimage[index].color = new Color(1, 1, 1, 0);
        }

        for (int index = 0; index < boom; index++)
        {
            boomimage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void GameOver()
    {
        GameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

}
