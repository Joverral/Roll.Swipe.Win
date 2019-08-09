using UnityEngine;
using System.Collections;

public class SpawnRandomHingeMotorDirection : MonoBehaviour {

	void OnEnable()
    {
        var joint = this.GetComponent<HingeJoint2D>();

        if(Random.Range(0, 2) == 1)
        {
            joint.motor = new JointMotor2D()
            {
                maxMotorTorque = joint.motor.maxMotorTorque * -1.0f,
                motorSpeed = joint.motor.motorSpeed * -1.0f
            };

        }
    }
}
