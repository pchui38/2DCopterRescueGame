using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    public float speed = 0.01f;
    public float moveRange = 5f;

    private Vector3 helipadPos;
    private Vector3 hospitalPos;
    private Vector3 housePointA;
    private Vector3 housePointB;
    private bool movingLeft;
    private float lastPosX;

    void Start()
    {
        helipadPos = GameObject.Find("helipad").transform.position;
        hospitalPos = GameObject.Find("hospital").transform.position;
        housePointA = new Vector3(transform.position.x - moveRange, transform.position.y, 0);
        housePointB = new Vector3(transform.position.x + moveRange, transform.position.y, 0);
        movingLeft = true;
    }

    void Update()
    {
        switch (GameManager.currentState)
        {
            case GameManager.GameState.RescueOperation:
                lastPosX = transform.position.x;

                float time = Mathf.PingPong(Time.time * speed, 1);
                transform.position = Vector3.Lerp(housePointA, housePointB, time);

                if (transform.position.x < lastPosX && movingLeft == false)
                {
                    transform.Rotate(0f, 180f, 0f);
                    movingLeft = true;
                }
                else if (transform.position.x > lastPosX && movingLeft == true)
                {
                    transform.Rotate(0f, 180f, 0f);
                    movingLeft = false;
                }
                break;

            case GameManager.GameState.SurvivorSaved:
                // Show Heart
                StartCoroutine(DisplayHeartsParticle());

                // Hide Survivor
                GetComponent<SpriteRenderer>().enabled = false;

                GameManager.currentState = GameManager.GameState.ReturnToBase;
                break;

            case GameManager.GameState.SendToHospital:
                // Show Survivor
                GetComponent<SpriteRenderer>().enabled = true;
                transform.Translate(-Time.deltaTime, 0, 0);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && GameManager.currentState == GameManager.GameState.RescueOperation)
        {
            // Hide Survivor
            GetComponent<SpriteRenderer>().enabled = false;
            // Show Heart
            StartCoroutine(DisplayHeartsParticle());
            GameManager.currentState = GameManager.GameState.SurvivorSaved;
        }

        if (collision.gameObject.CompareTag("Hospital"))
        {
            // Hide Survivor
            GetComponent<SpriteRenderer>().enabled = false;
            // Show Heart
            StartCoroutine(DisplayHeartsParticle());

            // Restart
            GameManager.currentState = GameManager.GameState.ArriveHospital;
        }
    }

    IEnumerator DisplayHeartsParticle()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        transform.GetChild(0).gameObject.SetActive(false);

        Destroy(this.gameObject);
    }
}
