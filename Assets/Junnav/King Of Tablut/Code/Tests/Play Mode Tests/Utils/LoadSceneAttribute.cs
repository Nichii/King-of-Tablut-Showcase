using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Junnav.KingOfTablut.PlayModeTests
{
    public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly string scene = string.Empty;
        private Scene loadedTestScene;

        private readonly bool overrideSetTimeScale = false;
        private readonly float overrideTimeScale = 0f;

        public LoadSceneAttribute(string scene)
        {
            this.scene = scene;
        }

        public LoadSceneAttribute(string scene, float overrideTimeScale)
        {
            this.scene = scene;
            overrideSetTimeScale = true;
            this.overrideTimeScale = overrideTimeScale;
        }

        IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
        {
            Debug.Assert(scene.EndsWith(".unity"));

            yield return SceneManager.LoadSceneAsync(scene, new LoadSceneParameters(LoadSceneMode.Additive));
            
            loadedTestScene = SceneManager.GetSceneByPath(scene);

            // This allows game object to instantiate in test scene
            SceneManager.SetActiveScene(loadedTestScene);

            if (overrideSetTimeScale)
            {
                yield return null;

                Time.timeScale = overrideTimeScale;
            }
        }

        IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
        {
            SceneManager.UnloadSceneAsync(loadedTestScene);
            yield return null;
        }
    }
}
