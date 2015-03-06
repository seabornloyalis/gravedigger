using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public void SetCamPos(int level)
	{
		float centerPos = 3.0f + (level*0.5f);
		transform.position = new Vector3 (centerPos, centerPos, -10);
	}
}
