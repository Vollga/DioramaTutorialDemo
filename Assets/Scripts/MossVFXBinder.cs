using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class MossVFXBinder : MonoBehaviour
{
    Transform playerFeet;
    VisualEffect mossVfx;
    Mesh bMesh;
    Bounds bBox;
    public bool _useParent = true;
    //public bool _showBounds = false;

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        if (Application.isPlaying)
        {
            playerFeet = GameObject.FindGameObjectWithTag("Player").transform.Find("Feet");
        }
        mossVfx = this.GetComponent<VisualEffect>();
        
        if (_useParent)
        {
            bMesh = GetComponentInParent<MeshFilter>().sharedMesh;
        } 
        else
        {
            bMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }

        bBox = bMesh.bounds;

        mossVfx.SetMesh("Base Mesh", bMesh);
        mossVfx.SetVector3("bbCenter", bBox.center);
        mossVfx.SetVector3("bbSize", bBox.size);
    }

    

    private void OnTriggerStay(Collider other)      //pass player position to vfx graph if player is within bounds
    {
        mossVfx.SetVector3("Player Position", transform.InverseTransformPoint(playerFeet.position));
        //print("aaaa somethings touching meeee");
    }
}
