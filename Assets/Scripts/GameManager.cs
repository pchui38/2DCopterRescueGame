using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject copterPrefab;
    public GameObject explosionPrefab;
    public GameObject survivorPrefab;
    public Transform helipad;
    public Transform house;
    public Transform hospital;
    public Camera copterCamera;

    private GameObject copterPlayer;
    private GameObject copterExplosion;
    private GameObject survivor;

    private AudioSource audioSource;

    public enum GameState
    {
        RescueOperation,
        SurvivorSaved,
        ReturnToBase,
        SurvivorDispatch,
        SendToHospital,
        ArriveHospital,
        CopterDestroyed,
        GameOver,
        Idle,
        GameRestart
    }

    public static GameState currentState;

    void Start()
    {
        // Initialise Game state
        currentState = GameState.RescueOperation;

        // Spawn Helicopter
        copterPlayer = Instantiate(copterPrefab);

        // Randomise House location X
        int posX = Random.Range(-60, 60);
        posX = posX > 0 ? posX + 10 : posX - 10;
        house.position = new Vector3(posX, house.position.y, house.position.z);

        // Spawn Survivor
        survivor = Instantiate(survivorPrefab, house.transform.position, Quaternion.identity);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (copterPlayer != null)
        {
            // Camera follow player copter
            copterCamera.transform.position = new Vector3(copterPlayer.transform.position.x, copterCamera.transform.position.y, copterCamera.transform.position.z);
        }

        switch (currentState)
        {
            case GameState.CopterDestroyed:
                StartCoroutine(DestroyAllGameObjects(true));
                break;

            case GameState.ReturnToBase:
                // Do nothing
                break;

            case GameState.SurvivorDispatch:
                // Spawn Survivor
                survivor = Instantiate(survivorPrefab, helipad.transform.position, Quaternion.identity);
                currentState = GameState.SendToHospital;
                break;

            case GameState.SendToHospital:
                // Do nothing
                break;

            case GameState.ArriveHospital:
                // Play Cheer sound
                audioSource.Play();
                currentState = GameState.GameOver;
                break;

            case GameState.GameRestart:
                RespawnGameObjects();
                break;

            case GameState.GameOver:
                StartCoroutine(DestroyAllGameObjects(false));
                currentState = GameState.Idle;
                break;
        }
    }
    private void RespawnGameObjects()
    {
        // Restart by Spawn Helicopter, Survivor
        copterPlayer = Instantiate(copterPrefab);
        int posX = Random.Range(-60, 60);
        posX = posX > 0 ? posX + 10 : posX - 10;
        house.position = new Vector3(posX, house.position.y, house.position.z);
        survivor = Instantiate(survivorPrefab, house.transform.position, Quaternion.identity);
        // Reset GameState to Rescue Operation
        currentState = GameState.RescueOperation;
    }

    IEnumerator DestroyAllGameObjects(bool isExplosion)
    {
        currentState = GameState.Idle;

        if (isExplosion)
        {
            copterExplosion = Instantiate(explosionPrefab, copterPlayer.transform.position, Quaternion.identity);
        }
        Destroy(copterPlayer);

        yield return new WaitForSeconds(1);

        if (isExplosion)
           Destroy(copterExplosion);

        Destroy(survivor);

        yield return new WaitForSeconds(2);
        currentState = GameState.GameRestart;
    }

    IEnumerator DestroySurvivor()
    {
        yield return new WaitForSeconds(2);
        Destroy(survivor);
    }
}
