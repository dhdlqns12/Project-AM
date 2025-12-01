using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private UnitBase unit;
    private bool isMoving = true;
    private float moveDirection;  // 1: 오른쪽, -1: 왼쪽

    public bool IsMoving => isMoving;

    public void Init(UnitBase unitBase)
    {
        unit = unitBase;

        // Team에 따라 이동 방향 결정
        moveDirection = unit.Team == Team.Player ? 1f : -1f;

        Debug.Log($"{unit.Data.Name} 이동 시작 (방향: {(moveDirection > 0 ? "->" : "<-")})");
    }

    private void Update()
    {
        if (!isMoving || unit == null || unit.IsDead) return;

        // 자동 이동
        transform.position += Vector3.right * moveDirection * unit.Data.MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 이동 중지 (전투 시작 시)
    /// </summary>
    public void Stop()
    {
        isMoving = false;
    }

    /// <summary>
    /// 이동 재개 (전투 종료 시)
    /// </summary>
    public void Resume()
    {
        if (unit != null && !unit.IsDead)
        {
            isMoving = true;
        }
    }
}
