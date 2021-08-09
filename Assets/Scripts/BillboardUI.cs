
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    public float size = 1;
    public float minSize = 1;
    Camera mainCam;
    float tempSize;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = mainCam.transform.rotation;
        //transform.LookAt(2 * transform.position - mainCam.position);

        tempSize = (mainCam.transform.position - transform.position).magnitude * size;
        tempSize *= mainCam.fieldOfView * 0.001f;
        
        if (tempSize < minSize)
        {
            tempSize = minSize;
        }

        transform.localScale = new Vector3(tempSize, tempSize, tempSize);
    }
}
