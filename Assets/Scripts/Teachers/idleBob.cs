using UnityEngine;

public class IdleBob : MonoBehaviour
{
    public float amplitude = 0.03f;
    public float speed = 3f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
