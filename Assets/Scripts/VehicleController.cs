using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float speed = 10f; // Speed of the vehicle
    public float rotationSpeed = 100f; // Rotation speed of the vehicle
    public float climbForce = 10f; // Force to climb the terrain
    public float maxGroundAngle = 45f; // Maximum angle for the terrain the vehicle can move on

    private Rigidbody rb;
    private Terrain terrain;

    void Start()
    {
        // Get the Rigidbody component attached to the vehicle GameObject
        rb = GetComponent<Rigidbody>();

        // Find the terrain in the scene
        terrain = Terrain.activeTerrain;
    }

    void FixedUpdate()
    {
        // Get input from the arrow keys
        float moveInput = Input.GetAxis("Vertical");
        float rotateInput = Input.GetAxis("Horizontal");

        // Calculate the movement and rotation
        float moveAmount = moveInput * speed * Time.deltaTime;
        float rotateAmount = rotateInput * rotationSpeed * Time.deltaTime;

        // Apply rotation to the Rigidbody
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotateAmount);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Get the normal of the terrain under the vehicle
        Vector3 vehiclePosition = transform.position;
        float terrainHeight = terrain.SampleHeight(vehiclePosition);
        Vector3 terrainNormal = terrain.terrainData.GetInterpolatedNormal(vehiclePosition.x / terrain.terrainData.size.x, vehiclePosition.z / terrain.terrainData.size.z);

        // Calculate the angle between the terrain normal and the vehicle's up direction
        float groundAngle = Vector3.Angle(Vector3.up, terrainNormal);

        // Apply movement to the Rigidbody if the ground angle is within the acceptable range
        if (groundAngle <= maxGroundAngle)
        {
            // Apply force to climb the terrain
            Vector3 climbForceDirection = Vector3.Cross(transform.right, terrainNormal);
            rb.AddForce(climbForceDirection * climbForce * moveInput, ForceMode.Force);

            // Apply movement to the Rigidbody
            rb.MovePosition(rb.position + transform.forward * moveAmount);
        }
    }
}
