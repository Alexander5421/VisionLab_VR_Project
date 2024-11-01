using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObject : MonoBehaviour {

	public float speedH = 2.0f;
	public float speedV = 2.0f;

	private float yaw;
	private float pitch;

	void Update() {

		yaw += speedH * Input.GetAxis("Mouse X") + speedH * Input.GetAxis("Horizontal");
		pitch -= speedV * Input.GetAxis("Mouse Y") + speedV * Input.GetAxis("Vertical");

		transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
	}
}