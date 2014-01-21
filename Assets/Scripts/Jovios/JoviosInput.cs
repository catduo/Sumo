using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosInput{
	public JoviosInput(){
		gyro = Quaternion.identity;
		direction = Vector2.zero;
		acceleration = Vector3.zero;
	}
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
	private Vector2 direction;
	public Vector2 GetDirection(){
		return direction;
	}
	public void SetDirection(Vector2 newDirection){
		direction = newDirection;
	}
}

