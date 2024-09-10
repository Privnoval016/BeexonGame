using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class BallAnimation : MonoBehaviour
{

    private enum BallState
    {
        MovingUp,
        PauseAtTop,
        MovingDown,
        PauseAtBottom,
    }

    private BallState m_State;

    public float height = 10.0f;
    public float speed = 3.0f;
    public float delay = 1.0f;
    public float randomWait;

    public Animator anim;
    public GameObject model;

    private float timer;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float targetY, originalY;

    public FMODUnity.EventReference jumpSound;

    public float rotateSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);

        targetY = transform.position.y + height;
        originalY = transform.position.y;

        randomWait = Random.Range(0.5f, 1.5f);
        m_State = BallState.MovingUp;
        //StartCoroutine(MoveUpAndReset());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.S.isPaused) return;

            switch (m_State)
        {

            case BallState.MovingUp:
                MoveUp();
                break;
            case BallState.PauseAtTop:
                PauseAtTop();
                break;
            case BallState.MovingDown:
                MoveDown();
                break;
            case BallState.PauseAtBottom:
                PauseAtBottom();
                break;
        }
    }

    private void MoveUp()
    {
        anim.SetBool("isJumping", true);
        if (transform.position.y < targetY)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            m_State = BallState.PauseAtTop;
        }
    }

    private void PauseAtTop()
    {
        if (timer < delay)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            m_State = BallState.MovingDown;
        }
    }

    private void MoveDown()
    {
        if (transform.position.y > originalY)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
                
        }
        else
        {
            m_State = BallState.PauseAtBottom;
        }
    }

    private void PauseAtBottom()
    {
        anim.SetBool("isJumping", false);
        if (timer < randomWait)
        {
            timer += Time.deltaTime;

        }
        else
        {
            timer = 0;
            randomWait = Random.Range(0.5f, 1.5f);

            FMODUnity.RuntimeManager.PlayOneShot(jumpSound, transform.position);

            m_State = BallState.MovingUp;
        }
    }
}
