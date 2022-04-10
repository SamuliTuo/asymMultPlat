using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public enum AudioClip {
    NULL,
    LASER_SHOOT,
    LASER_HIT,
    CANNON_SHOOT,
    CANNON_HIT,
    SWORD_SWING,
    SWORD_HIT,
    ENEMY_HIT,
    ENEMY_DEATH,
    PILOT_JUMP,
    PILOT_DASH
}

public class SFXManager : MonoBehaviour {

    public static SFXManager current;

    [SerializeField] private StudioEventEmitter shoot_laser = null;
    [SerializeField] private StudioEventEmitter shoot_cannon = null;
    [SerializeField] private StudioEventEmitter sword_swing = null;
    [SerializeField] private StudioEventEmitter hit_laser = null;
    [SerializeField] private StudioEventEmitter hit_cannon = null;
    [SerializeField] private StudioEventEmitter hit_enemy = null;
    [SerializeField] private StudioEventEmitter hit_sword = null;
    [SerializeField] private StudioEventEmitter death_enemy = null;
    [SerializeField] private StudioEventEmitter pilot_jump = null;
    [SerializeField] private StudioEventEmitter pilot_dash = null;

    private Dictionary<AudioClip, StudioEventEmitter> clips = new Dictionary<AudioClip, StudioEventEmitter>();

    void Start() {
        current = this;
        ListAllClips();
    }

    public void PlayAudio(AudioClip type) {
        if (type != AudioClip.NULL) {
            StudioEventEmitter clip;
            clips.TryGetValue(type, out clip);
            clip.Play();
        }
    }

    void ListAllClips() {
        clips.Add(AudioClip.LASER_SHOOT, shoot_laser);
        clips.Add(AudioClip.LASER_HIT, hit_laser);
        clips.Add(AudioClip.CANNON_SHOOT, shoot_cannon);
        clips.Add(AudioClip.CANNON_HIT, hit_cannon);
        clips.Add(AudioClip.SWORD_SWING, sword_swing);
        clips.Add(AudioClip.SWORD_HIT, hit_sword);
        clips.Add(AudioClip.ENEMY_HIT, hit_enemy);
        clips.Add(AudioClip.ENEMY_DEATH, death_enemy);
        clips.Add(AudioClip.PILOT_JUMP, pilot_jump);
        clips.Add(AudioClip.PILOT_DASH, pilot_dash);
    }
}