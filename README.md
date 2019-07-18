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
#### protected void Sensors():
Method controls the car's spatial awareness. Five raycast sensors exist on our car. Left, Angled Left, Front, Angled Right, and Right. If a sensor is hit on a given side, the avoiding boolean flag is set to true which allows for this method’s avoidMultiplier to take precedence over Drive()’s steering. avoidMultiplier’s value corresponds to a left turn if negative and a right turn if positive.
#### protected virtual void Steer():
Method controls the car’s pathfinding towards each node in the race path created in Start().
If the car is avoiding something, then this method is ignored during the FixedUpdate().

### Usage
AI scripts inherit from the Universal Car AI. By default, the Universal Car AI navigation follows a set path of nodes laid out in the game world. Custom applications may of course get rid of that default navigation though! The sensor suite is the primary method that makes the Universal Car AI important for car logic, as it gives the car collision detection whether or not they're following a path or not and lets the car react to its environment.
