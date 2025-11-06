// Copyright (c) 2025 PinePie. All rights reserved.

#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PinePie.PieTabs
{
    [InitializeOnLoad]
    public static class NavigatorLoader
    {
        static NavigatorLoader() => EditorApplication.update += RunOnceOnLoad;

        static void RunOnceOnLoad()
        {
            EditorApplication.update -= RunOnceOnLoad;

            if (!Directory.Exists($"{PathUtility.GetPieTabsPath()}/PinePie/PieTabs"))
                return;

            Navigator.Setup();
            UI.mainUI.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                Selection.selectionChanged -= EnsurePieDeskOverlay;

                EditorApplication.delayCall += () =>
                {
                    var projectWindow = Navigator.GetProjectBrowserUI();
                    if (UI.mainUI.panel == null)
                    {
                        Selection.selectionChanged += EnsurePieDeskOverlay;
                    }
                };
            });
        }

        private static void EnsurePieDeskOverlay()
        {
            var lastFocused = EditorWindow.focusedWindow;
            if (lastFocused != null && lastFocused.GetType().Name == "ProjectBrowser")
            {
                Selection.selectionChanged -= EnsurePieDeskOverlay;
                Navigator.Setup();
            }
        }

        [MenuItem("Tools/Refresh PieTabs")]
        public static void RefreshPieDesk()
        {
            Navigator.Setup();
        }
    }

    public static partial class Navigator
    {
        public static ShortcutButtonBundle navButtons = new();
        public static CreatorButtonBundle creatorButtons = new();

        private const string SplitterKey = "PieTabs_LastSplitterSpacing";

        private static float LastSplitterSpacing
        {
            get => EditorPrefs.GetFloat(SplitterKey, 300f);
            set => EditorPrefs.SetFloat(SplitterKey, value);
        }

        private static int placeHolderIndex;


        public static void Setup()
        {
            UI.projectBrowserUI = GetProjectBrowserUI();
            UI.mainUI = LoadUXML("PieDeskMainUI.uxml").Instantiate().Q<VisualElement>("PieDeskUI");
            UI.mainUI.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PathUtility.GetPieTabsPath()}/PinePie/PieTabs/editor/Core/UI/PieDeskStyling.uss"));

            UI.shortcutButtonAsset = LoadUXML("shortcutButton.uxml");
            UI.creatorButtonAsset = LoadUXML("creatorButton.uxml");
            UI.placeholderNeedle = LoadUXML("placeholderLine.uxml").Instantiate().Q<VisualElement>("line");

            UI.copiedText = UI.mainUI.Q<VisualElement>("copiedText");

            UI.menuEntryFieldBox = UI.mainUI.Q<VisualElement>("NewFileNameField");
            UI.menuEntryField = UI.menuEntryFieldBox.Q<TextField>("entry");

            UI.colorPopup = UI.mainUI.Q<VisualElement>("colorPopup").Q<VisualElement>("colorPopup");


            UI.projectBrowserUI.Clear();

            UI.shortcutButtonArea = UI.mainUI.Q<ScrollView>("shorcutsDragArea");
            UI.creatorButtonArea = UI.mainUI.Q<ScrollView>("CreationMenuDragArea");
            if (!IsTwoColumnMode())
            {
                UI.shortcutButtonArea.style.display = DisplayStyle.None;
                UI.creatorButtonArea.style.flexGrow = 1;

                UI.mainUI.Q<VisualElement>("splitter").style.display = DisplayStyle.None;

                VisualElement bottomBar = UI.mainUI.Q<VisualElement>("bottomAddressBar");
                bottomBar.style.marginRight = 0;
                bottomBar.style.marginLeft = 0;

                UI.shortcutButtonArea = UI.mainUI.Q<ScrollView>("CreationMenuDragArea");
            }
            else // two coloumn mode 
            {
                SetupSplitter();
                SetupBottomBarMargin();
                UI.creatorButtonArea.style.width = LastSplitterSpacing;

                // asset creator button 
                creatorButtons.LoadFromJson(UI.creatorButtonAsset);
                SetupDragNDropForCreatorArea(UI.creatorButtonArea, creatorButtons);
                FillCreatorButtons();
            }

            CallbacksForPopupBoxes();
            RegisterAddressCopyCallbacks();
            SetupDragAreaStyling();
            SetupSearchBarControls();
            CallbacksForColorPopup();

            // shortcut buttons
            navButtons.LoadFromJson(UI.shortcutButtonAsset);
            SetupDragNDropForShortcutArea(UI.shortcutButtonArea, navButtons);
            FillShortcutButtons();


            UI.projectBrowserUI.Add(UI.mainUI);
        }


        // click callbacks
        public static void OnShortcutButtonClicked(
            VisualElement button,
            Action<VisualElement> depSelection,
            ShortcutButton buttonProp)
        {
            // callbacks
            var state = new ClickState();
            button.userData = state;

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(buttonProp.Path);
            button.AddManipulator(new ShortcutDragManipulator(obj));

            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                var s = (ClickState)button.userData;

                if (evt.button == 0)
                {
                    s.leftClicked = true;
                    button.Q<VisualElement>("shade").style.backgroundColor = new Color(255, 255, 255, 0.15f);

                    evt.StopPropagation();
                }
                else if (evt.button == 1)
                {
                    s.rightClicked = true;

                    evt.StopPropagation();
                }
            });
            button.RegisterCallback<PointerUpEvent>(evt =>
            {
                var s = (ClickState)button.userData;

                // single click
                if (s.leftClicked)
                {
                    if (obj == null) return;

                    if (evt.ctrlKey)
                    {
                        depSelection?.Invoke(button);

                        if (AssetDatabase.IsValidFolder(buttonProp.Path))
                            OpenFolder(buttonProp.Path);
                        else
                            AssetDatabase.OpenAsset(obj);


                        evt.StopPropagation();
                    }
                    else if (evt.shiftKey)
                    {
                        buttonProp.isMinimal = !buttonProp.isMinimal;
                        button.Q<Label>("buttonLabel").text = buttonProp.isMinimal ? "" : buttonProp.Label;
                        button.tooltip = buttonProp.isMinimal ? buttonProp.Label : null;

                        var buttonShade = button.Q<VisualElement>("shade");
                        buttonShade.Q<Label>("buttonLabel").text = buttonProp.isMinimal ? "" : buttonProp.Label;

                        navButtons.SaveToJson();

                        evt.StopPropagation();
                    }
                    else if (evt.altKey)
                    {
                        ShowBoxAtPos(UI.colorPopup, evt.position.x - 100);

                        ColorPopup.isForCreator = false;
                        ColorPopup.popupIsOpen = true;

                        ColorPopup.activeNavButton = buttonProp;
                        ColorPopup.activeVisualItem = button;

                        evt.StopPropagation();
                    }
                    else
                    {
                        depSelection?.Invoke(button);

                        // uncomment line below to open folder even in single click

                        // if (AssetDatabase.IsValidFolder(buttonProp.Path))
                        //     OpenFolder(buttonProp.Path);
                        // else
                        FocusAsset(buttonProp.Path);

                        evt.StopPropagation();
                    }
                }
                else if (s.rightClicked)
                {
                    RemoveButton(buttonProp);

                    evt.StopPropagation();
                }

                button.Q<VisualElement>("shade").style.backgroundColor = new Color(255, 255, 255, 0.12f);
                s.leftClicked = false;
                s.rightClicked = false;
            });

        }

        public static void OnAssetCreatorButtonClicked(
            VisualElement button,
            Action<VisualElement> depSelection,
            CreatorButton buttonProp)
        {
            // callbacks
            var state = new ClickState();
            button.userData = state;

            button.RegisterCallback<PointerDownEvent>(evt =>
            {
                var s = (ClickState)button.userData;

                if (evt.button == 0)
                {
                    s.leftClicked = true;
                    button.Q<VisualElement>("shade").style.backgroundColor = new Color(255, 255, 255, 0.15f);

                    evt.StopPropagation();
                }
                else if (evt.button == 1)
                {
                    s.rightClicked = true;

                    evt.StopPropagation();
                }
            });

            button.RegisterCallback<PointerUpEvent>(evt =>
            {
                var s = (ClickState)button.userData;

                // single click
                if (s.leftClicked)
                {
                    if (evt.ctrlKey)
                    {
                        ShowBoxAtPos(UI.menuEntryFieldBox, evt.position.x);
                        UI.menuEntryField.SetValueWithoutNotify(buttonProp.Label);
                        UI.menuEntryField.Focus();

                        FinalizeMenuEntry(buttonProp);

                        evt.StopPropagation();
                    }
                    // minimal mode
                    else if (evt.shiftKey)
                    {
                        buttonProp.isMinimal = !buttonProp.isMinimal;
                        button.Q<Label>("buttonLabel").text = buttonProp.isMinimal ? "" : buttonProp.Label;
                        button.tooltip = buttonProp.isMinimal ? buttonProp.Label : null;

                        var buttonShade = button.Q<VisualElement>("shade");
                        buttonShade.Q<Label>("buttonLabel").text = buttonProp.isMinimal ? "" : buttonProp.Label;

                        creatorButtons.SaveToJson();

                        evt.StopPropagation();
                    }
                    // color setting
                    else if (evt.altKey)
                    {
                        ShowBoxAtPos(UI.colorPopup, evt.position.x - 100);

                        ColorPopup.isForCreator = true;
                        ColorPopup.popupIsOpen = true;

                        ColorPopup.activeCreatorButton = buttonProp;
                        ColorPopup.activeVisualItem = button;

                        evt.StopPropagation();
                    }
                    else
                    {
                        depSelection?.Invoke(button);
                        AssetDatabase.Refresh();
                        EditorApplication.ExecuteMenuItem(buttonProp.menuEntry);

                        evt.StopPropagation();
                    }
                }
                else if (s.rightClicked)
                {
                    RemoveButton(buttonProp);

                    evt.StopPropagation();
                }

                s.leftClicked = false;
                s.rightClicked = false;
            });

        }


        // UI Setup

        // split bars
        public static void SetupDragAreaStyling()
        {
            foreach (var view in new VisualElement[] { UI.shortcutButtonArea, UI.creatorButtonArea })
            {
                view.contentContainer.style.flexDirection = FlexDirection.RowReverse;
                view.contentContainer.style.justifyContent = Justify.FlexStart;
            }
        }

        public static void SetupSplitter()
        {
            VisualElement splitter = UI.mainUI.Q<VisualElement>("splitter");
            bool isDragging = false;
            int pointerId = -1;
            float distFromMouse = 0;



            splitter.RegisterCallback<PointerDownEvent>(evt =>
            {
                isDragging = true;
                pointerId = evt.pointerId;

                float mousePosFromRight = UI.mainUI.resolvedStyle.width - evt.position.x;
                distFromMouse = UI.creatorButtonArea.resolvedStyle.width - mousePosFromRight;

                splitter.CapturePointer(pointerId);

                evt.StopPropagation();
            });

            splitter.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!isDragging || evt.pointerId != pointerId) return;

                float mousePos = UI.mainUI.resolvedStyle.width - evt.position.x;
                UI.creatorButtonArea.style.width = mousePos + distFromMouse;

                evt.StopPropagation();
            });

            splitter.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (evt.pointerId != pointerId) return;

                LastSplitterSpacing = UI.creatorButtonArea.resolvedStyle.width;
                isDragging = false;

                splitter.ReleasePointer(pointerId);
                evt.StopPropagation();
            });
        }

        public static void SetupSearchBarControls()
        {
            VisualElement splitter = UI.mainUI.Q<VisualElement>("splitter");
            VisualElement spacer = UI.mainUI.Q<VisualElement>("itemSpacer");
            VisualElement creatorButtonPopupAdder = UI.mainUI.Q<VisualElement>("createAssetButton");

            Button openSearchButton = UI.mainUI.Q<Button>("searchToggle");
            Button closeSearchButton = UI.mainUI.Q<Button>("closeSearchBar");

            ScrollView AssetCreatorButtonArea = UI.mainUI.Q<ScrollView>("CreationMenuDragArea");

            openSearchButton.clicked += () =>
            {
                closeSearchButton.style.display = DisplayStyle.Flex;
                closeSearchButton.style.marginRight = 465;

                splitter.style.display = DisplayStyle.None;
                spacer.style.display = DisplayStyle.None;

                AssetCreatorButtonArea.style.width = 0;
                openSearchButton.style.display = DisplayStyle.None;
            };

            closeSearchButton.clicked += () =>
            {
                closeSearchButton.style.display = DisplayStyle.None;
                closeSearchButton.style.marginRight = 0f;

                spacer.style.display = DisplayStyle.Flex;
                if (IsTwoColumnMode())
                {
                    splitter.style.display = DisplayStyle.Flex;
                }

                AssetCreatorButtonArea.style.width = LastSplitterSpacing;
                openSearchButton.style.display = DisplayStyle.Flex;
            };
        }


        // address copy from bottom bar
        public static void SetupBottomBarMargin()
        {
            VisualElement bottomAddressBar = UI.mainUI.Q<VisualElement>("bottomAddressBar");

            bottomAddressBar.style.marginLeft = GetSideRectWidth(GetWin());
        }

        public static void RegisterAddressCopyCallbacks()
        {
            VisualElement bottomAddressBar = UI.mainUI.Q<VisualElement>("bottomAddressBar");
            bottomAddressBar.RegisterCallback<MouseDownEvent>((evt) =>
            {
                if (IsTwoColumnMode()) bottomAddressBar.style.marginLeft = GetSideRectWidth(GetWin());

                string copyingStr = "";

                if (evt.button == 0)
                {
                    copyingStr = !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject))
                        ? AssetDatabase.GetAssetPath(Selection.activeObject)
                        : GetActiveFolderPath();
                }
                else if (evt.button == 1)
                {
                    string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    copyingStr = !string.IsNullOrEmpty(assetPath)
                        ? Path.GetFileName(assetPath)
                        : "";
                }

                EditorGUIUtility.systemCopyBuffer = copyingStr;

                ShowCopiedNotification(evt.mousePosition, UI.mainUI);
            });

            if (IsTwoColumnMode())
                Selection.selectionChanged += () =>
                {
                    SetupBottomBarMargin();
                };
        }

        public static void ShowCopiedNotification(Vector2 position, VisualElement root)
        {
            UI.copiedText.style.left = position.x - 20;

            UI.copiedText.style.display = DisplayStyle.Flex;

            root.schedule.Execute(() =>
            {
                UI.copiedText.style.display = DisplayStyle.None;
            }).ExecuteLater(1000);
        }


        // icon popup
        public static void ShowBoxAtPos(VisualElement box, float posX)
        {
            UI.mainUI.pickingMode = PickingMode.Position;

            float rightOffset = UI.mainUI.resolvedStyle.width - 200;

            box.style.left = Mathf.Clamp(posX, 0, rightOffset);

            box.style.display = DisplayStyle.Flex;
        }

        public static void CallbacksForPopupBoxes()
        {
            UI.mainUI.RegisterCallback<MouseDownEvent>((evt) =>
            {
                UI.menuEntryFieldBox.style.display = DisplayStyle.None;
                UI.colorPopup.style.display = DisplayStyle.None;

                ColorPopup.popupIsOpen = false;

                UI.mainUI.pickingMode = PickingMode.Ignore;
            });
        }

        public static void CallbacksForColorPopup()
        {
            foreach (Button child in UI.colorPopup.Query<Button>("icon").ToList())
            {
                child.clicked += () =>
                {
                    if (ColorPopup.popupIsOpen)
                    {
                        if (ColorPopup.isForCreator)
                        {
                            ColorPopup.activeCreatorButton.color = ColorToHex(child.resolvedStyle.backgroundColor);
                            creatorButtons.SaveToJson();
                        }
                        else
                        {
                            ColorPopup.activeNavButton.color = ColorToHex(child.resolvedStyle.backgroundColor);
                            navButtons.SaveToJson();
                        }

                        ColorPopup.activeVisualItem.style.backgroundColor = HexToColor(ColorPopup.isForCreator
                            ? ColorPopup.activeCreatorButton.color
                            : ColorPopup.activeNavButton.color);
                    }
                };
            }

            UI.colorPopup.RegisterCallback<MouseDownEvent>((evt) => evt.StopPropagation());
        }

        public static void FinalizeMenuEntry(CreatorButton button)
        {
            var capturedButtons = button;

            void OnKey(KeyDownEvent evt)
            {
                if ((evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && UI.menuEntryFieldBox.style.display == DisplayStyle.Flex)
                {
                    capturedButtons.menuEntry = "Assets/Create/" + UI.menuEntryField.value;

                    UI.menuEntryFieldBox.style.display = DisplayStyle.None;
                    UI.mainUI.pickingMode = PickingMode.Ignore;

                    FillCreatorButtons();
                    creatorButtons.SaveToJson();

                    evt.StopPropagation();
                    UI.menuEntryField.UnregisterCallback<KeyDownEvent>(OnKey);
                }
            }

            UI.menuEntryFieldBox.RegisterCallback<MouseDownEvent>((evt) => evt.StopPropagation());
            UI.menuEntryField.RegisterCallback<KeyDownEvent>(OnKey);
        }



        // filling and removing
        public static void FillShortcutButtons()
        {
            UI.shortcutButtonArea.Clear();

            foreach (var button in navButtons.buttons)
            {
                UI.shortcutButtonArea.Add(button.UIbutton);
            }
        }
        public static void FillCreatorButtons()
        {
            UI.creatorButtonArea.Clear();

            foreach (var button in creatorButtons.buttons)
            {
                UI.creatorButtonArea.Add(button.UIbutton);
            }
        }

        public static void RemoveButton(ShortcutButton toRemove)
        {
            navButtons.RemoveButton(toRemove);

            FillShortcutButtons();
        }
        public static void RemoveButton(CreatorButton toRemove)
        {
            creatorButtons.RemoveButton(toRemove);

            FillCreatorButtons();
        }


        // shortcut bar dragging
        public static void SetupDragNDropForShortcutArea(VisualElement area, ShortcutButtonBundle navButtonsList)
        {
            area.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                PlaceholderNeedleAtPos(area, evt.localMousePosition.x);

                evt.StopPropagation();
            });

            area.RegisterCallback<DragPerformEvent>(evt =>
            {
                DragAndDrop.AcceptDrag();

                foreach (var obj in DragAndDrop.objectReferences)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));

                    Texture2D iconTexture = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image as Texture2D;

                    var foundButton = navButtonsList.buttons.FirstOrDefault(b => b.buttonProp.guid == guid);
                    ShortcutButton buttonAtSamePath = foundButton?.buttonProp;

                    var button = new ShortcutButton(obj.name, guid);
                    if (buttonAtSamePath != null) button = new ShortcutButton(obj.name, guid, buttonAtSamePath.isMinimal, buttonAtSamePath.color);

                    if (placeHolderIndex != -1) navButtonsList.InsertAt(placeHolderIndex, button, UI.shortcutButtonAsset);

                    if (buttonAtSamePath != null) navButtonsList.RemoveButton(buttonAtSamePath);

                    UI.placeholderNeedle.RemoveFromHierarchy();
                    placeHolderIndex = -1;
                }

                FillShortcutButtons();

                evt.StopPropagation();
            });

            area.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    UI.placeholderNeedle.RemoveFromHierarchy();

                    placeHolderIndex = -1;
                }
            });
        }

        public static void SetupDragNDropForCreatorArea(VisualElement area, CreatorButtonBundle creatorButtonsList)
        {
            area.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                PlaceholderNeedleAtPos(area, evt.localMousePosition.x);

                evt.StopPropagation();
            });

            area.RegisterCallback<DragPerformEvent>(evt =>
            {
                DragAndDrop.AcceptDrag();
                UI.placeholderNeedle.RemoveFromHierarchy();

                foreach (var obj in DragAndDrop.objectReferences)
                {
                    string iconName = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image.name;

                    var foundButton = creatorButtonsList.buttons.FirstOrDefault(b => b.buttonProp.iconName == iconName);
                    CreatorButton buttonAtSamePath = foundButton?.buttonProp;

                    CreatorButton button = new("", obj);
                    if (buttonAtSamePath != null) button = new CreatorButton("", obj, buttonAtSamePath.isMinimal, buttonAtSamePath.color);

                    if (placeHolderIndex != -1) creatorButtonsList.InsertAt(placeHolderIndex, button, UI.creatorButtonAsset);

                    if (buttonAtSamePath != null) creatorButtonsList.RemoveButton(buttonAtSamePath);

                    ShowBoxAtPos(UI.menuEntryFieldBox, evt.mousePosition.x);
                    UI.menuEntryField.SetValueWithoutNotify("");
                    UI.menuEntryField.Focus();

                    FinalizeMenuEntry(button);
                }

                placeHolderIndex = -1;
                FillCreatorButtons();

                evt.StopPropagation();
            });

            area.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    UI.placeholderNeedle.RemoveFromHierarchy();

                    placeHolderIndex = -1;
                }
            });

        }

    }

    static class UI
    {
        public static VisualElement projectBrowserUI;
        public static VisualElement mainUI;

        public static VisualElement placeholderNeedle;
        public static VisualElement copiedText;


        public static TextField menuEntryField;
        public static VisualElement menuEntryFieldBox;

        public static VisualElement colorPopup;


        public static VisualTreeAsset shortcutButtonAsset;
        public static VisualTreeAsset creatorButtonAsset;


        public static VisualElement shortcutButtonArea;
        public static VisualElement creatorButtonArea;
    }

    static class ColorPopup
    {
        public static bool isForCreator = false;
        public static bool popupIsOpen = false;

        public static ShortcutButton activeNavButton;
        public static CreatorButton activeCreatorButton;
        public static VisualElement activeVisualItem;
    }

}
#endif
