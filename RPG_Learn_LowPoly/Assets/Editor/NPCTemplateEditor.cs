using RPG.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharactersData))]
public class NPCTemplateEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    CharactersData npcTemplate = (CharactersData)target;

    //    // Mostrar campos comuns a todos os comportamentos
    //    EditorGUILayout.LabelField("Common Settings", EditorStyles.boldLabel);
    //    npcTemplate.behaviorType = (BehaviorType)EditorGUILayout.EnumPopup("Behavior Type", npcTemplate.behaviorType);
    //    Debug.Log(npcTemplate.behaviorType);
    //    // Mostrar campos específicos com base no tipo de comportamento
    //    switch (npcTemplate.behaviorType)
    //    {

    //        case BehaviorType.Chase:
    //            ShowChaseFields(npcTemplate);
    //            break;
    //        case BehaviorType.AttackAndFlee:
    //            ShowAttackAndFleeFields(npcTemplate);
    //            break;
    //            // Adicione outros casos para outros comportamentos
    //    }
    //}

    //private void ShowChaseFields(CharactersData npcTemplate)
    //{
    //    // Mostrar campos específicos para o comportamento de perseguição
    //    EditorGUILayout.LabelField("Chase Settings", EditorStyles.boldLabel);
    //    npcTemplate.velocidadePerseguicao = EditorGUILayout.FloatField("Chase Speed", npcTemplate.velocidadePerseguicao);
    //    // Adicione mais campos específicos, se necessário
    //}

    //private void ShowAttackAndFleeFields(CharactersData npcTemplate)
    //{
    //    // Mostrar campos específicos para o comportamento de ataque e fuga
    //    EditorGUILayout.LabelField("Attack and Flee Settings", EditorStyles.boldLabel);
    //    npcTemplate.distanciaAtaque = EditorGUILayout.FloatField("Attack Distance", npcTemplate.distanciaAtaque);
    //    npcTemplate.distanciaFuga = EditorGUILayout.FloatField("Flee Distance", npcTemplate.distanciaFuga);
    //    // Adicione mais campos específicos, se necessário
    //}
}
