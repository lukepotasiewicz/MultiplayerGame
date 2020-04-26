using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTextInput : MonoBehaviour {
    public static string NAME = "";
    void Start ()
    {
        var input = gameObject.GetComponent<InputField>();
        input.onEndEdit.AddListener(SetName);
    }

    private void SetName(string userInfo) {
        NAME = userInfo;
    }
}