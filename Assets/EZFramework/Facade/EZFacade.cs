/*
 * Author:      熊哲
 * CreateTime:  1/16/2017 2:10:50 PM
 * Description:
 * 
*/
using UnityEngine;

namespace EZFramework
{
    public class EZFacade : EZSingleton<EZFacade>
    {
        [SerializeField]
        private bool hideFacade = false;
        [SerializeField]
        private bool useDefaultLogHandler = true;
        [SerializeField]
        [Tooltip("Don't use 'Develop Mode' here.")]
        private EZSettings.RunMode runModeInApp = EZSettings.RunMode.Local;

        private ILogHandler defaultLogHandler = Debug.logger.logHandler;

        void Start()
        {
#if !UNITY_EDITOR
            if (runModeInApp == EZSettings.RunMode.Develop) runModeInApp = EZSettings.RunMode.Local;
            EZSettings.Instance.runMode = runModeInApp;
#endif
            gameObject.hideFlags = hideFacade ? HideFlags.HideInHierarchy : HideFlags.None;
            if (useDefaultLogHandler)
            {
                Debug.logger.logHandler = defaultLogHandler;
            }
            else
            {
                string logPath = EZUtility.persistentDirPath + "EZLog/";
                Debug.logger.logHandler = new EZLogHandler(logPath);
            }

            Screen.sleepTimeout = (int)EZSettings.Instance.sleepTimeout;
            Application.runInBackground = EZSettings.Instance.runInBackground;
            Application.targetFrameRate = EZSettings.Instance.targetFrameRate;

            EZNetwork.Instance.Init();
            EZUpdate.Instance.Init();
            EZUpdate.Instance.StartUpdate(delegate ()
            {
                EZDatabase.Instance.Init();
                EZResource.Instance.Init();
                EZUI.Instance.Init();
                EZSound.Instance.Init();
                EZLua.Instance.Init();
            });
        }
        void OnApplicationQuit()
        {
            EZLua.Instance.Exit();
            EZSound.Instance.Exit();
            EZUI.Instance.Exit();
            EZResource.Instance.Exit();
            EZDatabase.Instance.Exit();
            EZUpdate.Instance.Exit();
            EZNetwork.Instance.Exit();
            Debug.logger.logHandler = defaultLogHandler;
        }
    }
}