using UnityEngine;
using UnityEngine.InputSystem;
using CozyHome.Audio;
using CozyHome.UI;

namespace CozyHome.Interaction
{
    /// <summary>
    /// Base class for props that respond to player interaction.
    /// </summary>
    public class InteractiveProp : MonoBehaviour
    {
        /// <summary>
        /// Called when the prop is clicked in the world. This example spawns a floating note at
        /// the prop's pointer or world-to-screen position so the visual appears where the interaction happened.
        /// </summary>
        protected virtual void OnMouseDown()
        {
            Vector3 notePosition = transform.position;

            if (UnityEngine.InputSystem.Pointer.current != null)
            {
                notePosition = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    notePosition = mainCamera.WorldToScreenPoint(transform.position);
                }
            }

            if (MusicPuzzleManager.Instance == null || !MusicPuzzleManager.Instance.IsObjectActiveInCurrentRound(this))
            {
                return;
            }

            EffectManager.Instance?.SpawnNoteAt(notePosition);
            MusicPuzzleManager.Instance.PlayInteractionSound(this);
        }
    }
}
