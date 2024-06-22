using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualInputManager : MonoBehaviour
{
    public static VirtualInputManager instance;
    public VirtualsInputs inputs;

    public VirtualInputManager(){
        instance = this;
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        foreach (VirtualsInputs.VirtualAxis item in inputs.Virtuals){
            item.Init();
        }
    }
}
