using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour {

    public bool canHit = true;
    public int animState = 0;
    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && canHit && animState == 11) {
            other.GetComponentInParent<Character>().health -= 1;
            canHit = false;
            StartCoroutine(damageWait());
        }
    }
    
    IEnumerator damageWait() {
        yield return new WaitForSeconds(0.3f);
        canHit = true;
    }
}
