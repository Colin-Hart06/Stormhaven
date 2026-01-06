using UnityEngine;

public class placeBlock : MonoBehaviour
{
    public GameObject person;
    public GameObject box;
    public Camera cam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // Keep on 2D plane

            Instantiate(person, mouseWorldPos, Quaternion.identity);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // Keep on 2D plane

            Instantiate(box, mouseWorldPos, Quaternion.identity);
        }
    }
}
