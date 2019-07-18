using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CivilianAI : UniversalCarAI
{
    /// <summary>
    /// CivilianAI extends UniversalCarAI, stripping down most methods and retaining only Drive for
	/// straight line travel. Inclusion of a unique OnCollisionEnter method determines spawn capabilities
	/// sent back to the parent spawner.
    /// </summary>

	public float civilianSpeed = 20f;

	private Transform spawner;

	public string difficulty = "Normal";

	public float speedModifier = 1.0f;

	public float easySpeed = 0.5f;
	public float normalSpeed = 1.0f;
	public float hardSpeed = 2.0f;

	protected override void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = centerOfMass;

		spawner = this.transform.parent;

		if (difficulty == "Easy") speedModifier = easySpeed;
		else if (difficulty == "Normal") speedModifier = normalSpeed;
		else if (difficulty == "Hard") speedModifier = hardSpeed;
	}

	private void FixedUpdate()
	{
		if (this.spawner.GetComponent<CivilianSpawner>().carCrash == true)
		{
			base.deactivateAI();
			spawner.GetComponent<CivilianSpawner>().spawning = false;
		}
		else
		{
			Drive(speedModifier);
			base.Brake();
		}
	}

	protected override void Drive(float speedModifier)
	{
		currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

		if (currentSpeed < (civilianSpeed) && !isBraking && !isSlowing)
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

	private void OnCollisionEnter(Collision collision)
	{
		// If this car collides with a player, racer, or police officer, then register it as a car crash
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Racer" 
		   || collision.gameObject.tag == "Police")
		{
			spawner.GetComponent<CivilianSpawner>().carCrash = true;
		}
		// Else if it hits a collidable wall at the end of an intersection, despawn it
		else
		{
			Destroy(gameObject);
		}
	}
}
