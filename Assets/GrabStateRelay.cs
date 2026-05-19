using Oculus.Interaction;
using UnityEngine;

public class GrabStateRelay : MonoBehaviour
{
    public Grabbable grabbable;
    public PosterWallSnapper snapper;

    private bool wasGrabbed;

    private void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<Grabbable>();

        if (snapper == null)
            snapper = GetComponent<PosterWallSnapper>();
    }

    private void Update()
    {
        if (grabbable == null || snapper == null)
            return;

        bool isGrabbedNow = grabbable.SelectingPointsCount > 0;

        if (isGrabbedNow && !wasGrabbed)
            snapper.OnGrab();

        if (!isGrabbedNow && wasGrabbed)
            snapper.OnRelease();

        wasGrabbed = isGrabbedNow;
    }
}
