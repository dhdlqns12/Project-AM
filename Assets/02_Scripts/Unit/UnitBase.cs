using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    Enemy
}

public abstract class UnitBase : MonoBehaviour
{
    [Header("유닛 정보")]
    [SerializeField] private Team team;
    public Team Team => team;

    public UnitData Data { get; private set; }
}
