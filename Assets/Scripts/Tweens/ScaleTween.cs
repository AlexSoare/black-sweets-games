using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScaleTween : BaseTween {
	public Vector3 from = new Vector3(-1000, -1000, -1000);
	public Vector3 to = new Vector3(-1000, -1000, -1000);
	
	public void OnEnable() {
		if (this.from == new Vector3(-1000, -1000, -1000)) {
			this.from = transform.localScale;
		}
		
		if (this.to == new Vector3(-1000, -1000, -1000)) {
			this.to = transform.localScale;
		}
	}

    public override void Init() {
        transform.localScale = lerpFactor * to + @from * (1 - lerpFactor);
    }

	public override void Update() {
	    if (!Application.isPlaying || base.State == TweenState.Idle) return;
	    base.Update();
	    transform.localScale = lerpFactor * to + @from * (1 - lerpFactor);
	}

    [ContextMenu("PlayForward")]
    public void MenuPlayForward() {
        base.PlayForward();
    }
}
