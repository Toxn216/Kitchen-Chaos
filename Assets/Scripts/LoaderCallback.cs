using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if(isFirstUpdate) // ... Позже, после завершения загрузки LoadingScene ...
                            //Loader.LoaderCallback(); // Загрузит GameScene
        {
            isFirstUpdate = false;

            Loader.LoaderCallback();
        }
    }
}
