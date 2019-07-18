# RacerAI
Artificial Intelligence Suite for Race Cars built in C#/Unity

## Universal Car AI
Base methods shared by PoliceAI, CivilianAI, and RacerAI
### Notable Universal Car AI Methods
#### protected virtual void Start():
Method sets center of mass for car physics and initializes the path of waypoints procedurally generated on game start. Calls the waitForStart() method as an activation delay before any AIs on the map are able to start their update cycles.
#### protected IEnumerator waitForStart():
Method waits for a configurable amount of seconds before activating the AI, utilized in waiting for a race to begin before AIs gain the ability to move around.
#### protected void deactivateAI():
Method shuts off the AI by setting its maximum speed to zero and forces the brakes when the AI reaches the finish line.
protected void SteerCarToAngle(float speedModifier):
Method controls what direction the wheels turn in based on the target steering angle and current steering angle at a rate controlled by our turn speed set for the car. 
#### protected virtual void Steer():
Method controls the car’s pathfinding towards each node in the race path created in Start().
If the car is avoiding something, then this method is ignored during the FixedUpdate().

### Police Car AI
