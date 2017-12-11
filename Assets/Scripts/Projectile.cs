using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    [SerializeField] float initialSpeed = 200;
    [SerializeField] float lifetime = 5;

    private Rigidbody rb;
    private float timeInstatiated;
    private float lerpT;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(this.transform.up * initialSpeed);

        timeInstatiated = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeInstatiated > lifetime)
        {
            float scale = Mathf.Lerp(this.transform.localScale.x, 0, lerpT);
            lerpT += Time.deltaTime;

            if (scale < 0.05)
                Destroy(this.gameObject);
            else
                this.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}

