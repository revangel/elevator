﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    public int floor;
    public bool hasLightOn;

    public Sprite buttonOff;
    public Sprite buttonPressed;
    public Sprite buttonOn;

    private GameObject button;
    private SpriteRenderer sr;


	void Awake () {
        button = this.gameObject;
        sr = button.GetComponent<SpriteRenderer>();
        sr.sprite = buttonOff;
        hasLightOn = false;
	}

    private void LateUpdate() {
        if (hasLightOn) {
            sr.sprite = buttonOn;
        } else {
            sr.sprite = buttonOff;
        }
    }

    private void OnMouseDown() {
        sr.sprite = buttonPressed;
        hasLightOn = true;
        Elevator.instance.HandleFloorRequest(floor);
    }

    private void OnMouseUp() {
        sr.sprite = buttonOn;
    }

    public void TurnLightOn() {
        hasLightOn = true;
    }

    public void TurnLightOff() {
        hasLightOn = false;
    }
}
