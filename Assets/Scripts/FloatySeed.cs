using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatySeed : MonoBehaviour
{
    Animator ani;

    private void Awake()
    {
        ani = this.GetComponent<Animator>();
        ani.speed = Random.Range(0.75f, 1.25f);
        ani.Play(0, -1, Random.value);
    }


}
