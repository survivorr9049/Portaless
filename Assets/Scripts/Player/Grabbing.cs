using UnityEngine;

// Literally Garbage To Me!
public class Grabbing : MonoBehaviour {
	[Header("General")]
	public Transform grabbingPoint;
	public LayerMask layerMask;
	public KeyCode grabKey;
	private Transform _playerTransform;
	private Vector3 _cubeVelocity;
	private Vector3 _cubeLatePosition;
	[Header("Values")]
	[Range(1, 16)]
	public float maxDistance = 6f;
	[Range(1, 16)]
	public float lerpSpeed;
	[Range(1, 16)]
	public float rotationSpeed;
	public float minLocalHeight = -0.3f;

	[Header("Grabbed")]
	[SerializeField]
	private GameObject _grabbedObject;
	private Rigidbody _grabbedRigidbody;

	private GameObject fixedGrabbingPointGameObject;
	private Vector3 localGrabbingPoint;
	private Vector3 grabbingPointStartPosition;
	

	private void Start() {
		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		grabbingPointStartPosition = grabbingPoint.transform.localPosition;
		fixedGrabbingPointGameObject = Instantiate(
			new GameObject("FixedGrabbingPoint"),
			grabbingPoint.position,
			grabbingPoint.rotation
		);
		fixedGrabbingPointGameObject.transform.SetParent(_playerTransform);
	}

	private void Update() {
		RaycastHit hit;
		RaycastHit depthScan;
		Transform origin = Camera.main.transform;

		if(Physics.Raycast(origin.position, origin.forward, out depthScan, maxDistance, layerMask)) {
			Debug.Log(depthScan.transform.gameObject);
			grabbingPoint.transform.position = Vector3.Lerp(grabbingPoint.transform.position, depthScan.point - Camera.main.transform.forward * 0.8f, 12f*Time.deltaTime);
		}else{
			grabbingPoint.transform.localPosition = Vector3.Lerp(grabbingPoint.transform.localPosition, grabbingPointStartPosition, 4f*Time.deltaTime);
		}
		if (Input.GetKeyDown(grabKey)) {
			// Dropping held object
			if (_grabbedObject) {
				_grabbedObject.GetComponent<Rigidbody>().velocity = _cubeVelocity/Time.deltaTime;
				_grabbedObject.GetComponent<Rigidbody>().useGravity = true;
				IgnoreGrabbedObjectCollision(false);
				_grabbedObject = null;
				_grabbedRigidbody = null;
				return; // Ignore rest of the code
			}

			// Grabbing object
			if (Physics.Raycast(origin.position, origin.forward, out hit, maxDistance, ~layerMask)) {
				if (hit.rigidbody) {
					_grabbedObject = hit.collider.gameObject;
					_grabbedRigidbody = _grabbedObject.GetComponent<Rigidbody>();
					_grabbedRigidbody.useGravity = false;
					IgnoreGrabbedObjectCollision(true);
				}
			}
		}
		if (_grabbedObject) {
			localGrabbingPoint = _playerTransform.InverseTransformPoint(grabbingPoint.position);

			_grabbedObject.transform.position = Vector3.Lerp(
				_grabbedObject.transform.position,
				new Vector3(
					grabbingPoint.position.x,
					(localGrabbingPoint.y >= minLocalHeight) ?
						grabbingPoint.position.y : fixedGrabbingPointGameObject.transform.position.y + minLocalHeight,
					grabbingPoint.position.z
				),
				lerpSpeed * Time.deltaTime
			);

			_grabbedObject.transform.rotation = Quaternion.Lerp(
				_grabbedObject.transform.rotation,
				_playerTransform.rotation,
				rotationSpeed * Time.deltaTime
			);

			_grabbedRigidbody.velocity = Vector3.zero;
			_grabbedRigidbody.angularVelocity = Vector3.zero;
		}
		if(_grabbedObject)_cubeVelocity = _grabbedObject.transform.position - _cubeLatePosition;
		Debug.DrawRay(origin.position, origin.forward * maxDistance, Color.green);
	}
	void LateUpdate(){
		if(_grabbedObject)_cubeLatePosition = _grabbedObject.transform.position;	
	}

	private void IgnoreGrabbedObjectCollision(bool ignore) {
		Physics.IgnoreCollision(
			_grabbedObject.GetComponent<Collider>(),
			_playerTransform.gameObject.GetComponent<Collider>(),
			ignore
		);
	}
}
