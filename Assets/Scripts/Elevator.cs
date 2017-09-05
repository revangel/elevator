using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Notes
/// (1)
// destinations is a SortedList containing the floors to service in order
// Requests to floors > the current floor are added to destinations 
// and are automatically sorted by definition of SortedList
// The head of destinations (destinations.GetByIndex(0)) represents the next floor
// A Queue was originally considered but was rejected due to the inability to 
// enqueue at the head of the queue which is necessary for scenarios such as the one below:
// e.g. Consider and elevator going up from floor 1
// user1 requests to go to floor 9 - this is now at the head of destinations.
// At floor 2, user2 boards and requests floor 8. Since it doesn't make sense
// skip floor 8 on the way to floor 9, it should be possible to change the next
// floor to service to 8. Adding the floor to destinations automatically handles
// where to place floor 8.

/// (2)
// This array has the child Buttons in the order that they appear in the scene hierarchy
// So if the hierarchy was:
// - x
// -- foo
// -- bar
// -- gamma
// then buttons := [foo, bar, gamma]

/// Definitions
// A CALL to a floor is when an up/down button is pressed on the 
// outside of the elevator (i.e. someone wants to get on the elevator).
// A floor REQUEST is when a floor button is pressed on the 
// inside of the elevator (i.e. someone wants to go to floor f)
// SERVICING a floor means responding to either a CALL or REQUEST
public class Elevator : MonoBehaviour {
    public static Elevator instance = null;

    public Text floorDisplay;
    public Image upArrow;
    public Image downArrow;

    public float distanceBetweenFloors = 10f;
    public float floorWaitTime = 5f;
    public int floors = 10;
     
    private enum Directions {down, up, both, none};
    private List<Directions> calls;
    private SortedList upDestinations; /// See Notes (1)
    private SortedList downDestinations;

    
    private Directions direction;
    private float distanceTravelledBetweenFloors;    
    private int floor;
    private int targetFloor;

    private bool inBetweenFloors;
    private bool doorIsOpen;

    private Button[] buttons;

