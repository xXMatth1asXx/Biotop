using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f; // Geschwindigkeit der Kamera
    [SerializeField] private float mouseSensitivity = 100.0f; // Empfindlichkeit der Mausbewegung
    [SerializeField] private float speedSensitivity = 1.0f; // Sensitivität der Geschwindigkeitsänderung

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    // Awake wird aufgerufen, wenn das Skript instanziiert wird
    void Awake()
    {
        // Versteckt den Mauszeiger und sperrt ihn in der Mitte des Bildschirms
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        Vector3 move = new Vector3(0, 0, 0);

        // Bewegung nach vorne und hinten
        move += Input.GetAxis("Vertical") * Vector3.forward;

        // Bewegung nach links und rechts
        move += Input.GetAxis("Horizontal") * Vector3.right;

        // Bewegung transformieren, um mit der Kamerarotation zu übereinstimmen
        move = transform.rotation * move;

        // Auf und ab bewegen
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift))
            move -= Vector3.up;

        // Bewege die Kamera
        transform.position += move * speed * Time.deltaTime;

        // Kamerarotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Ändert die Geschwindigkeit basierend auf dem Mausrad
        speed += Input.mouseScrollDelta.y * speedSensitivity;
    }
}
