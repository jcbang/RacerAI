using UnityEngine;

/// <summary>
/// Missile script:
/// Two modes - Dumbfire (straight line) and Seeker (follow player).
/// Missile will automatically switch to dumbfire if it's active for more than
/// half of its lifespan.
/// 
/// All variables configurable below for convenience.
/// </summary>

public class RocketScript : MonoBehaviour
{
	private bool firing;
    public GameObject particle;
	private GameObject player;
	private Vector3 playerPosition;
	private int secondsPassed;
	private float timer;

	// CONFIG:
	public float rocketSpeed = 30f;
	public int missileLifeSpan = 10;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		this.gameObject.tag = "Rocket";
	}

    void Update()
    {
		playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        if (firing)
		{
			timer += Time.deltaTime;
			if (timer > 1f)
			{
				secondsPassed++;
				timer = 0f;
			}

			// If the missile is past half of it's lifespan, switch to dumb-fire mode instead of seeker-mode
			if (secondsPassed > (missileLifeSpan/2))
			{
				transform.position += transform.forward * rocketSpeed * Time.deltaTime;
			}
			// Seeker missiles track a player's location
			else
			{
				transform.LookAt(playerPosition);
				transform.position = Vector3.MoveTowards(transform.position, playerPosition, rocketSpeed * Time.deltaTime);
			}

		}

    }

    public void Fire(Vector3 forward)
    {
        GetComponent<BoxCollider>().isTrigger = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.eulerAngles = forward;

		// TEMPORARY FIX FOR POLICE CAR BASED SHOOTING
		transform.Translate(0, 0, 4);

        firing = true;
        Destroy(gameObject, missileLifeSpan);
    }

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag != "Police")
		{
			Instantiate(particle, transform.position, transform.rotation);
			Destroy(gameObject);
		}

    }
}
