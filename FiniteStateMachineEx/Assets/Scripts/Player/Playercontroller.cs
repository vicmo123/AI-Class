using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    enum keysAction{
        up,
        down,
        right,
        left,
        none
    }

    private GameObject playerPrefab;
    public Transform spawnLoc;

    private Transform playerInstance;

    public float movingSpeed = 3f;

    public void Initialize()
    {
        //ToDo create and set prefab
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
    }

    public void SecondInitialize()
    {
        Spawnplayer(spawnLoc);
    }
    

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        DoSelectedAction(KeyMananger());
    }

    private void Spawnplayer(Transform loc)
    {
        GameObject playerInstance = GameObject.Instantiate(playerPrefab, loc.position, Quaternion.identity);
        this.playerInstance = playerInstance.transform;
    }

    private keysAction KeyMananger()
    {
        if(Input.GetKey(KeyCode.W))
            return keysAction.up;
        if (Input.GetKey(KeyCode.S))
            return keysAction.down;
        if (Input.GetKey(KeyCode.A))
            return keysAction.left;
        if (Input.GetKey(KeyCode.D))
            return keysAction.right;
        else
            return keysAction.none;
    }

    private void DoSelectedAction(keysAction desiredAction)
    {
        switch (desiredAction)
        {
            //Todo fill each case
            case keysAction.up:
                playerInstance.transform.eulerAngles = new Vector3(0, 0, 0);
                playerInstance.position = (Vector2)playerInstance.position + new Vector2(0, 1) * movingSpeed * Time.deltaTime;
                break;
            case keysAction.down:
                playerInstance.transform.eulerAngles = new Vector3(0, 0, 180);
                playerInstance.position = (Vector2)playerInstance.position + new Vector2(0, -1) * movingSpeed * Time.deltaTime;
                break;
            case keysAction.right:
                playerInstance.transform.eulerAngles = new Vector3(0, 0, 270);
                playerInstance.position = (Vector2)playerInstance.position + new Vector2(1, 0) * movingSpeed * Time.deltaTime;
                break;
            case keysAction.left:
                playerInstance.transform.eulerAngles = new Vector3(0, 0, 90);
                playerInstance.position = (Vector2)playerInstance.position + new Vector2(-1, 0) * movingSpeed * Time.deltaTime;
                break;
            case keysAction.none:
                playerInstance.transform.eulerAngles = new Vector3(0, 0, playerInstance.transform.eulerAngles.z);
                break;
            default:
                Debug.Log("Unhandeled switch: " + desiredAction);
                break;
        }
    }
}
