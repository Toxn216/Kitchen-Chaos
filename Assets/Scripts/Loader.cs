using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene //состо€ни€ игры
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }

    private static Scene targetScene;//переменна€ дл€ храннени€ текущего состо€ни€


    public static void Load(Scene targetScene)//этот метод загружает сцену загрузки
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());     
    }

    public static void LoaderCallback()//обратный вызов после загрузки сцены LoadingScene.
                                       //ќн загружает ту сцену, котора€ была сохранена в поле targetScene в методе Load.
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
