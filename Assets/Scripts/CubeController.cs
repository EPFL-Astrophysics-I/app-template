using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float moveSpeed = 1;

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.right, Space.World);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.left, Space.World);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward, Space.World);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.back, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Reset();
    }

    public void Reset()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
