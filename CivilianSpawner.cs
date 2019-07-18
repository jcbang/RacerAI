using System.Collections;
using UnityEngine;

public class CivilianSpawner : MonoBehaviour
{
	/// <summary>
	/// To be placed on an empty game object, CivilianSpawner repeatedly spawns civilian cars
	/// until a carCrash is detected from one of its child cars.
	/// </summary>

	private int secondsPassed;
	private float timer;
	public Transform carSpawnPosition;
	public Transform car1;
	public Transform car2;
	public Transform car3;
	public Transform car4;
	private Transform newCar;

	public bool spawning = true;
	public bool carCrash = false;

	public int spawnTimer = 2;

	public float random;
	public int carType;

	private void Start()
	{
		// Random spawn rate
		random = Random.Range(1f, 2f);

		// Random selection of cars (SUV, Sedan...) for visual variety
		carType = Random.Range(1, 4);

		carSpawnPosition = this.transform;
	}

	private void Update()
	{
		if (spawning)
		{
			// Time.deltaTime for seconds incrementing
			timer += Time.deltaTime;
			if (timer > 1f)
			{
				secondsPassed++;
				timer = 0f;
			}

			// Keep spawning cars
			if (secondsPassed > spawnTimer)
			{
				spawnCar();
				secondsPassed = 0;
			}
		}
	}

    private void spawnCar()
    {
		var spawnable = car1;

		if (carType == 1) spawnable = car1;
		else if (carType == 2) spawnable = car2;
		else if (carType == 3) spawnable = car3;
		else spawnable = car4;

		newCar = Instantiate(spawnable, carSpawnPosition.position, carSpawnPosition.rotation, carSpawnPosition);
		newCar.GetComponent<CivilianAI>().civilianSpeed *= random;
		newCar.gameObject.tag = "Civilian";
	}
}
