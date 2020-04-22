using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene { GameScene, Loading, }

    private static Action loaderCallbackAction;

    public static void Load(Scene sceneToLoad)
    {
        loaderCallbackAction = () => { SceneManager.LoadScene(sceneToLoad.ToString()); };

        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        if (loaderCallbackAction != null)
        {
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}
