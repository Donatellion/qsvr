/************************************************************************************

DepthKit Unity SDK License v1
Copyright 2016-2018 Simile Inc. All Rights reserved.  

Licensed under the Simile Inc. Software Development Kit License Agreement (the "License"); 
you may not use this SDK except in compliance with the License, 
which is provided at the time of installation or download, 
or which otherwise accompanies this software in either electronic or hard copy form.  

You may obtain a copy of the License at http://www.depthkit.tv/license-agreement-v1

Unless required by applicable law or agreed to in writing, 
the SDK distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 

************************************************************************************/

using UnityEngine;
using UnityEditor;

namespace DepthKit
{

    [CustomEditor(typeof(Clip))]
    [CanEditMultipleObjects]
    public class ClipEditor : Editor
    {

        //PLAYER PROPERTIES
        SerializedProperty _playerTypeProp;
        SerializedProperty _videoClipProp;
        SerializedProperty _moviePathProp;
        SerializedProperty _fileLocationProp;
        SerializedProperty _posterProp;
        SerializedProperty _dynamicMetadataProp;
        SerializedProperty _metaDataFileProp;
        SerializedProperty _metaDataFilePathProp;
        SerializedProperty _autoPlayProp;
        SerializedProperty _autoLoadProp;
        SerializedProperty _delaySecondsProp;
        SerializedProperty _videoLoopsProp;
        SerializedProperty _volumeProp;

        //RENDERER PROPERTIES
        SerializedProperty _renderTypeProp;
        // SerializedProperty _depthPackingEpsilonProp;
        SerializedProperty _depthSaturationThresholdProp;
        SerializedProperty _depthBrightnessThresholdProp;
        SerializedProperty _internalEdgeThresholdProp;

        //CLIP REFRESH PROPERTIES
        SerializedProperty _resetPlayerTypeProp;
        //SerializedProperty _resetRenderTypeProp;
        SerializedProperty _refreshPlayerValuesProp;
        SerializedProperty _refreshMetadataProp;
        Texture2D logo;

        int cachedPlayerType;
        int cachedRenderType;
        bool _needToUndoRedo;

        void OnEnable()
        {
            // subscribe to the undo event
            Undo.undoRedoPerformed += OnUndoRedo;
            _needToUndoRedo = false;

            //set the property types
            _playerTypeProp = serializedObject.FindProperty("_playerType");
            _videoClipProp = serializedObject.FindProperty("_videoClip");
            _moviePathProp = serializedObject.FindProperty("_moviePath");
            _fileLocationProp = serializedObject.FindProperty("_fileLocation");
            _posterProp = serializedObject.FindProperty("_poster");
            _dynamicMetadataProp = serializedObject.FindProperty("_dynamicMetadataFile");
            _metaDataFileProp = serializedObject.FindProperty("_metaDataFile");
            _metaDataFilePathProp = serializedObject.FindProperty("_metaDataFilePath");
            _autoPlayProp = serializedObject.FindProperty("_autoPlay");
            _autoLoadProp = serializedObject.FindProperty("_autoLoad");
            _delaySecondsProp = serializedObject.FindProperty("_delaySeconds");
            _videoLoopsProp = serializedObject.FindProperty("_videoLoops");
            _volumeProp = serializedObject.FindProperty("_volume");

            _renderTypeProp = serializedObject.FindProperty("_renderType");
            // _depthPackingEpsilonProp = serializedObject.FindProperty("_depthPackingEpsilon");
            _depthSaturationThresholdProp = serializedObject.FindProperty("_depthSaturationFilter");
            _depthBrightnessThresholdProp = serializedObject.FindProperty("_depthLuminanceFilter");
            _internalEdgeThresholdProp = serializedObject.FindProperty("_triangleSizeLimit");

            cachedPlayerType = _playerTypeProp.enumValueIndex;
            cachedRenderType = _renderTypeProp.enumValueIndex;

            _resetPlayerTypeProp = serializedObject.FindProperty("_needToResetPlayerType");
            //_resetRenderTypeProp = serializedObject.FindProperty("_needToResetRenderType");
            _refreshPlayerValuesProp = serializedObject.FindProperty("_needToRefreshPlayerValues");
            _refreshMetadataProp = serializedObject.FindProperty("_needToRefreshMetadata");

            logo = Resources.Load("dk-logo-32", typeof(Texture2D)) as Texture2D;
        }

