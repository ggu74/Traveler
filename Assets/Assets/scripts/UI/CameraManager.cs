using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    public float ZoomSpeed;
    public Vector3 newTarget;
	public float newAngle;
	public float DefaultZoom;
	public float MoveSpeed;
	public float RotateSpeed;
    public Transform Up;
	public Transform MainCameraTrans;
    public float MinZoomOut=-4;
    public float MaxZoomOut = -40;
    private float newZoom;
    private float currentAngle;

    public float NewZoom
    {
        get
        {
            return newZoom;
        }

        set
        {
            newZoom = Mathf.Clamp(value, MaxZoomOut, MinZoomOut);
        }
    }

    void Start(){
		currentAngle = 0;
		NewZoom = DefaultZoom;
        MainCameraTrans.localPosition = Vector3.forward * NewZoom;
	}

	public void SlideToPosition(Vector3 target){
		newTarget = target;
	}

	public void JumpToPosition(Vector3 target){
		newTarget = target;
		transform.position = target;
	}

	public void SlideToRotation(float angle){
		newAngle = angle;
	}


	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp (transform.position, newTarget, MoveSpeed * Time.deltaTime);
		currentAngle = Mathf.Lerp (currentAngle, newAngle, RotateSpeed * Time.deltaTime);
		transform.localEulerAngles = new Vector3 (0,currentAngle,0);
		NewZoom += Input.GetAxis ("Mouse ScrollWheel") * ZoomSpeed* -newZoom;
        MainCameraTrans.localPosition = new Vector3 (0,0,Mathf.Lerp(MainCameraTrans.localPosition.z, NewZoom, 4 * Time.deltaTime));
        SlideToPosition(transform.position+ Time.deltaTime*(Vector3.forward*Input.GetAxis("Vertical") * MoveSpeed+ Vector3.right * Input.GetAxis("Horizontal")* MoveSpeed)*-newZoom);
      //  print(Input.mousePosition);
	}
}
