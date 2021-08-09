using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentCollision : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<MeshCollider>().enabled = true;
    }
}
