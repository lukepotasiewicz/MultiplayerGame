using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour {
	public static string IP = "";
    void Start ()
    {
        var input = gameObject.GetComponent<InputField>();
        input.onEndEdit.AddListener(SetIp);
        Debug.Log("hey");
    }

    private void SetIp(string userIP)
    {
        Debug.Log(userIP);
        IP = userIP;
    }
}