using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using client;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    public float speed;
    public string characterName;

    public bool grounded;
    private bool moving;
    private int direction = 1;
    public bool sendingData = false;

    public GameObject networkCharacter;
    public Text pingText;

    public GameObject[] networkCharacters = new GameObject[10];
    public static string[] serverData;
    public int numPlayers = 0;

    public Rigidbody2D rb2d;

    private Animator anim;

    private float timeStart;

    // Use this for initialization
    void Start() {
        timeStart = Time.time;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        moving = false;
        NetworkClient.Connect("73.219.102.187");
    }
    
    void OnApplicationQuit()
    {
        NetworkClient.Close();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    // Update is called once per frame
    void FixedUpdate() {

        moving = false;
        Vector2 movement = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
        if (Input.GetKey(KeyCode.A)) {
            direction = -1;
            movement.x = -1;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.D)) {
            direction = 1;
            movement.x = 1;
            moving = true;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && grounded) {
            movement.y = 10;
        }

        if (!moving && grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.8f, movement.y);
        }
        else if (!moving && !grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.86f, movement.y);
        }

        if (moving) {
            rb2d.velocity = new Vector2(movement.x * speed, movement.y);
        }

        gameObject.transform.localScale = new Vector2(direction, 1);
    }

    async void Update() {
        if (!sendingData) {
            var position = gameObject.transform.position;
            var data = characterName + "," + position.x + "," + position.y + "," + rb2d.velocity.x + "," + rb2d.velocity.y;
            sendingData = true;
            await Task.Run(() => sendData(data));
            
            var ping = Time.time * 1000 - timeStart;
            pingText.text = "Ping: " + Math.Floor(ping);
            timeStart = Time.time * 1000;
            
        }
        anim.SetBool("walking", moving);
    }
    
    
    async Task<int> sendData(string data) {
        // try {
            string response = NetworkClient.Receive();
            if (response.Length > 1) {
                serverData = response.Split(':');
                for(var i = numPlayers; i < serverData.Length; i++) {
                    networkCharacters[i] = Instantiate (networkCharacter, Vector3.zero, Quaternion.identity);
                    networkCharacters[i].GetComponentInParent<NetworkCharacter> ().id = i;
                }
                numPlayers = serverData.Length;
            }
            NetworkClient.Send(data);
            sendingData = false;
        // }
        // catch (Exception e) {
        //     Debug.Log(e);
        // }
        return 0;
    }
}