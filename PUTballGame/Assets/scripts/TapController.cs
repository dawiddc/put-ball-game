using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;
    public AudioSource tapSound;
    public AudioSource scoreSound;
    public AudioSource dieSound;
    public AudioSource gameSound;

    Rigidbody2D rigidBody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
    TrailRenderer trail;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -100);
        forwardRotation = Quaternion.Euler(0, 0, 40);
        game = GameManager.Instance;
        rigidBody.simulated = false;
        gameSound.Play();
        gameSound.loop = true;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.GameOver) return;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.position.x < Screen.width / 2)
            {
                rigidBody.velocity = Vector2.zero;
                transform.rotation = forwardRotation;
                rigidBody.AddForce(Vector2.left * tapForce, ForceMode2D.Force);
                tapSound.Play();
            }
            else if (touch.position.x > Screen.width / 2)
            {
                rigidBody.velocity = Vector2.zero;
                transform.rotation = forwardRotation;
                rigidBody.AddForce(Vector2.right * tapForce, ForceMode2D.Force);
                tapSound.Play();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                rigidBody.velocity = Vector2.zero;
                transform.rotation = forwardRotation;
                rigidBody.AddForce(Vector2.left * tapForce, ForceMode2D.Force);
                tapSound.Play();
            }
            else
            {
                rigidBody.velocity = Vector2.zero;
                transform.rotation = forwardRotation;
                rigidBody.AddForce(Vector2.right * tapForce, ForceMode2D.Force);
                tapSound.Play();
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored();
            scoreSound.Play();
        }
        if (col.gameObject.tag == "DeadZone")
        {
            rigidBody.simulated = false;
            OnPlayerDied();
            dieSound.Play();
        }
    }

}
