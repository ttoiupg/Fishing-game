// Copyright (c) 2025 PinePie. All rights reserved.

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PinePie.PieTabs
{
    public static partial class Navigator
    {
        private static MethodInfo showFolderContents;

        // internal values fetcher
        private static float GetSideRectWidth(EditorWindow win)
        {
            if (win.GetType().ToString() == "UnityEditor.ProjectBrowser")
            {
                var type = win.GetType();
                FieldInfo Field = type.GetField("m_DirectoriesAreaWidth", BindingFlags.NonPublic | BindingFlags.Instance);

                if (Field != null)
                {
                    var rect = Field.GetValue(win);
                    return (float)rect;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public static bool IsTwoColumnMode()
        {
            var projectBrowsers = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            foreach (EditorWindow window in projectBrowsers.Cast<EditorWindow>())
            {
                if (window.GetType().ToString() == "UnityEditor.ProjectBrowser")
                {
                    var viewModeField = window.GetType().GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (viewModeField != null)
                    {
                        object viewModeValue = viewModeField.GetValue(window);
                        return viewModeValue.ToString() == "TwoColumns";
                    }
                }
            }
            return false;
        }

        public static VisualElement GetProjectBrowserUI()
        {
            var projectBrowsers = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            foreach (EditorWindow window in projectBrowsers.Cast<EditorWindow>())
            {
                if (window.GetType().ToString() == "UnityEditor.ProjectBrowser")
                    return window.rootVisualElement;
            }

            return new();
        }

        public static EditorWindow GetWin()
        {
            var projectBrowsers = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            foreach (EditorWindow window in projectBrowsers.Cast<EditorWindow>())
            {
                if (window.GetType().ToString() == "UnityEditor.ProjectBrowser")
                    return window;
            }

            return new();
        }

        public static string GetActiveFolderPath()
        {
            var win = GetWin();
            if (win == null) return "Assets/";

            MethodInfo method = win.GetType().GetMethod("GetActiveFolderPath", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                string result = method.Invoke(win, null) as string;
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return "Assets/";
        }

        public static void OpenFolder(string FolderPath)
        {
            var win = GetWin();
            if (win == null) return;

            if (showFolderContents == null)
            {
                showFolderContents = win.GetType().GetMethod(
                    "ShowFolderContents",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(int), typeof(bool) },
                    null);
            }

            Object obj = AssetDatabase.LoadAssetAtPath<Object>(FolderPath);

            if (showFolderContents != null) showFolderContents.Invoke(win, new object[] { obj.GetInstanceID(), true });
            else AssetDatabase.OpenAsset(obj);
        }

        public static void FocusAsset(string assetPath)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorUtility.FocusProjectWindow();
            }
            else
            {
                Debug.LogError("Asset not found at path: " + assetPath);
            }
        }


        // placeholder helper
        public static int PlaceholderNeedleAtPos(VisualElement area, float mouseX)
        {
            bool containsPlaceholder = area.Contains(UI.placeholderNeedle);

            // getting drop index
            int dropIndex = area.childCount;
            for (int i = area.childCount - 1; i >= 0; i--)
            {
                var child = area[i];

                if (containsPlaceholder && child == UI.placeholderNeedle) continue;

                if (true)
                {
                    if (mouseX > child.layout.center.x) dropIndex--;
                    else break;
                }
            }
            if (containsPlaceholder) dropIndex--;

            if (dropIndex != placeHolderIndex)
            {
                placeHolderIndex = dropIndex;

                UI.placeholderNeedle.RemoveFromHierarchy();
                area.Insert(dropIndex, UI.placeholderNeedle);
            }

            return dropIndex;
        }

        // loader
        private static VisualTreeAsset LoadUXML(string relativePath) =>
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PathUtility.GetPieTabsPath()}/PinePie/PieTabs/editor/Core/UI/{relativePath}");


        // button setup 
        public static void SetShadeProperties(VisualElement button, bool isMinimal, string Label, Texture2D iconTexture)
        {
            // shade
            var buttonShade = button.Q<VisualElement>("shade");
            buttonShade.Q<Label>("buttonLabel").text = isMinimal ? "" : Label;
            buttonShade.Q<VisualElement>("buttonIcon").style.backgroundImage = new StyleBackground(iconTexture);
            button.RegisterCallback<MouseEnterEvent>(_ =>
            {
                buttonShade.style.backgroundColor = new Color(255, 255, 255, 0.06f);
            });
            button.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                buttonShade.style.backgroundColor = new Color(255, 255, 255, 0f);
            });
        }

        public static void SetupButtonProperties(bool isCreator, VisualElement button, bool isMinimal, string Label, string color)
        {
            // color and tooltip
            Color col = HexToColor(color);

            button.style.backgroundColor = col;
            button.tooltip = isMinimal ? Label : null;

            // label props
            var labelElement = button.Q<Label>("buttonLabel");
            if (labelElement != null)
            {
                string lbl = isCreator ? "" : null;
                labelElement.text = isMinimal ? lbl : Label;

                labelElement.style.color = IsColorDark(col)
                ? HexToColor("#f7f7f7")
                : HexToColor("#2e2e2e");
            }
        }


        // color setup helpers
        public static bool IsColorDark(Color color)
        {
            float brightness = (color.r * 0.299f) + (color.g * 0.587f) + (color.b * 0.114f);
            return brightness < 0.5f;
        }

        public static string ColorToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }

        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return HexToColor("#3E3E3E");

            if (!hex.StartsWith("#"))
                hex = "#" + hex;

            hex = hex.ToUpperInvariant();

            if (ColorUtility.TryParseHtmlString(hex, out Color color))
                return color;

            return HexToColor("#3E3E3E");
        }

    }
}

#endif