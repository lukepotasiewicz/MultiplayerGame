using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour {

    public bool canHit = true;
    public int animState = 0;
    public int direction = 0;
    public AudioClip block1;
    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && canHit && animState == 11) {
            // true if player isn't blocking
            if (other.GetComponentInParent<Character>().animState != 3) {
                // true if players are facing each other
                if (other.GetComponentInParent<Character>().direction != direction) {
                    // blocked
                    gameObject.GetComponent<AudioSource>().PlayOneShot(block1, 0.8f);
                }
                else {
                    // hit
                    other.GetComponentInParent<Character>().health -= 1;
                }
            }
            canHit = false;
            StartCoroutine(damageWait());
        }
    }
    
    IEnumerator damageWait() {
        yield return new WaitForSeconds(0.3f);
        canHit = true;
    }
}
