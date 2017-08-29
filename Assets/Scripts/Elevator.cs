using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

/// Definitions
// A CALL to a floor is when an up/down button is pressed on the 
// outside of the elevator (i.e. someone wants to get on the elevator).
// A floor REQUEST is when a floor button is pressed on the 
// inside of the elevator (i.e. someone wants to go to floor f)
// SERVICING a floor means responding to either a CALL or REQUEST
public class Elevator : MonoBehaviour {
    
    public int floors = 10;
    public float delayPerFloor = 1f;
     
    private enum Directions {down, up, both, none};
    private List<Directions> calls;
    private SortedList upDestinations; // See Notes (1)
    private SortedList downDestinations;

    private bool isDoorOpen;
    private int floor;
    private Directions direction;

    // Use this for initialization
    void Awake () {
        floor = 0;
        calls = new List<Directions>();
        upDestinations = new SortedList();
        downDestinations = new SortedList();
        isDoorOpen = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    // Adds a floor request to queue
    private void addFloorRequest(int f) {
        if (f > 0 && f < floors) {
            if (direction == Directions.up && !upDestinations.Contains(f)) { // Don't add duplicate floors
                if (floor < f) { 
                    upDestinations.Add(f, f);
                } else if (!downDestinations.Contains(f)) { // Request is in the opposite direction
                    downDestinations.Add(f, f);
                }
            } else if (direction == Directions.down) {
                if (floor > f) {
                    downDestinations.Add(f, f);
                } else if (!upDestinations.Contains(f)) { // Request is in the opposite direction
                    upDestinations.Add(f, f);
                }
            }
        }
    }
}
