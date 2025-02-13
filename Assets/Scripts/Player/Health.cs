using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour {
	public int maxHealth = 100;
	public int regenRate = 5;
	public float minTimeAfterHit = 5;
	public KeyCode emergencySuicideKey = KeyCode.R;
	public float timeToRespawn = 2f;

	[SerializeField]
	private float health;
	private bool isAlive = true;
	private GameObject cameraObject;
	[SerializeField]
	private Image hurtSprite;

	private float regenTimer;
	private float respawnTimer;
	private float lastHitTimer;
	
	private Transform playerTransform;

	private void Start() {
		health = maxHealth;
		cameraObject = Camera.main.gameObject;
	}

	private void Update() {
		if (GetHealth() > maxHealth)
			health = maxHealth;
		else if (GetHealth() <= 0)
			Kill();

		if (GetHealth() < maxHealth)
			lastHitTimer += Time.deltaTime;

		if (Input.GetKeyDown(emergencySuicideKey))
			Kill();

		if (!isAlive) {
			respawnTimer += Time.deltaTime;
			
			if (respawnTimer >= timeToRespawn && Input.anyKey)
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		hurtSprite.color = new Color(1, 1, 1, 1-(health/maxHealth));

		if (isAlive && lastHitTimer >= minTimeAfterHit && health < maxHealth) {
			regenTimer += Time.deltaTime;

			if (regenTimer >= 1) {
				health += regenRate;
				regenTimer = 0;
			}
		}
	}

	public float GetHealth() => health;
	public bool IsAlive() => isAlive;

	public void Hurt(float amount) {
		if (isAlive) {// You can't hurt me if I'm already dead...
			health -= amount;
			lastHitTimer = 0;
		}
	}

	public void Kill() {
		if (!isAlive)
			return;
		isAlive = false;
		health = 0;

		cameraObject.AddComponent<SphereCollider>().radius = 0.4f;
		Physics.IgnoreCollision(
			gameObject.GetComponent<Collider>(),
			cameraObject.GetComponent<Collider>(),
			true
		);
		cameraObject.AddComponent<Rigidbody>();
		cameraObject.transform.parent = null;
	}
}
