using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PositionTween : BaseTween {
	public Vector3 from = new Vector3(-1000, -1000, -1000);
	public Vector3 to = new Vector3(-1000, -1000, -1000);

	public void OnEnable() {
		if (this.from == new Vector3(-1000, -1000, -1000)) {
			this.from = transform.position;
		}

		if (this.to == new Vector3(-1000, -1000, -1000)) {
            this.to = transform.position;
		}
	}

    public override void Init() {
        transform.position = from * (1 - lerpFactor) + to * lerpFactor;
    }

    public override void Update()
    {
	    if (!Application.isPlaying || State == TweenState.Idle) return;
	    base.Update();
	    transform.position = @from * (1 - lerpFactor) + to * lerpFactor; //Vector3.Lerp(from, to, this.lerpFactor);
	}
}
