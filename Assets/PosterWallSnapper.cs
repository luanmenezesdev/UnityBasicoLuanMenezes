using UnityEngine;
using System.Collections;

public class PosterWallSnapper : MonoBehaviour
{
    [Header("Wall detection")]
    public LayerMask wallLayer;
    public float snapDistance = 1.5f;

    [Header("Poster placement")]
    public float posterHalfDepth = 0.075f;
    public float wallGap = 0.02f;
    public Vector3 rotationCorrection = new Vector3(0f, 91f, 3f);

    [Header("State")]
    public bool isGrabbed;
    public bool isSnapped;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator Start()
    {
        yield return null;
        TrySnapToWall();
    }

    public void OnGrab()
    {
        isGrabbed = true;
        isSnapped = false;

        rb.isKinematic = false;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void OnRelease()
    {
        isGrabbed = false;

        if (!TrySnapToWall())
        {
            isSnapped = false;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public bool TrySnapToWall()
    {
        if (isGrabbed)
            return false;

        Vector3 origin = transform.position;

        Vector3[] directions =
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        bool found = false;
        RaycastHit bestHit = default;
        float bestDistance = Mathf.Infinity;

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, snapDistance, wallLayer))
            {
                if (hit.distance < bestDistance)
                {
                    found = true;
                    bestDistance = hit.distance;
                    bestHit = hit;
                }
            }
        }

        if (!found)
            return false;

        Snap(bestHit);
        return true;
    }

    private void Snap(RaycastHit hit)
    {
        Vector3 wallNormal = hit.normal;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        Quaternion wallRotation = Quaternion.LookRotation(wallNormal, Vector3.up);
        Quaternion correction = Quaternion.Euler(rotationCorrection);

        transform.rotation = wallRotation * correction;

        float distanceFromWall = posterHalfDepth + wallGap;
        transform.position = hit.point + wallNormal * distanceFromWall;

        isSnapped = true;
    }
}
