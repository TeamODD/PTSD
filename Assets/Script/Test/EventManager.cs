using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{
    [Header("대상 포탈들")]
    [Tooltip("플레이어가 포탈과 상호작용 시 이동할 대상 포탈들의 Transform 리스트")]
    public List<GameObject> destinationPortals;


}
