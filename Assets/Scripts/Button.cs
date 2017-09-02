using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    public int floor;
    public bool wasPressed;

    public Sprite buttonOff;
    public Sprite buttonPressed;
    public Sprite buttonOn;

    private GameObject button;
    private SpriteRenderer sr;


	void Awake () {
        button = this.gameObject;
        sr = button.GetComponent<SpriteRenderer>();
        sr.sprite = buttonOff;
        wasPressed = false;
	}

    private void OnMouseDown() {
        sr.sprite = buttonPressed;
        wasPressed = true;
        Elevator.instance.HandleFloorRequest(floor);
    }

    private void OnMouseUp() {
        sr.sprite = buttonOn;
    }

    public void ResetButton() {
        sr.sprite = buttonOff;
    }


}
