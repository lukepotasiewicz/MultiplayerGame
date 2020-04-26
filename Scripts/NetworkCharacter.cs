using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCharacter : MonoBehaviour {
    public int id;
    public int direction = 1;
    public float health = 3;
    public int animState = 0;
    public string name;
    public bool stunned = false;

    private float previousHealth;

    public GameObject heathBar;
    
    public Rigidbody2D rb2d;
    private Animator anim;
    public GameObject nameTextObj;
    public GameObject stunnedIndicator;
    public AudioClip blockSound;
    public AudioClip damageSound;
    private Text nameText;

    // Start is called before the first frame update
    void Start() {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        nameText = nameTextObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        string[] myData = Character.serverData[id].Split(',');
        nameText.text = myData[0];
        name = myData[0];
        nameTextObj.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(0, 2.1f, 0));
        direction = Int32.Parse(myData[5]);
        animState = Int32.Parse(myData[6]);
        health = float.Parse(myData[7]);
        var blockedCharacters = myData[8];
        
        bool wasStunned = stunned;
        stunned = false;
        // check if this network character was recently blocked by another client
        foreach (var character in Character.serverData) {
            stunned = stunned || Array.IndexOf(character.Split(','), name) > 6;
        }
        // check if this network character was recently blocked by this client
        stunned = stunned || Array.IndexOf(Character.createBlockedString().Split('|'), name) != -1;
        // show or hide stunned indicator
        stunnedIndicator.transform.localScale = new Vector2(stunned ? 1 : 0, 1);
        
        // play audio if was just stunned
        if (!wasStunned && stunned) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(blockSound, 0.8f);
        }
        
        // play audio if damage was taken
        if (previousHealth > health + 0.5f) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(damageSound, 0.8f);
        }
        previousHealth = health;

        gameObject.transform.SetPositionAndRotation(
            ServerDataToPosition(myData),
            Quaternion.Euler(new Vector3(0, 0, 0))
        );
        
        gameObject.transform.localScale = new Vector2(direction, 1);

        if (animState < 10) {
            anim.SetInteger("animState", animState);
        }
        anim.SetBool("walking", Math.Abs(ServerDataToVelocity(myData).x) > 1);
        rb2d.velocity = ServerDataToVelocity(myData);
        heathBar.transform.localScale = new Vector2( health / Character.maxHealth, 1);
    }

    public static Vector2 ServerDataToPosition(string[] sArray) {
        Vector2 result = new Vector2(
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    public static Vector2 ServerDataToVelocity(string[] sArray) {

        Vector2 result = new Vector2(
            float.Parse(sArray[3]),
            float.Parse(sArray[4]));

        return result;
    }
}