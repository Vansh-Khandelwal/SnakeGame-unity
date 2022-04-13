using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    //unity stores touch in an array form of the position where a touch is made therefore a swipe would an array of touches
    //and even single touch will be stored but in that case swipestart and swipeend will
    //be very close therefore we are setting minimum distance criteria

    Vector2 swipeStart;
    Vector2 swipeEnd;

    //delegates - way of storing reference to function (used for events)
    public static event System.Action<SwipeDirection> OnSwipe = delegate {};

    float minimumDistance = 10;
    public enum SwipeDirection
    {
        Up,Down,Left,Right
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                ProcessSwipe();
            }
        }

        //mouse touch simulation
        if(Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
            ProcessSwipe();
        }
    }

    void ProcessSwipe()
    {
        float distance = Vector2.Distance(swipeStart, swipeEnd);
        if(distance>minimumDistance)
        {
            if(IsVerticalSwipe())
            {
                if (swipeEnd.y > swipeStart.y)
                {
                    OnSwipe(SwipeDirection.Up);
                }
                else
                {
                    OnSwipe(SwipeDirection.Down);
                }
            }
            else //horizontal
            {
                if (swipeEnd.x > swipeStart.x)
                {
                    OnSwipe(SwipeDirection.Right);
                }
                else
                {
                    OnSwipe(SwipeDirection.Left);
                }
            }
        }
    }

    bool IsVerticalSwipe()
    {
        float vertical = Mathf.Abs(swipeEnd.y - swipeStart.y);
        float horizontal = Mathf.Abs(swipeEnd.x - swipeStart.x);
        if(vertical>horizontal)
        {
            return true;
        }
        return false;
    }
}
