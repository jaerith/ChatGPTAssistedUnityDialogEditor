using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIDialogist : MonoBehaviour, IRaycastable
    {
        [SerializeField]
        string characterName = null;

        [SerializeField]
        Dialogue characterDialogue = null;

        public string CharacterName { get { return characterName; } }

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (characterDialogue == null) 
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var playerDialogist = callingController.GetComponent<PlayerDialogist>();

                playerDialogist?.StartDialogue(this, characterDialogue);
            }

            return true;
        }
    }
}
