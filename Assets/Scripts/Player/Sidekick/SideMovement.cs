using UnityEngine;

public class SideMovement : MonoBehaviour
{
    [SerializeField] private float radiusX = 5f;
    [SerializeField] private float radiusY = 3f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float yOffset = 0.3f;
    private float angle;

    void Update()
    {
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radiusX;
        float y = Mathf.Sin(angle) * radiusY;

        transform.position = this.transform.parent.position + new Vector3(x, y + yOffset, 0);
    }
}
