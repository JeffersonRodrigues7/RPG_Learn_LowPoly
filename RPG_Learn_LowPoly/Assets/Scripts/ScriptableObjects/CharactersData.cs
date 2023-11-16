using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Character
{
    [CreateAssetMenu(fileName = "CharactersData", menuName = "Characters Data")]
    public class CharactersData : ScriptableObject
    {
        [Header("Basic")]
        public string _id = "0";
        public string _name = "Meditrax";
        [TextArea] public string _description = "Primeiro Inimigo";
        public string _type = "Bestial";

        [Header("Detection")]
        public BehaviorType _behavior;
        public bool _chaseEnemyBehavior = true;
        public float _detectionDistance = 10f;
        public float _attackDistance = 1f;

        [Header("Movement")]
        public float _walkSpeed = 3f;
        public float _chaseSpeed = 5f;
        public float _cooldownTimeAfterChase = 2f;
        public float _arrivalDistance = 0.1f;
        public Transform[] _patrolPoints;

        [Header("Attack")]
        public float _damage = 5f;

        [Header("Health")]
        public float _maxHealth = 100f;

        [Header("Objects")]
        public AnimatorOverrideController _animatorOverrideController;
        public GameObject _prefab;
    }

    public enum BehaviorType
    {
        Dialogue, 
        Trader,
        MeleeAttack,
        RangedAttack,
    }

}

