using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity; // C#

public class ForceControl : MonoBehaviour {
	
	public Rigidbody rigidbody;
	public ConstantForce constantForce;
	public Vector3 center;
	public float forceMagnitude = 10F;
	public Vector3 directionUnitVector;
	public bool firstUpdate = true;
	public Vector3 prevVelocity;
	public bool isPaused;
	public bool circularMotion = false;
//	public Vector3 acceleration = new Vector3 (0, 10f, 0);
	public List<Vector3> prevVelocities;
	public List<Vector3> prevPositions;
	public bool isReversed = false;

	void Awake () {  // initialise the variable
		rigidbody = GetComponent<Rigidbody>();
		constantForce = GetComponent<ConstantForce>();
		center = new Vector3 (1, 0, 0);
	}

	void Start() {
		prevPositions = new List<Vector3> ();
		prevVelocities = new List<Vector3> ();
//		rigidbody.AddForce (new Vector3 (50f, 0f, 0), ForceMode.Acceleration);
//		rigidbody.velocity = new Vector3 (1f, 0, 0);
//		ForceMode.Acceleration = new Vector3 (0, -5f, 0);
		constantForce.force = new Vector3 (0, 0, 0);
		VectorLine.SetRay (Color.green, new Vector3(0, 0, 0), new Vector3(1f, 0, 0));
		VectorLine.SetRay (Color.green, new Vector3(0, 0, 0), new Vector3(0, 1f, 0));
		VectorLine.SetRay (Color.green, new Vector3(0, 0, 0), new Vector3(0, 0, 1f));
	}

	void Update () {
		if (Input.GetKeyDown ("space")) {
			if (!isPaused) {
				prevVelocity = rigidbody.velocity;
				rigidbody.isKinematic = true;
				isPaused = true;
				createArrows ();
			} else {
				rigidbody.isKinematic = false;
				rigidbody.velocity = prevVelocity;
				isPaused = false;
			}
		}

		if (Input.GetKeyDown ("up")) {
			rigidbody.AddForce (new Vector3 (0, 0f, 1f), ForceMode.Impulse);
		}

		if (Input.GetKeyDown ("down")) {
			rigidbody.AddForce (new Vector3 (0, 0f, -1f), ForceMode.Impulse);
		}

		if (Input.GetKeyDown ("right")) {
			rigidbody.AddForce (new Vector3 (1f, 0f, 0), ForceMode.Impulse);
		}

		if (Input.GetKeyDown ("left")) {
			rigidbody.AddForce (new Vector3 (-1f, 0f, 0), ForceMode.Impulse);
		}

		if (isPaused) {
			if (Input.GetKeyDown ("return")) {
//				acceleration = -1f* acceleration;
				rigidbody.isKinematic = false;
				prevVelocities.Reverse ();
				prevPositions.Reverse ();
				isReversed = true;
//				rigidbody.velocity = -1f * prevVelocity;
				isPaused = false;
			}
		}
			
	}

	void createArrows () {
		for (int i=0; i< prevPositions.Count; i=i+10) {
			Vector3 position = prevPositions [i];
			Vector3 velocity = prevVelocities [i];
			Vector3 right = new Vector3(1f, 0, 0);
			Vector3 up = new Vector3(0, 1f, 0);
			Vector3 forward = new Vector3(0, 0, 1f);
			Vector3 rightVelocity = Vector3.Project (velocity, right);
			Vector3 upVelocity = Vector3.Project (velocity, up);
			Vector3 forwardVelocity = Vector3.Project (velocity, forward);
			Vector3 final = position + Vector3.Normalize(velocity);
			Vector3 final1 = position + 2*Vector3.Normalize(velocity);
			print ("start: "+position);
			VectorLine.SetRay (Color.green, position, Vector3.Normalize(velocity));
			var linePoints = new List<Vector3>(){final, final+ new Vector3(0.5f, 0, 0), final1, final+ new Vector3(-0.5f, 0, 0), final}; // C#
			var myLine = new VectorLine("Line", linePoints, 2.0f);
			myLine.Draw3D();
			VectorLine.SetRay (Color.red, position, 0.1f*upVelocity);
			VectorLine.SetRay (Color.yellow, position, 0.1f*rightVelocity);
			VectorLine.SetRay (Color.blue, position, 0.1f*forwardVelocity);
		}
	}
	  

	void FixedUpdate () {

		if (!isReversed) {
			prevVelocities.Add (rigidbody.velocity);
			prevPositions.Add (rigidbody.position);

//			rigidbody.AddForce (acceleration, ForceMode.Acceleration);
		} else {
			if (prevVelocities.Count > 0) {
				rigidbody.velocity = prevVelocities [0];
				prevVelocities.RemoveAt (0);
				rigidbody.position = prevPositions [0];
				prevPositions.RemoveAt (0);
			} else if (prevVelocities.Count == 0) {
				isReversed = false;
			}
		}

		if (circularMotion) {
			if (firstUpdate) {
				rigidbody.velocity = new Vector3 (0, 1f, 0);
				firstUpdate = false;
			}
			Vector3 currentPosition = transform.position;
			Vector3 direction = center - currentPosition;
			directionUnitVector = Vector3.Normalize (direction);
			constantForce.force = directionUnitVector * forceMagnitude;
		}
	}
}
