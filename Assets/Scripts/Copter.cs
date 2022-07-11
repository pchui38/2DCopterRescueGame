using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copter : MonoBehaviour
{
    public float engineForce = 500f;
    public float rotationForce = 1;
    public Transform fan;
    public float maxFanSpeed = 5000f;

    private float currentFanSpeed;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private float SFXvol;
    private float SFXpitch;
    private bool facingRight = true;
    private float moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SFXvol = 0;
        SFXpitch = 0;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        fan.Rotate(0, 0, currentFanSpeed * Time.deltaTime);
        moveDirection = Input.GetAxis("Horizontal");

        if (moveDirection > 0 && !facingRight)
        {
            FlipCopter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCopter();
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.up * engineForce * Time.deltaTime);
            currentFanSpeed = Mathf.Lerp(currentFanSpeed, maxFanSpeed, Time.deltaTime);
            SFXvol = Mathf.Lerp(SFXvol, 1, Time.deltaTime);
            SFXpitch = Mathf.Lerp(SFXpitch, 1, Time.deltaTime);
        }
        else
        {
            currentFanSpeed = Mathf.Lerp(currentFanSpeed, 1000, Time.deltaTime);
            SFXvol = Mathf.Lerp(SFXvol, 0, Time.deltaTime);
            SFXpitch = Mathf.Lerp(SFXpitch, 0, Time.deltaTime);
        }

        audioSource.volume = SFXvol;
        audioSource.pitch = SFXpitch;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveDirection > 0)
                transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
            else
                transform.Rotate(Vector3.back * rotationForce * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (moveDirection < 0)
                transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
            else
                transform.Rotate(Vector3.back * rotationForce * Time.deltaTime);
        }
    }
    
    private void FlipCopter()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || 
            collision.gameObject.CompareTag("Hospital") ||
            collision.gameObject.CompareTag("House"))
        {
            GameManager.currentState = GameManager.GameState.CopterDestroyed;
        }

        if (collision.gameObject.CompareTag("Base") && 
            GameManager.currentState == GameManager.GameState.ReturnToBase)
        {
            GameManager.currentState = GameManager.GameState.SurvivorDispatch;
        }
    }
}
