using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PositionIncrementalTween : BaseTween {
	public Vector3 from = new Vector3(-1000, -1000, -1000);
	public Vector3 to = new Vector3(-1000, -1000, -1000);
    public Vector3 _distance;

    new void Awake() {
        to = from;
    }

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
		if (Application.isPlaying && State != TweenState.Idle) {
			base.Update();
            //if (State == TweenState.Idle) return;
			transform.position = from * (1 - lerpFactor) + to * lerpFactor;
		}
	}


    public override BaseTween PlayForward() {
        t = 0;
        callback += UpdatePositionsForward;
        this.State = TweenState.Forward;
        from = transform.position;
        to += _distance;
        return this;
    }

    public override BaseTween PlayReverse() {
        t = 0;
        callback += UpdatePositionsReverse;
        this.State = TweenState.Forward;
        from = transform.position;
        to -= _distance;
        return this;
    }

    void UpdatePositionsForward() {
        callback -= UpdatePositionsForward;
        //transform.position = from * (1 - lerpFactor) + to * lerpFactor;
        //from = transform.position;
        //to = transform.position + _distance;
    }

    void UpdatePositionsReverse() {
        //Debug.Log("mimi ki: " + to + " from: " + from);
        callback -= UpdatePositionsReverse;
        //transform.position = from * (1 - lerpFactor) + to * lerpFactor;
        //from = transform.position;
        //to = transform.position - _distance;
    }
}
