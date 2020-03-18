using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using client;
using UnityEngine.UI;
using static TextInput;

public class Character : MonoBehaviour {
    private const string END_CONNECTION = "EC";
    public float speed;
    public string characterName;
    public bool grounded;
    public bool sendingData = false;
    public bool connectionCreated = false;
    public int animState = 0;
    public float health = 3;
    public int blocking = 0;
    private bool moving;
    private int direction = 1;

    public GameObject networkCharacter;
    public Text pingText;
    public GameObject heathBar;
    public GameObject nameTextObj;
    private Text nameText;

    public GameObject[] networkCharacters = new GameObject[10];
    public static string[] serverData = new string[0];
    public int numPlayers = 0;

    public Rigidbody2D rb2d;

    private Animator anim;

    private float timeStart;

    // Use this for initialization
    void Start() {
        timeStart = Time.time;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        nameText = nameTextObj.GetComponent<Text>();
    }

    void OnApplicationQuit() {
        NetworkClient.Send(END_CONNECTION);
        NetworkClient.Close();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (serverData.Length > 0) {
            for (var i = numPlayers; i < serverData.Length; i++) {
                if (serverData[i] != END_CONNECTION) {
                    networkCharacters[i] = Instantiate(networkCharacter, Vector3.zero, Quaternion.identity);
                    networkCharacters[i].GetComponentInParent<NetworkCharacter>().id = i;
                }
            }
            numPlayers = serverData.Length;
        }

        moving = false;
        Vector2 movement = new Vector2(rb2d.velocity.x, rb2d.velocity.y);

        if (animState == 0) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                blocking = 1;
                anim.SetInteger("blocking", 1);
            }
            else {
                blocking = 0;
                anim.SetInteger("blocking", 0);
            }
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

            if (Input.GetKey(KeyCode.Mouse0) && animState != 1 && blocking == 0) {
                anim.SetInteger("animState", 1);
                animState = 1;
                StartCoroutine(delay(() => { animState = 11; }, 0.3f));
                StartCoroutine(delay(() => {
                    anim.SetInteger("animState", 0);
                    animState = 0;
                }, 0.4f));
            }

            if (blocking != 0) {
                moving = false;
            }

            if (moving) {
                rb2d.velocity = new Vector2(movement.x * speed, movement.y);
            }

            gameObject.transform.localScale = new Vector2(direction, 1);
        }

        if (!moving && grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.8f, movement.y);
        }
        else if (!moving && !grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.96f, movement.y);
        }
    }

    async void Update() {
        nameText.text = characterName;
        nameTextObj.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(0, 2.1f, 0));
        
        if (Time.time * 1000 - timeStart > 1000 && connectionCreated) {
            NetworkClient.Send("hack fix");
        }

        if (health > 3) {
            health = 3;
        }
        else if (health <= 0) {
            gameObject.transform.SetPositionAndRotation(
                new Vector3(0, 0, 0),
                Quaternion.Euler(new Vector3(0, 0, 0))
            );
            health = 3;
        }

        heathBar.transform.localScale = new Vector2(health / 3, 1);

        if (!connectionCreated && TextInput.IP.Length > 1) {
            characterName = TextInput.NAME;
            NetworkClient.Connect(TextInput.IP);
            connectionCreated = true;
        }

        if (connectionCreated && !sendingData) {
            var position = gameObject.transform.position;
            var data = characterName + "," + position.x + "," + position.y + "," + rb2d.velocity.x + "," +
                       rb2d.velocity.y + "," + direction + "," + animState + "," + health;
            sendingData = true;
            await Task.Run(() => sendData(data));

            var ping = Time.time * 1000 - timeStart;
            pingText.text = "Ping: " + Math.Floor(ping);
            timeStart = Time.time * 1000;
        }


        anim.SetBool("walking", moving);
    }

    IEnumerator delay(Action func, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        func();
    }


    async Task<int> sendData(string data) {
        // try {
        string response = NetworkClient.Receive();
        if (response.Length > 1) {
            serverData = response.Split(':');
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