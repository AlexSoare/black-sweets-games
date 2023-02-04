using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RotationTween : BaseTween {

	public Vector3 from = new Vector3(-1000, -1000, -1000);
	public Vector3 to = new Vector3(-1000, -1000, -1000);
	
	public void OnEnable() {
		if (this.from == new Vector3(-1000, -1000, -1000)) {
			this.from = transform.localRotation.eulerAngles;
		}
		
		if (this.to == new Vector3(-1000, -1000, -1000)) {
			this.to = transform.localRotation.eulerAngles;
		}
	}

    public override void Init() {
        transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, this.lerpFactor));
    }

    public override void Update()
    {
		if (Application.isPlaying && State != TweenState.Idle) {
			base.Update();
			transform.localRotation = Quaternion.Euler(Vector3.Lerp(from, to, this.lerpFactor));
		}
	}
}
