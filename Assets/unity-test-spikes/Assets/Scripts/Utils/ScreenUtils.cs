#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

using System;
using UnityEngine;

namespace Ikigai.DontTouchTheSpikes.Utils
{
    public static class ScreenUtils
    {
#if UNITY_STANDALONE
        private const int HEIGHT = 960;
        private static readonly Vector2Int ASPECT = new(9, 16);

        // Automatically set screen resolution when launching the app
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetScreenResolution()
        {
            if (Application.isEditor)
                return;
            Debug.Log("Aspect was changed to 9:16");
            int width = Mathf.RoundToInt((float)HEIGHT * ASPECT.x / ASPECT.y);
            Screen.SetResolution(width, HEIGHT, false);
        }
#endif


#if UNITY_EDITOR
        // ReSharper disable PossibleNullReferenceException
        
        // From https://gist.github.com/Biodam/b0616918ea5c50c2c9e4b16e5bb1034b
        [InitializeOnLoad]
        internal static class GameViewUtils
        {
            public enum GameViewSizeType
            {
                AspectRatio,
                FixedResolution
            }

            private static readonly object GAME_VIEW_SIZES_INSTANCE;
            private static readonly MethodInfo GET_GROUP;
            private static readonly string SIZE_NAME = "Ikigai 9:16";

            static GameViewUtils()
            {
                Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                PropertyInfo instanceProp = singleType.GetProperty("instance");
                GET_GROUP = sizesType.GetMethod("GetGroup");
                GAME_VIEW_SIZES_INSTANCE = instanceProp.GetValue(null, null);
                
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
            
            private static void OnPlayModeStateChanged(PlayModeStateChange state)
            {
                if (state != PlayModeStateChange.EnteredPlayMode)
                    return;
                
                if (!(Mathf.Abs((float)Screen.width / Screen.height - (float)ASPECT.x / ASPECT.y) > .001f))
                    return;
                
                try
                {
                    ApplyDefaultRatio();
                    Debug.Log("Editor Aspect was changed to 9:16");
                
                    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning("Failed to set Editor Aspect to 9:16, please change it manually");
                }
            }

            [MenuItem("Tools/Apply Default 9:16 Game View")]
            public static void ApplyDefaultRatio()
            {
                GameViewSizeGroupType currentGroupType = GetCurrentGroupType();
                if (!SizeExists(currentGroupType, SIZE_NAME))
                    AddCustomSize(GameViewSizeType.AspectRatio, GameViewSizeGroupType.Standalone, 9, 16, SIZE_NAME);
                
                SetSize(FindSize(currentGroupType, SIZE_NAME));
                UpdateZoomAreaAndParent();
            }

            public static void SetSize(int index)
            {
                if (index == -1)
                    return;

                Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                PropertyInfo selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);
                selectedSizeIndexProp.SetValue(gvWnd, index, null);
            }

            public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
            {
                object group = GetGroup(sizeGroupType);
                MethodInfo addCustomSize = GET_GROUP.ReturnType.GetMethod("AddCustomSize");
                Type gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
                Type gvstType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
                ConstructorInfo ctor = gvsType.GetConstructor(new[] {gvstType, typeof(int), typeof(int), typeof(string)});
                object newSize = ctor.Invoke(new object[] {(int)viewSizeType, width, height, text});
                addCustomSize.Invoke(group, new[] {newSize});
            }
            
            public static void UpdateZoomAreaAndParent()
            {
                Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                MethodInfo updateZoomAreaAndParentMethod = gvWndType.GetMethod("UpdateZoomAreaAndParent",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);
                updateZoomAreaAndParentMethod.Invoke(gvWnd, null);
            }

            public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
            {
                return FindSize(sizeGroupType, text) != -1;
            }

            public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
            {
                object group = GetGroup(sizeGroupType);
                MethodInfo getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
                string[] displayTexts = getDisplayTexts.Invoke(group, null) as string[];
                for (int i = 0; i < displayTexts.Length; i++)
                {
                    string display = displayTexts[i];
                    // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
                    // so if we're querying a custom size text we substring to only get the name
                    // You could see the outputs by just logging
                    int pren = display.IndexOf('(');
                    if (pren != -1)
                        display = display.Substring(0, pren - 1); // -1 to remove the space before. This is very implementation-dependent
                    if (display == text)
                        return i;
                }
                return -1;
            }
            
            private static object GetGroup(GameViewSizeGroupType type)
            {
                return GET_GROUP.Invoke(GAME_VIEW_SIZES_INSTANCE, new object[] {(int)type});
            }

            public static GameViewSizeGroupType GetCurrentGroupType()
            {
                PropertyInfo getCurrentGroupTypeProp = GAME_VIEW_SIZES_INSTANCE.GetType().GetProperty("currentGroupType");
                return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(GAME_VIEW_SIZES_INSTANCE, null);
            }
        }
#endif
    }
}