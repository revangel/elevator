# Elevator
## Overview
Inspired by an interview question that asked how one would design an elevator, **Elevator** is a small Unity app that implements one with a GUI. The logic is straightforward:

- At any given time, the elevator is either:
  - (a) stopped with no floor requests, or
  - (b) servicing floors towards a given direction.
- If in (b), the elevator continues in the same direction, stopping on floors that were requested along the way until it reaches the furthest requested floor in that direction (the *destination floor*).
- Once the elevator reaches the destination floor, it resets its direction, immediately reversing it if there were any floors requested the other way.

This algorithm is literally called the [Elevator algorithm](https://en.wikipedia.org/wiki/Elevator_algorithm) and is also used by read/write heads in disk-based hard drives.

![elevator screencap](Docs/elevator_screencap.gif)

## Current Functionality
- Implements the Elevator algorithm
- Indicators for current floor and direction
- Door chime and timed delay whenever the elevator stops on a floor

## Potential Enhancements
- Door:
  - Open/close animations
  - Open/close button
    - Open button delays elevator algorithm until it is released
- Internal vs. External Requests (requires AI):
  - An *internal request* is a floor request made within the elevator
  - An *external request* is a floor request made by pressing an up or down button outside on a given floor
  - The algorithm would be modified to allow stopping on floors that issued an external request for the same direction the elevator is travelling in
  - External requests in the opposite direction would be ignored. If the elevator stops on that floor because of an internal request, then the external request would still be active until the elevator switches direction
- AI:
  - Randomized internal/external floor requests
