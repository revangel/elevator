using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

    // Definitions
    // A CALL to a floor is when an up/down button is pressed on the 
    // outside of the elevator (i.e. someone wants to get on the elevator)

    // A floor REQUEST is when a floor button is pressed on the 
    // inside of the elevator (i.e. someone wants to go to floor __ )

    // SERVICING a floor means responding to either a CALL or REQUEST

    public int floors;
    public float delayPerFloor = 1f;
     
    private enum Directions {down, up, none};
    private List<Directions> calls;
    private List<bool> floorsToService;
    private int highestFloorToService;
    private int lowestFloorToService;

    private bool isDoorOpen;
    private int floor;

    // Use this for initialization
    void Awake () {
        floor = 0;
        calls = new List<Directions>();
        floorsToService = new List<bool>();
        initializeLists(floors);
        isDoorOpen = false;
        Directions direction = Directions.none;
	}
	
	// Update is called once per frame
	void Update () {

	}

    // Initializes call and request lists to default 
    // (null equivalent) values based on the number of floors
    private void initializeLists(int floors) {
        for (int i = 0; i < floors; i++) {
            calls.Add(Directions.up);
            floorsToService.Add(false);
        }
    }

    // Adds call to the calls list
    private void addToCalls(int floor, Directions direction) {
        if (floor >= 0 && floor < floors) {
            
        }
    }

    // Adds request to the floor service list
    private void addToFloorsToService(int floor) {
        if (floor >= 0 && floor < floors) {
            if (!floorsToService[floor]) {
                floorsToService[floor] = true;
            }
        }
    }
}
