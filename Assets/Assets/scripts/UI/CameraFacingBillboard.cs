using UnityEngine;
using System.Collections;


public class CameraFacingBillboard : MonoBehaviour
{
    public bool ScaleWithCamera;
    public float BillboardScale;
    private Transform mainCameraTran;
    private float initiallY;
    void Start()
    {
        if (!mainCameraTran)
        {
            mainCameraTran = Camera.main.transform;
        }
        initiallY = transform.position.y;
    }

    void Update()
    {
        transform.LookAt(transform.position + mainCameraTran.rotation * Vector3.forward,
            mainCameraTran.rotation * Vector3.up);
        if (ScaleWithCamera)
        {
            transform.localScale = new Vector3(Mathf.Max(1f, mainCameraTran.position.z / BillboardScale), Mathf.Max(1f,mainCameraTran.position.z/ BillboardScale), 1f);
            transform.position = new Vector3(transform.position.x, initiallY+(mainCameraTran.position.z/ BillboardScale/2), transform.position.z);
        }
    }
}