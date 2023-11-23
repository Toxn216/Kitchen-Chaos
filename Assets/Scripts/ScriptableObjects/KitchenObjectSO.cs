using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu()]//нужная штука что б все работала но пока хз что это=)
public class KitchenObjectSO : ScriptableObject//подрубаем юнити класс скриптабле, он для перечесления обьектов как я понял
{
    public Transform prefab;//сам прифаб китчен обьекта
    public Sprite sprite;//его спрайт
    public string objectName;//и имя
}
