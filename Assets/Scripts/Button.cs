using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    public Sprite buttonOff;
    public Sprite buttonPressed;
    public Sprite buttonOn;

    private SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
        sr = GetComponent<SpriteRenderer>();	
	}
	
	// Update is called once per frame
	void Update () {
        
        
	}

    private void OnMouseDown() {
        if (sr.sprite == buttonOff) {
            sr.sprite = buttonPressed;
        } else if (sr.sprite == buttonOn) {
            sr.sprite = buttonPressed;
        }
    }

    private void OnMouseUp() {
        if (sr.sprite == buttonPressed) {
            sr.sprite = buttonOn;
        }
    }



}
