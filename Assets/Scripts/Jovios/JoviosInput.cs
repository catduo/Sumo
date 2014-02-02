using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosInput{
	//this initializes the input variables
	public JoviosInput(){
		gyro = Quaternion.identity;
		direction = Vector2.zero;
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

	//this is for any directional inputs, will eventually support arbitrary definitions
	private Vector2 direction;
	public Vector2 GetDirection(){
		return direction;
	}
	public void SetDirection(Vector2 newDirection){
		direction = newDirection;
	}
}

