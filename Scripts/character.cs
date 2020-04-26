using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using client;
using UnityEngine.UI;
using static IpTextInput;
using static NameTextInput;

public class Character : MonoBehaviour {
    private const string END_CONNECTION = "EC";
    public float speed;
    public string characterName;
    public bool grounded;
    public bool sendingData = false;
    public bool connectionCreated = false;
    public int animState = 0;
    public float health;
    public static float maxHealth;
    public int direction = 1;
    public static string[] blockedCharacters = new string[10] {"", "", "", "", "", "", "", "", "", ""};
    public bool stunned = false;
    private bool moving;
    private bool canAttack = true;

    public GameObject networkCharacter;
    public Text pingText;
    public GameObject heathBar;
    public GameObject nameTextObj;
    public GameObject stunnedIndicator;
    public AudioClip blockSound;
    public Button playButton;
    public GameObject startingCanvas;
    public GameObject startingOverlay;
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
        maxHealth = health;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        nameText = nameTextObj.GetComponent<Text>();
        // play button
        Button playButtonLocal = playButton.GetComponent<Button>();
        playButtonLocal.onClick.AddListener(attemptConnection);
    }

    void attemptConnection() {
        if (!connectionCreated && IP.Length > 0 && NAME.Length > 0) {
            characterName = NAME;
            NetworkClient.Connect(IP);
            connectionCreated = true;
            startingCanvas.transform.localScale = new Vector2(0, 0);
            startingOverlay.transform.localScale = new Vector2(0, 0);
        }
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

        bool wasStunned = stunned; 
        stunned = false;
        // check to see if you are stunned
        foreach (var character in serverData) {
            stunned = stunned || Array.IndexOf(character.Split(','), characterName) > 6;
        }
        // play audio if was just stunned
        if (!wasStunned && stunned) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(blockSound, 0.8f);
        }

        if (animState == 0 || animState == 3 || animState == 4) {
            // block 1
            if (Input.GetKey(KeyCode.LeftShift)) {
                animState = 3;
                anim.SetInteger("animState", 3);
            }
            // block 2
            else if (Input.GetKey(KeyCode.LeftControl)) {
                animState = 4;
                anim.SetInteger("animState", 4);
            }
            else {
                animState = 0;
                anim.SetInteger("animState", 0);
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

            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && grounded && animState != 3) {
                movement.y = 10;
            }

            if (!stunned && canAttack) {
                // attack 1
                if (Input.GetKey(KeyCode.Mouse0) && animState == 0) {
                    anim.SetInteger("animState", 1);
                    animState = 1;
                    canAttack = false;
                    StartCoroutine(delay(() => { animState = 11; }, 0.3f));
                    StartCoroutine(delay(() => {
                        anim.SetInteger("animState", 0);
                        animState = 0;
                    }, 0.4f));
                    StartCoroutine(delay(() => {
                        canAttack = true;
                    }, 0.6f));
                }

                // attack 2
                if (Input.GetKey(KeyCode.Mouse1) && animState == 0) {
                    anim.SetInteger("animState", 2);
                    animState = 2;
                    canAttack = false;
                    StartCoroutine(delay(() => { animState = 22; }, 0.3f));
                    StartCoroutine(delay(() => {
                        anim.SetInteger("animState", 0);
                        animState = 0;
                    }, 0.4f));
                    StartCoroutine(delay(() => {
                        canAttack = true;
                    }, 0.6f));
                }
            }


            // stop walking if doing any other animation
            if (animState != 0) {
                moving = false;
            }

            if (moving) {
                rb2d.velocity = new Vector2(movement.x * speed * (stunned ? 0.6f : 1f), movement.y);
            }

            gameObject.transform.localScale = new Vector2(direction, 1);
        }

        // character is in wind, make them float
        if (gameObject.transform.position.x > 40.8 && gameObject.transform.position.x < 45 && gameObject.transform.position.y < -5) {
            if (movement.y >= 15) {
                movement.y = 15;
            }
            else {
                movement.y += 1;
            }
        }

        if (!moving && grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.8f, movement.y);
        }
        else if (!moving && !grounded) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.96f, movement.y);
        }
        
        
        if (health > maxHealth) {
            health = maxHealth;
        }
        // if character loses all health, or falls off the map
        else if (health <= 0 || gameObject.transform.position.y < -50) {
            gameObject.transform.SetPositionAndRotation(
                new Vector3(0, 0, 0),
                Quaternion.Euler(new Vector3(0, 0, 0))
            );
            health = maxHealth;
        }
        else {
            // health slowly regenerates
            health += 0.001f;
        }
    }

    async void Update() {
        // show or hide stunned indicator
        stunnedIndicator.transform.localScale = new Vector2(stunned ? 1 : 0, 1);

        nameText.text = characterName;
        nameTextObj.transform.position =
            Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(0, 2.1f, 0));

        if (Time.time * 1000 - timeStart > 1000 && connectionCreated) {
            NetworkClient.Send("hack fix");
        }
        
        heathBar.transform.localScale = new Vector2(health / maxHealth, 1);


        if (connectionCreated && !sendingData) {
            var position = gameObject.transform.position;
            var data = characterName + "," + position.x + "," + position.y + "," + rb2d.velocity.x + "," +
                       rb2d.velocity.y + "," + direction + "," + animState + "," + health + "," + createBlockedString();
            sendingData = true;
            await Task.Run(() => sendData(data));

            var ping = Time.time * 1000 - timeStart;
            pingText.text = "Ping: " + Math.Floor(ping);
            timeStart = Time.time * 1000;
        }


        anim.SetBool("walking", moving);
    }

    public static String createBlockedString() {
        string blockedString = "";
        foreach (var character in blockedCharacters) {
            if (character.Length != 0) {
                blockedString += character + "|";
            }
        }

        if (blockedString.Length > 0) {
            return blockedString.Remove(blockedString.Length - 1, 1);
        }
        else {
            return "0";
        }
    }

    public void addBlockedCharacter(String name, float time) {
        int index = Array.IndexOf(blockedCharacters, "");
        blockedCharacters[index] = name;
        StartCoroutine(delay(() => { blockedCharacters[index] = ""; }, time));
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