using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum animStates { WALKING, JOGGING, RUNNING}

public class CompleteBodyAnim : MonoBehaviour
{
    public walking leftLeg;
    public walking rightLeg;
    public ArmsWalking leftArm;
    public ArmsWalking rightArm;

    public animStates states;

    void Start()
    {
        states = animStates.WALKING;
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (states) { 
        case animStates.WALKING:
                break;
        case animStates.JOGGING:
                break;
        case animStates.RUNNING:
                break;
        }
    }
}
