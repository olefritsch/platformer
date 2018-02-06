using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPlayers : MonoBehaviour
{
    [SerializeField] TransformRuntimeSet targets;

    [SerializeField] Vector3 offset = new Vector3(0f, 0f, -15f);
    [SerializeField] float smoothTime = 0.5f;

    [SerializeField] float minZoom = 70f;
    [SerializeField] float maxZoom = 30f;
    [SerializeField] float zoomLimiter = 50f;

    private Camera cam;
    private Vector3 _refVelocity;
    
    void Start()
    {
        cam = GetComponent<Camera>();    
    }

    private void LateUpdate()
    {
        if (targets.Count < 1)
            return;

        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        // Smoothly update camera position
        transform.position = Vector3.SmoothDamp(transform.position, bounds.center + offset, ref _refVelocity, smoothTime);
        
        // Lerp between max/min zoom based on distance between players
        cam.fieldOfView = Mathf.Lerp(maxZoom, minZoom, bounds.size.x / zoomLimiter);
    }
}
