using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniFlags : MonoBehaviour
{
    public Component target;
    // Start is called before the first frame update


    public void FlagCallFunction(string FunctionName)
    {
        target.SendMessage(FunctionName);
    }
}
