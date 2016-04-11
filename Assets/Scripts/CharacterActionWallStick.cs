using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Perform an action over a character
  /// </summary>
  [RequireComponent (typeof (PlatformerController))]
  [RequireComponent (typeof (Controller2D))]
  [RequireComponent (typeof (Character))]
  public class CharacterActionWallStick: MonoBehaviour, CharacterAction, UpdateManagerAttach {
    [Comment("Vertical terminal velocity while stick")]
	  public float wallSlideSpeedMax = 3;

    [Comment("Time player need to oppose walkstick to leave / press in the other direction.")]
	  public float wallStickLeaveTime = 0.25f;

    [Comment("If you want to enable Jumping. Use empty string to disable.")]
    public string jumpAction = "Jump";

    [Comment("Jump in the same direction as the wall. Climb")]
    public Vector2 wallJumpClimb = new Vector2(10, 35);

    [Comment("Jump with no direction pressed.")]
    public Vector2 wallJumpOff = new Vector2(20, 20);

    [Comment("Jump in the opposite direction")]
    public Vector2 wallLeap = new Vector2(20, 20);

	  float timeToWallStickLeave;

    PlatformerController input;
    Controller2D controller;
    Character character;

    public void Start() {
      input = GetComponent<PlatformerController>();
      controller = GetComponent<Controller2D> ();
      character = GetComponent<Character> ();

      timeToWallStickLeave = wallStickLeaveTime;
    }

    public void Attach(UpdateManager um) {
    }

    /// <summary>
    /// Tells the character we want to take control
    /// Positive numbers fight: Higher number wins
    /// TODO REVIEW Negative numbers are used to ignore fight, and execute.
    /// </summary>
    public int WantsToUpdate() {
      return (
        (controller.collisions.left || controller.collisions.right) &&
        !controller.collisions.below &&
        character.velocity.y < 0
        ) ? 7 : 0;
    }

    public void PerformAction(float delta) {
      int wallDirX = (controller.collisions.left) ? -1 : 1;
      float x = input.GetAxisRawX();

      if (character.velocity.y < -wallSlideSpeedMax) {
        character.velocity.y = -wallSlideSpeedMax;
      }

      if (timeToWallStickLeave > 0) {
        character.velocity.x = 0;

        if (x != wallDirX && x != 0) {
          timeToWallStickLeave -= delta;
        }
        else {
          timeToWallStickLeave = wallStickLeaveTime;
        }
      }
      else {
        timeToWallStickLeave = wallStickLeaveTime;
      }

      // jump
      if (input.IsActionButtonDown(jumpAction)) {
        if (wallDirX == x) {
          character.velocity.x = -wallDirX * wallJumpClimb.x;
          character.velocity.y = wallJumpClimb.y;
        } else if (x == 0) {
          character.velocity.x = -wallDirX * wallJumpOff.x;
          character.velocity.y = wallJumpOff.y;
        } else {
          character.velocity.x = -wallDirX * wallLeap.x;
          character.velocity.y = wallLeap.y;
        }
      }
    }

    public PostUpdateActions GetPostUpdateActions() {
      return PostUpdateActions.WORLD_COLLISIONS | PostUpdateActions.APPLY_GRAVITY;
    }
  }
}
