using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IpTextInput : MonoBehaviour {
	public static string IP = "";
    void Start ()
    {
        var input = gameObject.GetComponent<InputField>();
        input.onEndEdit.AddListener(SetIp);
    }

    private void SetIp(string userInfo) {
        IP = userInfo;
    }
}