using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviour {
    public int id;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        string myData = Character.serverData[id];
        gameObject.transform.SetPositionAndRotation(
            ServerDataToPosition(myData),
            Quaternion.Euler(new Vector3(0, 0, 0))
        );
        if (ServerDataToVelocity(myData).x > 0) {
            gameObject.transform.localScale = new Vector2(1, 1);
        }
        else {
            gameObject.transform.localScale = new Vector2(-1, 1);
        }
    }

    public static Vector2 ServerDataToPosition(string sVector) {
        string[] sArray = sVector.Split(',');

        Vector2 result = new Vector2(
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    public static Vector2 ServerDataToVelocity(string sVector) {
        string[] sArray = sVector.Split(',');

        Vector2 result = new Vector2(
            float.Parse(sArray[3]),
            float.Parse(sArray[4]));

        return result;
    }
}