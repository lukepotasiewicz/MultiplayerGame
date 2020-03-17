using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour {
    public int id;
    public int direction = 1;
    public float health = 3;
    public int animState = 0;

    public GameObject heathBar;
    
    private Animator anim;

    // Start is called before the first frame update
    void Start() {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        string[] myData = Character.serverData[id].Split(',');
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