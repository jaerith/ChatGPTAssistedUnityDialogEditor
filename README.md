# ChatGPT Assisted Unity Dialog Editor
A dialog editor plugin for the Unity Editor, with the enhancement of ChatGPT assisting in the writing of static dialog and possibly being an active, dynamic part of in-game dialogue (via a proxy).

## Overview 
This project is jointly inspired by the [Unity Dialogue Quests](https://www.udemy.com/course/unity-dialogue-quests) Udemy course (created by the GameDev.tv Team) and the [Unity ChatGPT Experiments](https://github.com/jaerith/UnityChatGPT) repo by Dilmer Valecillos.  

The forementioned Udemy course teaches one how to create a customizable dialog editor for Unity games, providing the developer with an embedded GUI for creating DAGs; each of these DAGs represent the flow of conversation (i.e., a dialog) between the player and NPCs.  Other useful features are available, such as the potential to fire events based on reaching certain nodes within the graph. 
 This project takes the dialog editor one step further by providing the option to use ChatGPT as an assistant when creating the DAG and its embedded text.  As the developer, you can request for ChatGPT to write the text at certain nodes during test simulations, and when you're happy with the generated text, you can flip the switch on individual nodes and leave that text as static for the final edition of the dialog instance (i.e., for the shipped version of the game).
 
However, perhaps there is also interest in the option for certain nodes of a dialog to be left as open, dynamically altered by ChatGPT during an actual session of the game (via a proxy).  This option, while possible, can create unpredictable results and isn't recommended.
