using UnityEngine;
using System.Collections;

public class VolumeTween : BaseTween {
    public float from = 0;
    public float to = 0;
    private AudioSource source;
    public bool DestroyAfter = false;

    void OnEnable() {
        source = GetComponent<AudioSource>();
    }

    public override void Init() {
        source.volume = from;
    }

    public override void Update() {
        base.Update();
        source.volume = from*(1 - lerpFactor) + to*lerpFactor;

        if (DestroyAfter && Mathf.Abs(lerpFactor - 1) < 0.01f) {
            source.volume = to;
            Destroy(this);
        }
    }

}