        void OnUndoRedo()
        {
            _needToUndoRedo = true;
        }

        public override void OnInspectorGUI()
        {
            //update the object with the object variables
            serializedObject.Update();

            //set the clip var as the target of this inspector
            Clip clip = (Clip)target;

            // DK INFO
            OnInspectorGUI_DepthKitInfo();

            EditorGUILayout.BeginVertical("Box");
            {
                // PLAYER INFO
                OnInspectorGUI_PlayerOptions(clip);
                // META INFO
                OnInspectorGUI_PlayerMetaInfo();
                EditorGUILayout.Space();
                // PLAYER SETUP FEEDBACK
                OnInspectorGUI_PlayerSetupInfo(clip);

                // if(GUILayout.Button("Hard Refresh")) {setAllPropsToRefresh();}
            }
            EditorGUILayout.EndVertical();

            // PLAYBACK OPTIONS
            OnInspectorGUI_PlaybackOptions();

            // RENDERER OPTIONS
            OnInspectorGUI_RendererOptions();

            EditorGUILayout.Space();
            OnInspectorGUI_CheckForUndo();

            // APPLY PROPERTY MODIFICATIONS
            serializedObject.ApplyModifiedProperties();
        }

        void OnInspectorGUI_DepthKitInfo()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            Rect rect = GUILayoutUtility.GetRect(logo.width, logo.height); GUI.DrawTexture(rect, logo);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Depthkit Clip Editor", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Version " + DepthKitPlugin.Version);
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void OnInspectorGUI_PlayerSetupInfo(Clip clip)
        {
            if (clip.IsSetup)
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginVertical();
                // GUI.color = Color.black;
                EditorGUILayout.HelpBox("DepthKit clip is setup and ready for playback",
                                        MessageType.Info);
            }

            else
            {
                GUI.backgroundColor = Color.red;
                EditorGUILayout.BeginVertical();
                // GUI.color = Color.black;
                EditorGUILayout.HelpBox("DepthKit clip is not setup. \n"
                                        + string.Format("Clip Setup: {0} | Metadata Setup: + {1} | Player Setup: {2}",
                                        clip.IsSetup, clip._metaSetup, clip._playerSetup),
                                        MessageType.Error);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }

