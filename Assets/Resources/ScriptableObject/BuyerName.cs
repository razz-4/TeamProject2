using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyerName", menuName = "ScriptableObject/BuyerName")]
public class BuyerName : ScriptableObject
{
    [SerializeField] private List<string> buyerNameList;

    public List<string> BuyerNameList => buyerNameList;

    public string GetRandomName()
    {
        return buyerNameList[Random.Range(0, buyerNameList.Count)];
    }
}