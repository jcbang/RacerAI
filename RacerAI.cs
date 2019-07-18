using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacerAI : UniversalCarAI
{
    /// <summary>
    /// RacerAI extends UniversalCarAI. Adds a new CheckWin method that controls which
	/// position the car is registered as (first, second, third) when the car crosses the finish line.
    /// </summary>

	// Difficulty Settings
	public string difficulty = "Normal";

	public float speedModifier = 1.0f;
	public float easySpeed = 0.5f;
	public float normalSpeed = 1.0f;
	public float hardSpeed = 2.0f;

	public float personalityModifier = 1.0f;
	public float passive = 0.8f;
	public float standard = 1.0f;
	public float aggressive = 1.2f;

	public int racerNumber;
	

	protected override void Start()
	{
		base.Start();

		if (difficulty == "Easy") speedModifier = easySpeed;
		else if (difficulty == "Normal") speedModifier = normalSpeed;
		else if (difficulty == "Hard") speedModifier = hardSpeed;

		int random = Random.Range(1, 4);

		if (random == 1) personalityModifier = passive;
		else if (random == 2) personalityModifier = aggressive;
		else personalityModifier = standard;
	}

	private void FixedUpdate()
	{
		if (base.activate)
		{
			base.Sensors();
			base.Steer();
			base.Drive(speedModifier * personalityModifier);
			base.CheckNodeDistance();
			base.Brake();
			base.SteerCarToAngle(speedModifier * personalityModifier);
		}

		checkWin();
	}

	private void checkWin()
	{
		if (base.end == true)
		{
			if (!GameManager.Instance.firstPlaceTaken || !GameManager.Instance.secondPlaceTaken)
			{
				if (!GameManager.Instance.firstPlaceTaken)
				{
					GameManager.Instance.firstPlaceIndex = racerNumber;
				}
				else if (!GameManager.Instance.secondPlaceTaken)
				{
					GameManager.Instance.secondPlaceIndex = racerNumber;
				}
				else
				{
					;
				}
			}
		}
	}
}