        void OnInspectorGUI_PlaybackOptions()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Playback Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_autoLoadProp);
            EditorGUILayout.PropertyField(_autoPlayProp);
            if (_autoPlayProp.boolValue)
            {
                _autoLoadProp.boolValue = true;
                EditorGUILayout.PropertyField(_delaySecondsProp);
            }
            EditorGUILayout.Slider(_volumeProp, 0.0f, 1.0f);
            EditorGUILayout.PropertyField(_videoLoopsProp);
            EditorGUILayout.EndVertical();
        }

        void OnInspectorGUI_RendererOptions()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Renderer Options", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_renderTypeProp);
            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += OnInspectorGui_UserConfirmRendererSwitch;
                //OnInspectorGui_UserConfirmRendererSwitch(); 
            }

            // EditorGUILayout.PropertyField(_depthPackingEpsilonProp);
            EditorGUILayout.PropertyField(_depthBrightnessThresholdProp);
            EditorGUILayout.PropertyField(_depthSaturationThresholdProp);
            EditorGUILayout.PropertyField(_internalEdgeThresholdProp);
            EditorGUILayout.EndVertical();
        }

        // https://answers.unity.com/questions/413101/invalidoperationexception-operation-is-not-valid-d.html
        void OnInspectorGui_UserConfirmRendererSwitch()
        {
            //Debug.Log("Change check- current " + _renderTypeProp.enumValueIndex + " cached: " + cachedRenderType);
            if(_renderTypeProp.enumValueIndex != cachedRenderType)
            {
                Clip clip = (Clip)target;

                if (_renderTypeProp.enumValueIndex != (int)RenderType.Hologram && 
                    cachedRenderType               == (int)RenderType.Hologram)
                {
                    if (EditorUtility.DisplayDialog("Changing Renderer type", "WARNING: you will lose all render layers if you change renderer type, would you like to change renderer?", "Yes", "No"))
                    {
                        cachedRenderType = _renderTypeProp.enumValueIndex;
                        clip._needToResetRenderType = true;
                    }
                    else
                    {
                        //_renderTypeProp.enumValueIndex = cachedRenderType; // reset back to render type before selection.
                        clip._renderType = (RenderType)cachedRenderType;
                    }            
                }
                else
                {
//                    Debug.Log("Render type changed");
                    cachedRenderType = _renderTypeProp.enumValueIndex;
                    clip._needToResetRenderType = true;
                   // _resetRenderTypeProp.boolValue = true;
                }
            }
        }

        void OnInspectorGUI_PlayerOptions(Clip clip)
        {
            EditorGUILayout.LabelField("Player Options", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_playerTypeProp);
            if (EditorGUI.EndChangeCheck() || (_playerTypeProp.enumValueIndex != cachedPlayerType))
            {
                _resetPlayerTypeProp.boolValue = true;
                cachedPlayerType = _playerTypeProp.enumValueIndex;
            }

            //VIDEO PLAYER
            if (clip._playerType == (AvailablePlayerType)PlayerType.UnityVideoPlayer)
            {
                EditorGUI.BeginChangeCheck();
                try { EditorGUILayout.PropertyField(_videoClipProp); }
                catch (System.NullReferenceException)
                {
                    if (_playerTypeProp.enumValueIndex < 0)
                    {
                        Debug.LogError("Invalid player for current build target. Use the 'Mobile' prefab when using a mobile build target and 'Standalone' prefab when using a standalone build target.");
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    _refreshPlayerValuesProp.boolValue = true;
                }
            }

            if (clip._playerType == (AvailablePlayerType)PlayerType.AVProVideo ||
                clip._playerType == (AvailablePlayerType)PlayerType.MPMP)
            {

                //MPMP
                if (clip._playerType == (AvailablePlayerType)PlayerType.MPMP)
                {
                    string helpString = ""
                    + "For MPMP, your Movie Path should just be the name of your video with its extension, "
                    + "something like 'MyClip.mp4'. \n\n"
                    + "MPMP also requires your video be in the Assets/StreamingAssets folder,"
                    + "otherwise it will not be read and playback will not happen. If you do not already have a StreamingAssets folder "
                    + "in your project, just make a new folder called 'StreamingAssets' in Assets.";
                    EditorGUILayout.HelpBox(helpString, MessageType.Info);
                }

                //AVPROVIDEO
                if (clip._playerType == (AvailablePlayerType)PlayerType.AVProVideo)
                {

                    EditorGUI.BeginChangeCheck();

                    string helpString = ""
                    + "For AVProVideo, your Movie Path is the name of your video with its extension (MyClip.mp4 for example), relative to what "
                    + "you selected for 'File Location'. We recommend simply placing your video in the Assets/StreamingAssets folder "
                    + "(or creating it if it doesn't already exist) and setting File Location to 'Relative to Streaming Assets folder' "
                    + "as this is the easiest way to quickly get going. \n\n"
                    + "For more advanced functionality, check out the AVProVideo documentation.";
                    EditorGUILayout.HelpBox(helpString, MessageType.Info);

                    EditorGUILayout.PropertyField(_fileLocationProp);

                    if (EditorGUI.EndChangeCheck())
                    {
                        _refreshPlayerValuesProp.boolValue = true;
                    }

                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.DelayedTextField(_moviePathProp);
                if (EditorGUI.EndChangeCheck())
                {
                    _refreshPlayerValuesProp.boolValue = true;
                }
            }
        }

        void OnInspectorGUI_PlayerMetaInfo()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_dynamicMetadataProp);
            if (_dynamicMetadataProp.boolValue)
            {
                EditorGUILayout.DelayedTextField(_metaDataFilePathProp);
            }
            else
            {
                EditorGUILayout.PropertyField(_metaDataFileProp);
            }
            EditorGUILayout.PropertyField(_posterProp);
            if (EditorGUI.EndChangeCheck())
            {
                _refreshMetadataProp.boolValue = true;
            }
        }

        void OnInspectorGUI_CheckForUndo()
        {
            if (_needToUndoRedo)
            {
                _refreshPlayerValuesProp.boolValue = true;
                _needToUndoRedo = false;
            }
        }
    }
}