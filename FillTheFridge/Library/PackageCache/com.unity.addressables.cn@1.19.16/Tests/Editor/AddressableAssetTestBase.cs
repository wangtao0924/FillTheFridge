using System;
using System.IO;
using NUnit.Framework;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnityEditor.AddressableAssets.Tests
{
    public abstract class AddressableAssetTestBase
    {
        protected const string k_TestConfigName = "AddressableAssetSettings.Tests";

        protected string TestFolder => $"Assets/{TestFolderName}";
        protected string TestFolderName => $"{GetType()}_Tests";
        protected string ConfigFolder => TestFolder + "/Config";
        
        
        protected string GetAssetPath(string assetName)
        {
            return $"{TestFolder}/{assetName}";
        }

        private AddressableAssetSettings m_Settings;

        protected AddressableAssetSettings Settings
        {
            get
            {
                if (m_Settings == null)
                    m_Settings = AddressableAssetSettings.Create(ConfigFolder, k_TestConfigName, true, PersistSettings);
                return m_Settings;
            }
        }
        protected string m_AssetGUID;
        protected string[] m_SceneGuids;

        protected virtual bool PersistSettings { get { return true; } }
        [OneTimeSetUp]
        public void Init()
        {
            //TODO: Remove when NSImage warning issue on bokken is fixed
            Application.logMessageReceived += CheckLogForWarning;

            if (Directory.Exists(TestFolder))
            {
                Debug.Log($"{GetType()} (init) - deleting {TestFolder}");
                if (!AssetDatabase.DeleteAsset(TestFolder))
                    Directory.Delete(TestFolder);
            }
            
            Debug.Log($"{GetType()} (init) - creating {TestFolder}");
            AssetDatabase.CreateFolder("Assets", TestFolderName);
            AssetDatabase.CreateFolder(TestFolder, "Config");

            Settings.labelTable.labelNames.Clear();
            GameObject testObject = new GameObject("TestObject");
            GameObject testObject1 = new GameObject("TestObject 1");
            GameObject testObject2 = new GameObject("TestObject 2");
          
            PrefabUtility.SaveAsPrefabAsset(testObject, TestFolder + "/test.prefab");
            PrefabUtility.SaveAsPrefabAsset(testObject1, TestFolder + "/test 1.prefab");
            PrefabUtility.SaveAsPrefabAsset(testObject2, TestFolder + "/test 2.prefab");
            m_AssetGUID = AssetDatabase.AssetPathToGUID(TestFolder + "/test.prefab");

            string scene1Path = TestFolder + "/contentUpdateScene1.unity";
            string scene2Path = TestFolder + "/contentUpdateScene2.unity";
            string scene3Path = TestFolder + "/contentUpdateScene3.unity";

            Scene scene1 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            EditorSceneManager.SaveScene(scene1, scene1Path);

            Scene scene2 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            EditorSceneManager.SaveScene(scene2, scene2Path);

            Scene scene3 = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            EditorSceneManager.SaveScene(scene3, scene3Path);

            //Clear out the active scene so it doesn't affect tests
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            m_SceneGuids = new string[]
            {
                AssetDatabase.AssetPathToGUID(scene1Path),
                AssetDatabase.AssetPathToGUID(scene2Path),
                AssetDatabase.AssetPathToGUID(scene3Path)
            };

            OnInit();

            //TODO: Remove when NSImage warning issue on bokken is fixed
            //Removing here in the event we didn't recieve any messages during the setup, we can respond appropriately to
            //logs in the tests.
            Application.logMessageReceived -= CheckLogForWarning;
            if (resetFailingMessages)
                LogAssert.ignoreFailingMessages = false;
        }

        private bool resetFailingMessages = false;
        //TODO: Remove when NSImage warning issue on bokken is fixed
        private void CheckLogForWarning(string condition, string stackTrace, LogType type)
        {
            LogAssert.ignoreFailingMessages = true;
            resetFailingMessages = true;
        }

        protected virtual void OnInit() {}

        [OneTimeTearDown]
        public void Cleanup()
        {
            OnCleanup();
            if (Directory.Exists(TestFolder))
            {
                Debug.Log($"{GetType()} - (cleanup) deleting {TestFolder}");
                AssetDatabase.DeleteAsset(TestFolder);
            }
            EditorBuildSettings.RemoveConfigObject(k_TestConfigName);
        }

        protected virtual void OnCleanup()
        {
        }

        protected string CreateAsset(string assetPath, string objectName = null)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (string.IsNullOrEmpty(objectName))
                objectName = Path.GetFileNameWithoutExtension(assetPath);
            go.name = objectName;
            //this is to ensure that bundles are different for every run.
            go.transform.localPosition = UnityEngine.Random.onUnitSphere;
            PrefabUtility.SaveAsPrefabAsset(go, assetPath);
            UnityEngine.Object.DestroyImmediate(go, false);
            return AssetDatabase.AssetPathToGUID(assetPath);
        }

        protected string CreateFolderDeep(string path)
        {
            if (!path.StartsWith("Assets/"))
                return null;
            var split = path.Split('/');
            string fullPath = "Assets";
            string parentFolder;
            string guid = null;
            for (int i = 1; i < split.Length; ++i)
            {
                parentFolder = fullPath;
                fullPath += "/" + split[i];
                if (!AssetDatabase.IsValidFolder(fullPath))
                    guid = AssetDatabase.CreateFolder(parentFolder, split[i]);
            }

            return guid;
        }
    }
}
