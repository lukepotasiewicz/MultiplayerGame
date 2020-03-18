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

    public GameObject heathBar;
    
    public Rigidbody2D rb2d;
    private Animator anim;
    public GameObject nameTextObj;
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
        nameTextObj.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(0, 2.1f, 0));
        direction = Int32.Parse(myData[5]);
        animState = Int32.Parse(myData[6]);
        health = Int32.Parse(myData[7]);
        
        gameObject.transform.SetPositionAndRotation(
            ServerDataToPosition(myData),
            Quaternion.Euler(new Vector3(0, 0, 0))
        );
        
        gameObject.transform.localScale = new Vector2(direction, 1);

        if (animState < 10) {
            anim.SetInteger("animState", animState);
        }
        anim.SetBool("walking", Math.Abs(ServerDataToVelocity(myData).x) > 0.1);
        rb2d.velocity = ServerDataToVelocity(myData);
        gameObject.GetComponentInChildren<attack>().animState = animState;
        heathBar.transform.localScale = new Vector2( health / 3, 1);
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