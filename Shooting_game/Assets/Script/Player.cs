using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
    public bool isHit;
    public bool isBoomTime;
    public bool isReSpawnTime;

    public int life;
    public int score;
    public int power;
    public int boom;
    public int maxPower;
    public int maxBoom;

    public float speed;
    public float maxShotDelay;
    public float curShotDelay;

    public GameManager manager;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject BoomEffect;

    Animator anim;

    void Awake() 
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
        
    }

    void SpawnDelayTime()
    {
        isReSpawnTime = false;
    }

    void Boom()
    {
        if (!Input.GetKeyDown(KeyCode.F))
            return;

        if (boom == 0)
            return;

        if (isBoomTime)
            return;

        isBoomTime = true;
        --boom;
        manager.UpdateBoomIcon(boom);
        BoomEffect.SetActive(true);

        Invoke("offBoomEffect", 4f);


        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int index = 0; index < enemies.Length; index++)
        {
            Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }

        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int index = 0; index < Bullets.Length; index++)
        {
            Destroy(Bullets[index]);
        }
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Fire()
    {
        if (!Input.GetButton("Jump"))
           return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right*0.1f, transform.rotation);
                GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left*0.1f, transform.rotation);
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletC = Instantiate(bulletObjB, transform.position, transform.rotation);
                Rigidbody2D rigidC = bulletC.GetComponent<Rigidbody2D>();
                rigidC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

            case 4:
                GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.35f, transform.rotation);
                GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);
                GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.35f, transform.rotation);
                
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

        }
        

        curShotDelay = 0;
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if((h == 1 && isTouchLeft)||(h == -1 && isTouchRight))
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom))
            v = 0;


        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

    transform.Translate(nextPos);

        if (Input.GetButtonDown("Horizontal") || 
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int) h);
}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;
            isHit = true;
            if (!isReSpawnTime)
            {
                --life;
                manager.UpdateLifeIcon(life);
                if (life == 0) manager.GameOver();
                else manager.RespawnPlayer();
                isReSpawnTime = true;
                gameObject.SetActive(false);
            }
            Invoke("SpawnDelayTime", 5f);
            
            
            if(collision.gameObject.tag == "Enemy")
            {
                Enemy E = collision.gameObject.GetComponent<Enemy>();
                E.OnHit(1000);
            }else Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Power":
                    if (maxPower == power)
                        score += 500;
                    else ++power;
                    break;
                case "Coin":
                    score += 1000;
                    break;
                case "Boom":
                    if (maxBoom == boom)
                        score += 700;
                    else 
                    { 
                        ++boom;
                        manager.UpdateBoomIcon(boom);
                    } 
                    
                    break;
            }
            Destroy(collision.gameObject);
        }
    }

    void offBoomEffect()
    {
        BoomEffect.SetActive(false);
        isBoomTime = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
