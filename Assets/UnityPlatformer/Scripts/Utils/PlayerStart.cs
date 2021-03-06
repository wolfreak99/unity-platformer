using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Create a player at given position
  /// </summary>
  public class PlayerStart : InstancePrefab {
    /// <summary>
    /// If you have your own camera, this should be false.
    /// </summary>
    public bool setupCameraFollow = true;
    /// <summary>
    /// true: Enable CharacterMonitor if it's found in the Prefab or Create it
    /// false: do nothing
    /// </summary>
    public bool monitor = false;

    [HideInInspector]
    public Character character;
    [HideInInspector]
    public PlatformerInput input;
    [HideInInspector]
    public CharacterMonitor mon;

    /// <summary>
    /// Instance the prefab, rename and attach it
    /// </summary>
    /// <param name="notify">true -> SendMessage: OnInstancePrefab</param>
    public override void OnAwake(bool notify = true) {
      base.OnAwake(false); // notify below, maybe someone need mon

      Character[] chars = instance.gameObject.GetComponentsInChildren<Character>();
      if (chars.Length != 1) {
        Debug.LogErrorFormat("Found {0} Character(s) expected 1", chars.Length);
        return;
      }


      PlatformerInput[] inputs = instance.gameObject.GetComponentsInChildren<PlatformerInput>();

      if (inputs.Length != 1) {
        Debug.LogErrorFormat("Found {0} PlatformerInput(s) expected 1", inputs.Length);
        return;
      }

      character = chars[0];
      input = inputs[0];

      mon = character.gameObject.GetOrAddComponent<CharacterMonitor>();
      mon.enabled = monitor;

      if (setupCameraFollow) {
        var cams = Camera.allCameras;

        foreach (var c in cams) {
          CameraFollow cf = c.GetComponent<CameraFollow>();

          if (cf) {
            cf.target = character;
            cf.targetInput = input;
          }
        }
      }

      // notify
      SendMessage("OnInstancePrefab", this, SendMessageOptions.DontRequireReceiver);
    }
  }
}
