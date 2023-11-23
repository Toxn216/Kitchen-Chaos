using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{


    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;

}
