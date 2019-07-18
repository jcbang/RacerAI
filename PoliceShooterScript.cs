using System.Collections;
using UnityEngine;

public class PoliceShooterScript : MonoBehaviour
{
    /// <summary>
    /// The logic for the police officer inside of the cop car once the player
    /// gets spotted. The officer goes through the following pattern perpetually:
    /// LOOP -> get out of car -> fire rocket -> get back in car -> reload -> LOOP
    /// 
    /// The cycleTime variable is the wait time before he goes to the next phase.
    /// Shorter wait times = runs through phases quicker = fires quicker = difficulty
    /// 
    /// Also handles the rolling down of the car window, so it will need a reference to that mesh
    /// 
    /// Finally, this script expects the police officer to be a direct child of the cop car
    /// 
    /// There are more features, like the readyToFire variable, that will control things
    /// like aiming more meticulously, but these are dependent on the cop car AI
    /// </summary>

    private Animator anim;
    public SkinnedMeshRenderer smr;
    public MeshRenderer mr;
    private bool outOfCar, fired, loaded = true;
    public float cycleTime;
    private WaitForSeconds w;
    public GameObject rocket, rocketPrefab;
    public Transform rocketSpawnPosition;
    public ParticleSystem explosionPS;
    private bool readyToFire = true;
    private bool rollingDownWindow;
    private Vector3 r1;
    public Transform window;
    private Vector3 targetWindowPos;
	private GameObject player;

    void Start()
    {
        anim = GetComponent<Animator>();
        w = new WaitForSeconds(1);
        smr.enabled = mr.enabled = false;
        InvokeRepeating("CycleState", 2, cycleTime);
        StartCoroutine(RollDownWindow());
        targetWindowPos = new Vector3(window.localPosition.x, window.localPosition.y - .42f, window.localPosition.z);
    }

    void Update()
    {
        if (rollingDownWindow)
            window.localPosition = Vector3.SmoothDamp(window.localPosition, targetWindowPos, ref r1, .5f);
    }

    void CycleState()
    {
        if (outOfCar)
        {
            if (loaded && readyToFire)
                Fire();
            else
                GetInCar();
        }
        else
        {
			if (loaded)
			{
				// If the player is seen, then get out of the car and begin firing
				player = GameObject.FindGameObjectWithTag("Player");

				if (player.GetComponent<PlayerState>().isSeen == true)
				{
					GetOutOfCar();
				}
			}
			else
			{
				Reload();
			}
		}
    }

    private IEnumerator DelayedRendererOff()
    {
        yield return w;
        smr.enabled = mr.enabled = false;
    }

    // Four Cycle Functions that CycleState alternates between

    void GetOutOfCar()
    {
        smr.enabled = mr.enabled = true;
        anim.SetBool("OutOfCar", true);
        outOfCar = true;
    }

    void Fire()
    {
        // The readyToFire variable reflects whether the AI logic determines
        // It has a good shot based on where the cop car positions him

        anim.SetTrigger("Fire");
        loaded = false;
        rocket.GetComponent<RocketScript>().Fire(transform.parent.forward);
        rocket.transform.parent = null;
        explosionPS.Play();
    }

    void GetInCar()
    {
        anim.SetBool("OutOfCar", false);
        StartCoroutine(DelayedRendererOff());
        outOfCar = false;
    }

    void Reload()
    {
        loaded = true;
        rocket = Instantiate(rocketPrefab, rocketSpawnPosition.position, rocketSpawnPosition.rotation, rocketSpawnPosition);
    }

    IEnumerator RollDownWindow()
    {
        rollingDownWindow = true;
        yield return new WaitForSeconds(2);
        rollingDownWindow = false;
    }
}