    private AudioSource chime;

    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance == this) {
            Destroy(gameObject);
        }

        calls = new List<Directions>();
        upDestinations = new SortedList();
        downDestinations = new SortedList();
        
        direction = Directions.none;
        distanceTravelledBetweenFloors = 0;        
        floor = 0;
        targetFloor = -1;

        doorIsOpen = false;
        inBetweenFloors = false;

        chime = gameObject.GetComponent<AudioSource>();
	}

    private void Start() {
        buttons = GetComponentsInChildren<Button>(); /// See Notes(2)
   
        UpdateFloorDisplay();
        UpdateDirection();
        UpdateDirectionDisplay();
    }

    // Use FixedUpdate for consistent intervals between calls 
    /// If whiles are used, then all the decrementing would occur in one frame
    void Update () {
        if(floor != targetFloor && targetFloor != -1) {
            if (!inBetweenFloors && !doorIsOpen) { // Prevent repeated calls to MoveToNextFloor() while moving between floors
                inBetweenFloors = true;
                UpdateDirection();
                StartCoroutine(MoveToNextFloor());
            }            
        }
    }

    //////////////////
    // Coroutines
    IEnumerator MoveToNextFloor() {
        Debug.Log("Moving to next floor from floor " + floor);
        yield return new WaitForSeconds(distanceBetweenFloors);
        ArriveAtFloor();
    }

    /// (a) Everything called after StartCoroutine() will execute immediately after StartCoroutine
    /// So in order to actually execute statements after waiting, those statements have to be
    /// called within the coroutine itself
    private void ArriveAtFloor() {
        inBetweenFloors = false;        
        if (direction == Directions.up) {
            floor++;
        } else if (direction == Directions.down) {
            floor--;
        }
        Debug.Log("Now at floor " + floor);
        if (floor == GetNextFloor()) {
            doorIsOpen = true;
            StartCoroutine(WaitAtFloor());
        }        
    }

    IEnumerator WaitAtFloor() {
        Debug.Log("Waiting at floor " + floor);
        yield return new WaitForSeconds(floorWaitTime);
        doorIsOpen = false;
        Debug.Log("Finished waiting at floor " + floor);
        Debug.Log("At target floor?: " + (floor == targetFloor));
    }


    //////////////////
    // Elevator Logic

    public void HandleFloorRequest(int f) {
        if (f >= 0 && f < floors) {
            if (direction != Directions.none) {
                AddFloorRequest(f);
            } else if (direction == Directions.none) {
                StartTravel(f);
            }
        }
        UpdateTargetFloor();
        Debug.Log("New target floor: " + targetFloor);
    }

    // Begins a new destinations queue
    private void StartTravel(int f) {
        if (f > floor) {
            upDestinations.Add(f, f);
        } else if (f < floor) {
            downDestinations.Add(f, f);
        }
    }

    // Adds a floor request to existing destinations if not already in
    private void AddFloorRequest(int f) {
        // When going up, add any reqests for floors > current floor and any calls for upward travel
        // to the upward destinations. 
        // For any requests for floors < current floor and calls for downward travel, 
        // add to opposite destinations
        // Vice versa for downward travel
        if (direction == Directions.up) { 
            if (f > floor && !upDestinations.Contains(f)) { 
                upDestinations.Add(f, f);
            } else if (!downDestinations.Contains(f)) { // Request is in the opposite direction
                downDestinations.Add(f, f);
            }
        } else if (direction == Directions.down) {
            if (f < floor && !downDestinations.Contains(f)) {
                downDestinations.Add(f, f);
            } else if (!upDestinations.Contains(f)) { // Request is in the opposite direction
                upDestinations.Add(f, f);
            }
        } 
    }

    // Updates the target floor, which is the furthest floor in the direction of travel
    // i.e. max(upDestinations), min(downDestinations)
    // If there is no target floor (i.e. destinations are empty), the target floor == -1
    private void UpdateTargetFloor() {
        if (floor == targetFloor) {
            targetFloor = -1;
        } else if (targetFloor == -1) { // Target floor initialized
            if (upDestinations.Count > 0) {
                targetFloor = (int)upDestinations.GetByIndex(upDestinations.Count - 1);
            } else if (downDestinations.Count > 0) {
                targetFloor = (int)downDestinations.GetByIndex(0);
            }
        } else if (direction == Directions.up && upDestinations.Count > 0) { // A floor higher than current up target 
            targetFloor = (int)upDestinations.GetByIndex(upDestinations.Count - 1);
        } else if (direction == Directions.down && downDestinations.Count > 0) { // A floor lower than current down target
            targetFloor = (int)downDestinations.GetByIndex(0);
        }
    }

    // Updates direction based on targetFloor
    private void UpdateDirection() {
        if (targetFloor < 0) {
            direction = Directions.none;
        } else {
            if (targetFloor < floor) {
                direction = Directions.down;
            } else if (targetFloor > floor) {
                direction = Directions.up;
            }
        }
    }

    // Returns the next floor to stop to along the direction of travel
    // Returns -1 if there are no floors in any of the queues
    /// (a) Since downDestinations is a SortedList, floors are in ascending order
    /// but since the elevator is going down, should stop by floors in descending order
    private int GetNextFloor() {
        if (direction == Directions.up && upDestinations.Count > 0) {            
            return (int)upDestinations.GetByIndex(0);
        }
        else if (direction == Directions.down && downDestinations.Count > 0) {
            return (int)downDestinations.GetByIndex(downDestinations.Count-1); /// See (a)
        }
        return -1;
    }

    private void RemoveCurrentFloorFromDestinations() {
        if (direction == Directions.up) {
            upDestinations.Remove(floor);
        } else if (direction == Directions.down) {
            downDestinations.Remove(floor);
        }
    } 

    ////////////////////
    // Display Functions

    private void UpdateDirectionDisplay() {
        if (direction == Directions.up) {
            upArrow.enabled = true;
            downArrow.enabled = false;
        } else if (direction == Directions.down) {
            downArrow.enabled = true;
            upArrow.enabled = false;
        } else if (direction == Directions.none) {
            upArrow.enabled = false;
            downArrow.enabled = false;
        }
    }

    // Changes the floor display text to the current floor
    private void UpdateFloorDisplay() {
        if (floor == 0) {
            floorDisplay.text = "G";
        } else {
            floorDisplay.text = floor.ToString();
        }
    }

    // Turns off light for the button for floor i
    private void TurnButtonLightOff(int i) {
        buttons[i].TurnLightOff();
    }
}
