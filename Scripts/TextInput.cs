using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour {
	public static string IP = "";
    public static string NAME = "";
    void Start ()
    {
        var input = gameObject.GetComponent<InputField>();
        input.onEndEdit.AddListener(SetIp);
    }

    private void SetIp(string userInfo) {
        IP = userInfo.Split(',')[0];
        NAME = userInfo.Split(',')[1];
    }
}