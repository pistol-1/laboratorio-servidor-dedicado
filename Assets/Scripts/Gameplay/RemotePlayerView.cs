using UnityEngine;

public class RemotePlayerView : MonoBehaviour
{
    public float smoothingSpeed = 10f;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
    }
}
