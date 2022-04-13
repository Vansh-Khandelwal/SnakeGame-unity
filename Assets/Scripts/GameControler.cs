using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControler : MonoBehaviour
{
    int level = 0;
    int noofeggsfornextlevel = 0;

    public int score = 0;
    public int highscore = 0;

    public static GameControler instance = null;

    const float width = 3.7f;
    const float height = 7f;

    public float snakeSpeed = 1f;

    public Sprite tailSprite = null;
    public Sprite bodySprite = null;
    public BodyPart bodyprefab = null;

    public GameObject rockPrefab = null;

    public GameObject eggPrefab = null;
    public GameObject goldeneggPrefab = null;

    public SnakeHead snakehead = null;

    public GameObject spikePrefab = null;

    public bool alive = true;

    public bool waitingtoplay = true;

    public Text scoreText = null;
    public Text highscoreText = null;

    public Text taptoplayText = null;
    public Text gameoverText = null;

    List<Egg> eggs = new List<Egg>();
    List<Spike> spikes = new List<Spike>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Debug.Log("Starting Snake game");
        CreateWalls();
        CreateEgg();
        alive = false;
    }

    void startGamePlay()
    {
        scoreText.text = "Score = " + score;
        highscoreText.text = "HighScore = " + highscore;

        score = 0;
        level = 0;
        alive = true;
        waitingtoplay = false;

        taptoplayText.gameObject.SetActive(false);
        gameoverText.gameObject.SetActive(false);

        KillOldEggs();
        KillOldSpikes();

        LevelUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(waitingtoplay)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Ended)
                {
                    startGamePlay();
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                startGamePlay();
            }
        }
    }

    void LevelUp()
    {
        level++;
        noofeggsfornextlevel = 4 + (level * 2); // 6 8 10 12 ...

        snakeSpeed = (level / 4f) + 1f;
        if(snakeSpeed>6)
        {
            snakeSpeed = 6;
        }

        snakehead.ResetSnake();
        CreateEgg();

        KillOldSpikes();
        CreateSpike();
    }

    public void eggEaten(Egg egg)
    {
        noofeggsfornextlevel--;
        score++;
        if(noofeggsfornextlevel == 0)
        {
            score += 10;
            LevelUp();
        }
        else if(noofeggsfornextlevel==1)
        {
            CreateEgg(true);
        }
        else
        {
            CreateEgg();
        }

        if(score>highscore)
        {
            highscore = score;
        }

        scoreText.text = "Score = " + score;
        highscoreText.text = "HighScore = " + highscore;

        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }
    void CreateWalls()
    {
        Vector3 start = new Vector3(-width, -height, 0);
        Vector3 finish = new Vector3(-width, height, 0);
        Createwall(start, finish);

        start = new Vector3(width, -height, 0);
        finish = new Vector3(width, height, 0);
        Createwall(start, finish);

        start = new Vector3(-width, -height, 0);
        finish = new Vector3(+width, -height, 0);
        Createwall(start, finish);

        start = new Vector3(-width, height, 0);
        finish = new Vector3(+width, height, 0);
        Createwall(start, finish);
    }

    void Createwall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noofrocks = (int)(distance * 3f);
        Vector3 delta = (finish - start) / noofrocks;

        Vector3 position = start;
        for(int rock = 0; rock<=noofrocks; rock++)
        {
            float rotation = Random.Range(0, 360f);
            float scale = Random.Range(1.5f, 2f);
            CreateRock(position, scale, rotation);
            position = position + delta;
        }
    }

    void CreateRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0, 0, rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }

    void CreateEgg(bool golden = false)
    {
        Vector3 position;
        position.x = -width + Random.Range(1f, (width * 2) - 2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = 0;
        Egg egg = null;

        if (golden)
        {
            egg = Instantiate(goldeneggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }
        else
        {
            egg = Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        }

        eggs.Add(egg);
    }

    void CreateSpike()
    {
        Spike spike = null;
        for (int i = 0; i < level; i++)
        {
            Vector3 position;
            position.x = -width + Random.Range(1f, (width * 2) - 2f);
            position.y = -height + Random.Range(1f, (height * 2) - 2f);
            position.z = 0;

            spike = Instantiate(spikePrefab, position, Quaternion.identity).GetComponent<Spike>();

            spikes.Add(spike);
        }

    }

    void KillOldEggs()
    {
        foreach(Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }

    void KillOldSpikes()
    {
        foreach (Spike spike in spikes)
        {
            Destroy(spike.gameObject);
        }
        spikes.Clear();
    }

    public void GameOver()
    {
        alive = false;
        waitingtoplay = true;

        taptoplayText.gameObject.SetActive(true);
        gameoverText.gameObject.SetActive(true);
    }
}
