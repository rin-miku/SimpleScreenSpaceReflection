using UnityEngine;

public class DrawRay : MonoBehaviour
{
    public Transform mainCamera;
    public int maxBounces = 3;
    public float maxDistance = 20f;

    private Color[] bounceColors;

    void OnDrawGizmos()
    {
        if (mainCamera == null) return;

        if (bounceColors == null || bounceColors.Length != maxBounces)
        {
            bounceColors = new Color[maxBounces];
            for (int i = 0; i < maxBounces; i++)
                bounceColors[i] = Random.ColorHSV();
        }

        Vector3 origin = mainCamera.position;
        Vector3 dir = mainCamera.forward;

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit hit;
            Vector3 endPoint = origin + dir * maxDistance;

            if (Physics.Raycast(origin, dir, out hit, maxDistance))
                endPoint = hit.point;

            Gizmos.color = bounceColors[i];
            Gizmos.DrawLine(origin, endPoint);

            if (Physics.Raycast(origin, dir, out hit, maxDistance))
            {
                origin = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);
            }
            else
            {
                break;
            }
        }
    }
}