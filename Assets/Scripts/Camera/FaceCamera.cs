using UnityEngine;
using TMPro;

public class FaceCamera : MonoBehaviour
{
    public GameObject cameraToLookAt;

    private void Start()
    {
        cameraToLookAt = GameObject.Find("Main Camera");
    }

    void Update()
    {
        transform.LookAt(2 * transform.position - cameraToLookAt.transform.position);
    }
}

