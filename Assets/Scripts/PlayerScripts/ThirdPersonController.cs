using UnityEngine;
using System.Collections;

public class ThirdPersonController : MonoBehaviour {

	float speed;
	Vector2 moveDirection;

	float lookDirection;
	public float rotationAngle = 90f;
	float rotationSpeed;

	Vector3 velocity;

	PlayerStats player;
	
	void Start() {
		player = GetComponent<PlayerStats>();
		speed = player.speed;
		rotationSpeed = player.rotationSpeed;
	}

	void Update () {
		if (moveDirection.magnitude > GameManager.Instance.ControllerDeadZone) {
			velocity = transform.forward * moveDirection.y;
			velocity += transform.right * moveDirection.x * 0.25f;
			velocity = velocity * speed * Time.deltaTime;
			GetComponent<Rigidbody>().velocity += velocity;
		}
		if (Mathf.Abs(lookDirection) > GameManager.Instance.ControllerDeadZone) {
			rotationAngle += rotationSpeed * lookDirection * Time.deltaTime;
		}

		gameObject.transform.rotation = Quaternion.AngleAxis (rotationAngle, Vector3.up);

		GetComponent<Rigidbody>().velocity *= 0.8f;
	}

	public void UpdateMoveDirection(Vector2 direction) {
		if (direction.magnitude > 1f) {
			moveDirection = direction.normalized;
		} else {
			moveDirection = direction;
		}
	}

	public void UpdateLookDirection(float rotationDirection) {
		lookDirection = rotationDirection;
	}
}
