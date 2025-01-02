﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static sharedActionSystem;

public class sharedActionContent : MonoBehaviour
{
    [Header ("Main Settings")]
    [Space]

    public bool firstCharacterIsPlayer;

    [Space]
    [Header ("Match Settings")]
    [Space]

    public bool adjustPositionToFirstCharacter;
    public bool adjustPositionToSecondCharacter;

    [Space]

    public bool matchPositionUsedOnAction;
    public bool adjustMatchPositionOnFirstCharacter;
    public bool adjustMatchPositionOnSecondCharacter;

    [Space]

    public bool alignMainSharedActionGameObjectToBothCharacters;

    [Space]
    [Header ("Probability Settings")]
    [Space]

    public bool useProbabilityToUseAction;
    [Range (0, 100)] public float probabilityToUseAction;


    [Space]
    [Header ("Condition Settings")]
    [Space]

    public bool useMinDistanceToActivateAction;
    public float minDistanceToActivateAction;

    public bool useMinAngleToActivateAction;
    public float minAngleToActivateAction;

    [Space]

    public bool checkCharacterStats;

    public List<sharedActionConditionStatInfo> sharedActionConditionStatInfoList = new List<sharedActionConditionStatInfo> ();

    [Space]
    [Space]

    public bool stopActionIfAnyCharacterIsDead = true;
    public bool stopActionIfFirstCharacterIsDead;
    public bool stopActionIfSecondCharacterIsDead;

    [Space]
    [Header ("Components")]
    [Space]

    public actionSystem firstCharacterActionSystem;
    public actionSystem secondCharacterActionSystem;


    [System.Serializable]
    public class sharedActionConditionStatInfo
    {
        [Header ("Main Settings")]
        [Space]

        public string statName;

        public bool statIsAmount;

        public bool checkStatIsHigher;

        public float statAmount;

        public bool stateValue;
    }
}
