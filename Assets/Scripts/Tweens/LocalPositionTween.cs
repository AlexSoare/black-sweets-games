using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LocalPositionTween : BaseTween {
	public Vector3 from = new Vector3(-1000, -1000, -1000);
	public Vector3 to = new Vector3(-1000, -1000, -1000);

	public void OnEnable() {
		if (this.from == new Vector3(-1000, -1000, -1000)) {
			this.from = transform.localPosition;
		}

		if (this.to == new Vector3(-1000, -1000, -1000)) {
            this.to = transform.localPosition;
		}
	}

    public override void Init() {
        transform.localPosition = from * (1 - lerpFactor) + to * lerpFactor;
    }

    public override void Update()
    {
		if (Application.isPlaying && State != TweenState.Idle) {
			base.Update();
			transform.localPosition = from * (1 - lerpFactor) + to * lerpFactor;
		}
	}
}
