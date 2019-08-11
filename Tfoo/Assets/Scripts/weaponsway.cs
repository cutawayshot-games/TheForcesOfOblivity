using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponsway : MonoBehaviour
{
    public float amount;
    public float smoothamount;
    public float maxamount;
    private Vector3 initialposition;

    void Start()
    {
        initialposition = transform.localPosition;
    }

    void Update()
    {
        float movementx = -Input.GetAxis("Mouse X") * amount;
        float movementy = -Input.GetAxis("Mouse Y") * amount;
        movementx = Mathf.Clamp(movementx, -maxamount, maxamount);
        movementy = Mathf.Clamp(movementy, -maxamount, maxamount);

        Vector3 finalposition = new Vector3(movementx, movementy, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalposition + initialposition, Time.deltaTime*smoothamount);
    }
}
