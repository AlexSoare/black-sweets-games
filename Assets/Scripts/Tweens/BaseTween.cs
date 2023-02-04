using System;
using UnityEngine;
using System.Collections;

public enum TweenStyle {
	Once = 1, Loop = 2, PingPong = 3, PingPongLoop = 4
}

public enum TweenState {
	Idle = 1, Forward = 2, Reverse = 3
}

public abstract class BaseTween : MonoBehaviour {
	public bool playOnStart = false;
	public float delay = 0f;
	private float initialDelay = 0f;
	public float duration = 1f;
	public TweenStyle style = TweenStyle.Once;
	public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public Action callback;

	protected float lerpFactor {
		get { return animationCurve.Evaluate(t); }
	}

	protected float t = 0;
    public float T
    {
        get
        {
            return t;
        }
    }
	float directionChangeCount;
	public TweenState State = TweenState.Idle;
	float speed;

    public void PauseOn() { }

    public void Resume() { }

    public void Awake() {
        State = TweenState.Idle;
        initialDelay = delay;
    }

	public void Start() {
        t = 0;
		speed = 1 / duration;
		if (playOnStart) {
            Init();
			this.Play();
		}
        
	}

    public abstract void Init();

	public void Play() {
        Init();
		directionChangeCount = 0;
		this.PlayForward();
	}

	public virtual BaseTween PlayForward() {
        t = 0;
        speed = 1 / duration;
        delay = initialDelay;
        Init();
		directionChangeCount = 0;		
		this.State = TweenState.Forward;
	    return this;
	}

    public virtual BaseTween PlayReverse() {
        t = 1;
        speed = 1 / duration;
        Init();
		directionChangeCount = 0;		
		this.State = TweenState.Reverse;
        return this;
    }

	public void Stop() {
		this.State = TweenState.Idle;
		directionChangeCount = 0;
		t = 0;
	}

	public void Pause() {
		this.State = TweenState.Idle;
	}

    private void Finished() {
        if (callback != null)
            callback();
    }

	public virtual void Update() {
		// (tibi): This is horrible, find a nicer way to write this //mbadita: i agree. instructions unclear, dick got caught in the blender
		if (this.State == TweenState.Forward) {
		    if (delay > 0) {
		        delay -= Time.deltaTime;
                return;
		    }

			t = t + Time.deltaTime * speed;
			if (t > 1) {
				// Update tween state
				if (style == TweenStyle.Once) {
					// End of a once animation
					t = 1;
					this.State = TweenState.Idle;
				    Finished();
				} else if (style == TweenStyle.Loop) {
					// Animation should loop
					t = 0;
					this.State = TweenState.Forward;
				} else {
					// We are in a ping pong animation
					if (directionChangeCount == 0) {
						// just change the direction here
						t = 1;
						directionChangeCount ++;
						this.State = TweenState.Reverse;						
					} else if (directionChangeCount == 1 && style == TweenStyle.PingPong) {
						// We already changed direction once
						directionChangeCount = 0;
						t = 1;
						this.State = TweenState.Idle;
					} else {
						// We are in a ping pong loop
						t = 1;
						directionChangeCount ++;
						this.State = TweenState.Reverse;
					}
				}
			}
		}

		if (this.State == TweenState.Reverse) {
			t = t - Time.deltaTime * speed;
			if (t < 0) {
				// Update tween state
				if (style == TweenStyle.Once) {
					// End of a once animation
					t = 0;
					this.State = TweenState.Idle;
                    Finished();
				} else if (style == TweenStyle.Loop) {
					// Animation should loop
					t = 1;
					this.State = TweenState.Reverse;
				} else {
					// We are in a ping pong animation
					if (directionChangeCount == 0) {
						// just change the direction here
						t = 0;
						directionChangeCount ++;
						this.State = TweenState.Forward;						
					} else if (directionChangeCount == 1 && style == TweenStyle.PingPong) {
						// We already changed direction once
						directionChangeCount = 0;
						t = 0;
						this.State = TweenState.Idle;
					} else {
						// We are in a ping pong loop
						t = 0;
						directionChangeCount ++;
						this.State = TweenState.Forward;
					}
				}
			}
		}
	}

	public void DestroySelf() {
		Destroy (this);
	}
}
