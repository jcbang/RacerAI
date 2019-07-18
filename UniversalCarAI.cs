using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UniversalCarAI : MonoBehaviour
{
    /// <summary>
    /// UniversalCarAI that serves as the main template all extended classes can tweak.
	/// Includes all basic inherited navigational capabilities cars share, namely the Sensors method.
    /// </summary>

    public Transform path;

    public float maxSteerAngle = 60f;
    public float turnSpeed = 30f;

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    public float maxMotorTorque = 500f;
    public float maxBrakeTorque = 300f;

    public float currentSpeed;
    public float maxSpeed = 400f;

    public Vector3 centerOfMass;

    public bool isBraking = false;
	public bool isSlowing = false;

    [Header("Sensors")]
    public float sensorLength = 10f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.0f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;
	public float detectionRange = 10f;
	
	[Header("Pathfinding")]
	protected List<Transform> nodes;
    protected int currentNode = 0;

    public bool avoiding = false;
    protected float targetSteerAngle = 0;

	// Modifiers
	protected float temp = 0;
	protected float slowSpeed = 10f;
	
	protected float timer;
	protected int secondsPassed;
	public int timeToStart = 3; // Measured in seconds
	protected bool activate;
	protected bool deactivate;
	protected bool end = false;

	protected virtual void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = centerOfMass;

		Transform[] racePath = path.GetComponentsInChildren<Transform>();
		nodes = new List<Transform>();

		for (int i = 0; i < racePath.Length; i++)
		{
			if (racePath[i] != path.transform)
			{
				nodes.Add(racePath[i]);
			}
		}

		StartCoroutine(waitForStart());
	}

	// Shut down the AI permanently
	protected void deactivateAI()
	{
		wheelFL.motorTorque = 0;
		wheelFR.motorTorque = 0;

		isBraking = true;
		Brake();
	}

	protected IEnumerator waitForStart()
	{
		yield return new WaitForSeconds(timeToStart);

		isBraking = false;
		activate = true;
	}

	// Front, Left, and Right sensors give the racer spatial awareness in interacting with
	// the environment and other cars in its way
    protected void Sensors()
	{
        RaycastHit hit;

        Vector3 sensorStartPos = transform.position;

        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

		// Controls steering left / right upon exit of this method,
		// with negative avoidMultipliers giving sharper left turns,
		// and positive avoidMultipliers giving sharper right turns
        float avoidMultiplier = 0;

        avoiding = false;

        //front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
		{
			if (!hit.collider.CompareTag("Intersection"))
			{
				Debug.DrawLine(sensorStartPos, hit.point);
				avoiding = true;
				avoidMultiplier -= 1f;
			}
		}

        //front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
		{
			if (!hit.collider.CompareTag("Intersection"))
			{
				Debug.DrawLine(sensorStartPos, hit.point);
				avoiding = true;
				avoidMultiplier -= 0.5f;
			}
		}

        //front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
		if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
		{
			if (!hit.collider.CompareTag("Intersection"))
			{
				Debug.DrawLine(sensorStartPos, hit.point);
				avoiding = true;
				avoidMultiplier += 1f;
			}
		}

		//front left angle sensor
		else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
		{
			// If we hit anything that's not a waypoint
            if (!hit.collider.CompareTag("Intersection"))
			{
				Debug.DrawLine(sensorStartPos, hit.point);
				avoiding = true;
				avoidMultiplier += 0.5f;

            }
        }

        //front center sensor
        if (avoidMultiplier == 0)
		{
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
			{
                if (!hit.collider.CompareTag("Intersection"))
				{
					Debug.DrawLine(sensorStartPos, hit.point);
					avoiding = true;
					if (hit.normal.x < 0)
					{
						avoidMultiplier = -1;
					}
					else
					{
						avoidMultiplier = 1;
					}
                }
            }
        }

        if (avoiding)
		{
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

	// Gets location of the next node in the list and heads towards it
    protected virtual void Steer()
	{
        if (avoiding)
		{
			return;
		}


        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);

        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;
    }

	protected virtual void Drive(float speedModifier)
	{
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

		// Slow down when arriving at intersections
		if (nodes[currentNode].gameObject.tag == "IntersectNode")
		{
			if (!isSlowing)
			{
				temp = maxSpeed;
				maxSpeed = slowSpeed * speedModifier;
			}

			GetComponent<Rigidbody>().velocity = (transform.forward * maxSpeed);
			isSlowing = true;
		}
		else
		{
			if (isSlowing) maxSpeed = temp;

			isSlowing = false;
		}

		if (currentSpeed < (maxSpeed * speedModifier) && !isBraking && !isSlowing)
		{
            wheelFL.motorTorque = maxMotorTorque * speedModifier;
            wheelFR.motorTorque = maxMotorTorque * speedModifier;
        }
		else
		{
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

	// Checks to see if the node is close enough to be flagged as visited
    protected void CheckNodeDistance()
	{
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < detectionRange)
		{
			Debug.DrawLine(transform.position, nodes[currentNode].position);
			Debug.Log(currentNode);

			if (currentNode == nodes.Count - 1)
			{
				activate = false;
				end = true;
			}
			else
			{
                currentNode++;
            }
        }
    }

    protected void Brake()
	{
        if (isBraking)
		{
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
		else
		{
			wheelRL.brakeTorque = 0;
			wheelRR.brakeTorque = 0;
		}
    }

	// Handles fluid movement for car turning
    protected void SteerCarToAngle(float speedModifier)
	{
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed * speedModifier);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed * speedModifier);
    }
}
