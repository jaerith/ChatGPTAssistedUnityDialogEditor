using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace RPG.DialogueNS.Editor
{
    public class DialogueModficationProcessor : UnityEditor.AssetModificationProcessor
    {
        /*
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            var dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialogue;

            if (dialogue == null) 
            {
                return AssetMoveResult.DidNotMove;
            }

            if (Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath))
            {
                return AssetMoveResult.DidNotMove;
            }

            dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);

            return AssetMoveResult.DidNotMove;
        }
        */
    }
}
