using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoliceAI : UniversalCarAI
{
	/// <summary>
	/// PoliceAI extends UniversalCarAI, overriding navigation methods to lock onto players
	/// rather than the race track nodes
	/// </summary>

	GameObject player;

	// Difficulty Settings
	public string difficulty = "Normal";
	public float speedModifier = 1.0f;

	// Police Following
	public float stopRange = 5f;
	public float stopSpeed = 2f;

	public float easySpeed = 0.5f;
	public float normalSpeed = 1.0f;
	public float hardSpeed = 2.0f;

	protected override void Start()
	{
		base.Start();

		// Grab initial reference to player
		player = GameObject.FindWithTag("Player");

		if (difficulty == "Easy") speedModifier = easySpeed;
		else if (difficulty == "Normal") speedModifier = normalSpeed;
		else if (difficulty == "Hard") speedModifier = hardSpeed;

		// Find closest node and start its pathfinding there
		float closestDistanceToPolice = Vector3.Distance(transform.position, nodes[0].position);

		// Build our waypoints used in default navigation without player line of sight
		for (int i = 1; i < nodes.Count; i++)
		{
			float distance = Vector3.Distance(transform.position, nodes[i].position);

			// Since police cars are placed randomly across the map, and our waypoint guides start and end
			// with the length of the map, we need to grab the closest node and set that as our start, rather than
			// starting at node 0 like a regular race car would
			if (distance < closestDistanceToPolice)
			{
					closestDistanceToPolice = distance;
					currentNode = i;
			}
		}
	}

	private void FixedUpdate()
	{
		if (base.activate)
		{
			if (base.deactivate)
			{
				base.deactivateAI();
			}
			else if (player.GetComponent<PlayerState>().isSeen && player.GetComponent<PlayerState>().isUsingPhone)
			{
				base.Sensors();
				Steer();
				Drive(speedModifier);
				base.Brake();
				base.SteerCarToAngle(speedModifier);
			}
		}
		else
		{
			base.waitForStart();
		}
	}

	// Drive script with maximum speed determined by speedModifier necessary for difficulty tweaking
	protected override void Drive(float speedModifier)
	{
		currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

		// Slow down when close to the player
		if (Vector3.Distance(transform.position, player.transform.position) < stopRange)
		{
			GetComponent<Rigidbody>().velocity = (transform.forward * stopSpeed * speedModifier);
			isBraking = true;
		}
		else if (Vector3.Distance(transform.position, player.transform.position) < detectionRange)
		{
			GetComponent<Rigidbody>().velocity = (transform.forward * slowSpeed * speedModifier);
			isSlowing = true;
		}
		else
		{
			isBraking = false;
			isSlowing = false;
		}

		// Drive as fast as you can if we're not yet at our max speed, and we're not braking or slowing
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

	// Steer car to the player's current position
	protected override void Steer()
	{
		Vector3 relativeVector = transform.InverseTransformPoint(player.transform.position);
		float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
		targetSteerAngle = newSteer;
	}
}
