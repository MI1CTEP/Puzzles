using UnityEngine;
using UnityEditor;

namespace MyGame.Gameplay
{
    [CustomEditor(typeof(Scenario))]
    public sealed class ScenarioEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty stagesProp = serializedObject.FindProperty("_scenarioStages");

            for (int i = 0; i < stagesProp.arraySize; i++)
            {
                SerializedProperty stage = stagesProp.GetArrayElementAtIndex(i);
                SerializedProperty typeStage = stage.FindPropertyRelative("_typeStage");
                SerializedProperty sprite = stage.FindPropertyRelative("_sprite");
                SerializedProperty puzzleValueX = stage.FindPropertyRelative("_puzzleValueX");
                SerializedProperty videoClip = stage.FindPropertyRelative("_videoClip");
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("↑", GUILayout.Width(20)) && i > 0)
                    stagesProp.MoveArrayElement(i, i - 1);

                if (GUILayout.Button("↓", GUILayout.Width(20)) && i < stagesProp.arraySize - 1)
                    stagesProp.MoveArrayElement(i, i + 1);

                typeStage.enumValueIndex = (int)(TypeStage)EditorGUILayout.EnumPopup((TypeStage)typeStage.enumValueIndex);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    stagesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                switch ((TypeStage)typeStage.enumValueIndex)
                {
                    case TypeStage.SetPuzzle:
                        EditorGUILayout.PropertyField(sprite);
                        EditorGUILayout.PropertyField(puzzleValueX);
                        break;

                    case TypeStage.SetVideo:
                        EditorGUILayout.PropertyField(videoClip);
                        break;

                    case TypeStage.SetDialogue:
                        EditorGUILayout.PropertyField(sprite);
                        SerializedProperty dialogue = stage.FindPropertyRelative("_simpleDialogue");
                        SerializedProperty firstPhrase = dialogue.FindPropertyRelative("_firstPhrase");
                        EditorGUILayout.PropertyField(firstPhrase);
                        SerializedProperty phraseVariantsProp = dialogue.FindPropertyRelative("_phraseVariants");
                        for (int j = 0; j < phraseVariantsProp.arraySize; j++)
                        {
                            SerializedProperty phraseVariants = phraseVariantsProp.GetArrayElementAtIndex(j);
                            SerializedProperty answer = phraseVariants.FindPropertyRelative("_answer");
                            SerializedProperty respect = phraseVariants.FindPropertyRelative("_respect");
                            SerializedProperty secondPhrase = phraseVariants.FindPropertyRelative("_secondPhrase");

                            EditorGUILayout.BeginHorizontal("Box");
                            EditorGUILayout.BeginVertical("Box");
                            EditorGUILayout.PropertyField(answer);
                            EditorGUILayout.PropertyField(respect);
                            EditorGUILayout.PropertyField(secondPhrase);
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("Box");
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                phraseVariantsProp.DeleteArrayElementAtIndex(j);
                                break;
                            }

                            if (GUILayout.Button("↑", GUILayout.Width(20)) && j > 0)
                                phraseVariantsProp.MoveArrayElement(j, j - 1);

                            if (GUILayout.Button("↓", GUILayout.Width(20)) && i < phraseVariantsProp.arraySize - 1)
                                phraseVariantsProp.MoveArrayElement(j, j + 1);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();

                        }
                        if (GUILayout.Button("Добавить вариант"))
                        {
                            phraseVariantsProp.arraySize++;
                        }
                        break;
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Добавить этап"))
            {
                stagesProp.arraySize++;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}