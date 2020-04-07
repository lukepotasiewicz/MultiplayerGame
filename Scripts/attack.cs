using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour {

    public int blockingState;
    public int attackingState;
    
    public bool canHit = true;
    public AudioClip blockSound;

    void OnTriggerStay2D(Collider2D other) {
        var parent = transform.parent.gameObject.GetComponent<NetworkCharacter>();
        int animState = parent.animState;
        int direction = parent.direction;
        if (other.gameObject.tag == "Player" && canHit && animState == attackingState) {
            // true if player isn't blocking
            if (other.GetComponentInParent<Character>().animState == blockingState) {
                // true if players are facing each other
                if (other.GetComponentInParent<Character>().direction != direction) {
                    // blocked
                    gameObject.GetComponent<AudioSource>().PlayOneShot(blockSound, 0.8f);
                    other.GetComponentInParent<Character>().addBlockedCharacter(parent.name, 1.0f);
                }
                else {
                    // hit
                    other.GetComponentInParent<Character>().health -= 1;
                }
            } else {
                // hit
                other.GetComponentInParent<Character>().health -= 1;
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
