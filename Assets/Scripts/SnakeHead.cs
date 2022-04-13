using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : BodyPart
{
    Vector2 movement=new Vector2(0.01f,0.01f);

    private BodyPart tail = null;

    const float Timetoaddabodypart = 0.1f;
    float addtimer = Timetoaddabodypart;

    public int partstoadd = 0;

    List<BodyPart> parts = new List<BodyPart>();

    public AudioSource[] gulpsounds = new AudioSource[3];
    public AudioSource diesound = null;

    // Start is called before the first frame update
    void Start()
    {
        Swipe.OnSwipe += SwipeDectection;
    }

    // Update is called once per frame
    override public void Update()
    {
        if (!GameControler.instance.alive)
        {
            return;
        }

        base.Update();

        SetMovement(movement * Time.deltaTime);
        UpdateDirection();
        UpdatePosition();

        if(partstoadd>0)
        {
            addtimer -= Time.deltaTime;
            if(addtimer<=0)
            {
                addtimer = Timetoaddabodypart;
                AddBodyPart();
                partstoadd--;
            }
        }
    }

    void SwipeDectection(Swipe.SwipeDirection direction)
    {
        switch(direction)
        {
            case Swipe.SwipeDirection.Up:
                MoveUp();
                break;
            case Swipe.SwipeDirection.Down:
                MoveDown();
                break;
            case Swipe.SwipeDirection.Right:
                MoveRight();
                break;
            case Swipe.SwipeDirection.Left:
                MoveLeft();
                break;
        }
    }

    void AddBodyPart()
    {
        if(tail == null)
        {
            Vector3 newposition = transform.position;
            newposition.z = newposition.z + 0.01f;

            BodyPart newPart = Instantiate(GameControler.instance.bodyprefab, transform.position, Quaternion.identity);
            newPart.following = this;
            tail = newPart;
            newPart.TurnIntoTail();

            parts.Add(newPart);
        }
        else
        {
            Vector3 newposition = tail.transform.position;
            newposition.z = newposition.z + 0.01f;

            BodyPart newpart = Instantiate(GameControler.instance.bodyprefab, newposition, tail.transform.rotation);
            newpart.following = tail;
            newpart.TurnIntoTail();
            tail.TurnIntoBodyPart();
            tail = newpart;

            parts.Add(newpart);
        }
    }

    void MoveUp()
    {
        movement = Vector2.up * GameControler.instance.snakeSpeed;
    }

    void MoveDown()
    {
        movement = Vector2.down * GameControler.instance.snakeSpeed;
    }

    void MoveLeft()
    {
        movement = Vector2.left * GameControler.instance.snakeSpeed;
    }

    void MoveRight()
    {
        movement = Vector2.right * GameControler.instance.snakeSpeed;
    }

    public void ResetSnake()
    {
        foreach(BodyPart part in parts)
        {
            Destroy(part.gameObject);
        }
        parts.Clear();

        tail = null;
        MoveUp();

        partstoadd = 5;
        addtimer = Timetoaddabodypart;
        ResetMemory();

        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0); //up
        gameObject.transform.position = new Vector3(0, 0, -8f); //center
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Egg egg = collision.GetComponent<Egg>();
        if(egg)
        {
            Debug.Log("Hit an egg");
            eatEgg(egg);
            int rand = Random.Range(0, 3);
            gulpsounds[rand].Play();
        }
        else
        {
            Debug.Log("Hit an obstacle");
            GameControler.instance.GameOver();
            diesound.Play();
        }
    }

    private void eatEgg(Egg egg)
    {
        partstoadd = 5;
        addtimer = 0;

        GameControler.instance.eggEaten(egg);
    }
}
