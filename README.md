# ChatGPT Assisted Unity Dialog Editor
A dialog editor plugin for the Unity Editor, with the enhancement of ChatGPT assisting in the writing of static dialog and possibly being an active, dynamic part of in-game dialogue (via a proxy).

## Overview 
This project is jointly inspired by the [Unity Dialogue Quests](https://www.udemy.com/course/unity-dialogue-quests) Udemy course (created by the GameDev.tv Team) and the [Unity ChatGPT Experiments](https://github.com/jaerith/UnityChatGPT) repo by Dilmer Valecillos.  The main goal of this project is to provide in-editor assistance when creating dialog text that is associated with gameplay (like quests) but is not essential to the overall story arc.

The forementioned Udemy course teaches one how to create a customizable dialog editor for Unity games, providing the developer with an embedded GUI for creating DAGs; each of these DAGs represent the flow of conversation (i.e., a dialog) between the player and NPCs.  Other useful features are available, such as the potential to fire events based on reaching certain nodes within the graph. 

This project takes the dialog editor one step further by providing the option to use ChatGPT as an assistant when creating the DAG and its embedded text.  As the developer, you can request for ChatGPT to write the text at certain nodes during test simulations, and when you're happy with the generated text, you can flip the switch on individual nodes and leave that text as static for the final edition of the dialog instance (i.e., for the shipped version of the game).
 
However, there is also interest in the option for certain nodes of a dialog to be left as open in the shipped version, dynamically altered by ChatGPT (via a proxy) during an actual session of the game.  This option, while possible and very interesting, has not been explored thoroughly in the project; it can create unpredictable results and isn't generally recommended at the moment.

## Example of Usage

Let's assume that your game has an area where the NPCs all tell riddles, where each has its own set of consequences.  You'd like to position a number of characters throughout the scene, each with its own riddle.  How would this tool help with that?

### Preparation

First, you would need to create a "seed" (i.e., a template) that is attached to each character, configuring each NPC with the same intention of using ChatGPT to generate the riddle and other associated text.  This [seed example](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Scripts/Dialogue/ChatGPT/SeedExamples/RiddlingGuardConversationSeed.json) demonstrates how you could configure such a NPC, by adding a ChatGPT Controller to the NPC and providing it with the forementioned seed.  When this seed is used in conjunction with the text in a dialog instance, the results from ChatGPT can be more contextual and relevant to the presented situation. 

Next, you would need to create the dialog instance via the editor and attach it to your NPC.  (The Udemy course describes how to build and use the dialog editor, which can be summarized by another section here in the future.)  The dialog editor has two type of nodes in the graph, both of which have static text belonging to the dialog: grey nodes and blue nodes.  The grey nodes represent text spoken by the NPC, while the blue nodes represent text spoken by the player.  With the enhancement provided by this project, there is now the possibility of a turqoise node, which has a placeholder for NPC text that will be supplied by ChatGPT during testing/gameplay.

Once the dialog graph has been created, it should resemble the following, with the leftmost node being the starting point and the dialog's graph flow proceeding to the right.  In this case, a grey [NPC node](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep01.png) starts the conversation, with the properties for the node appearing in the Inspector panel to the right.  This node is then followed by a blue [player node](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep02.png), which has its own text and properties to display.  And the subsequent node is a turquoise [ChatGPT node](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep03.png), which will be dynamically updated when we test the game by running it within the Unity editor:

![Step 03](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep03.png)

### Invocation of ChatGPT

Now, when the game is run within the Unity editor and the dialog is fired, we will observe the following happen:

⚔️ Guard: "Allo, peasant.  Hmmm...I'll let you by if you answer one of me riddles.  Up for it, mate?"
![NPC Static](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep05.png)

🧙‍♂️ Player: "Go ahead, gov.  I'll take that challenge."
![Player Static](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep06.png)

And then the riddle will be dynamically provided by ChatGPT:
![NPC Dynamic](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep07.png)

### Keeping ChatGPT Results

You'll notice that once the riddle has been presented, the turquoise ChatGPT node (shown below in the dialog editor) is now updated with the riddle text provided by ChatGPT.  If we're not satisfied with the riddle provided, we can keep running the game within the editor and trying different ones.  Eventually, we should arrive at a satisfactory option and then keep the text by flipping the ChatGPT flag in the Inspector panel, turning the node into a static NPC (i.e., grey) node:

![Keeping ChatGPT Results](https://github.com/jaerith/ChatGPTAssistedUnityDialogEditor/blob/main/Screenshots/RiddleStep08.png)

We can now repeat the process with other nodes in the DAG, letting ChatGPT assist with providing text for them.
