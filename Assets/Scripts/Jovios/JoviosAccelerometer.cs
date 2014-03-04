using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosAccelerometer{
	private JoviosAccelerometerStyle accelerometerStyle;
	public JoviosAccelerometerStyle GetAccelerometerStyle(){
		return accelerometerStyle;
	}
	public void SetAccelerometerStyle(JoviosAccelerometerStyle newStyle){
		accelerometerStyle = newStyle;
	}
	//this initializes the input variables
	public JoviosAccelerometer(JoviosAccelerometerStyle newAccelerometerStyle){
		accelerometerStyle = newAccelerometerStyle;
		gyro = Quaternion.identity;
		acceleration = Vector3.zero;
	}
	//this is for the accelerometer
	private Quaternion gyro;
	public Quaternion GetGyro(){
		return gyro;
	}
	public void SetGyro(Quaternion setGyro){
		gyro = setGyro;
	}
	private Vector3 acceleration;
	public Vector3 GetAcceleration(){
		return acceleration;
	}
	public void SetAcceleration(Vector3 setAcc){
		acceleration = setAcc;
	}
}

