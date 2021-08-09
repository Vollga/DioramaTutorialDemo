
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(2 * transform.position - mainCam.position);
    }
}
