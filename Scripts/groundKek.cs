﻿using UnityEngine;
using System.Collections;

public class groundKek : MonoBehaviour {

	void OnTriggerStay2D (Collider2D other){
		if (other.gameObject.tag == "ground") {
			gameObject.GetComponentInParent<character> ().grounded = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "ground") {
			gameObject.GetComponentInParent<character> ().grounded = false;
		} 
	}
}
