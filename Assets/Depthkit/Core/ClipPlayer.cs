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
using System.Collections;

namespace DepthKit
{

    /// <summary>
    /// The base class that any DepthKit player implementation will derrive from </summary>
    /// <remarks>
    /// This class provides methods that are implemented in child classes to allow
    /// a way for clip to genericly interact with a given player backend. </remarks>
    public abstract class ClipPlayer : MonoBehaviour
    {
        public struct PlayerValues
        {
            public string moviePath;
            public FileLocation location;
            public UnityEngine.Video.VideoClip videoClip;
        }

        public bool VideoLoaded { get; protected set; }
        [SerializeField, HideInInspector]
        private PlayerEvents events = new PlayerEvents();

        public PlayerEvents Events
        {
            get { return events; }
            private set { events = value; }
        }

        public void AssignEvents(PlayerEvents events)
        {
            Events = events;
        }

        /// <summary>
        /// Sets up the player values with the given configuration. </summary>
        public abstract bool SetValues(ClipPlayer.PlayerValues values);

        /// <summary>
        /// returns player created status
        /// </summary>
        public abstract bool IsPlayerCreated();

        /// <summary>
        /// Load the implemented player video.
        /// </summary>
        public abstract IEnumerator Load();

        /// <summary>
        /// Method to dispatch the video loader to start </summary>
        public abstract void StartVideoLoad();

        /// <summary>
        /// Load a video and then play through the implemented player.</summary>
        public abstract IEnumerator LoadAndPlay();

        /// <summary>
        /// Play through the implemented player. Worth using in combination with VideoLoaded to ensure playback will start when called. </summary>
        public abstract void Play();

        /// <summary>
        /// Pause through the implemented player. </summary>
        public abstract void Pause();

        /// <summary>
        /// Stop playback through the player. </summary>
        public abstract void Stop();

        /// <summary>
        /// Tell the implemented player wheter or not it should loop playback. </summary>
        public abstract void SetLoop(bool loopStatus);

        /// <summary>
        /// Set the volume of the video </summary>
        public abstract void SetVolume(float volume);

        /// <summary>
        /// Remove the player components from this GameObject. </summary>
        public abstract void RemoveComponents();

        /// <summary>
        /// Return the texture for DepthKit to use by Renderers </summary>
        public abstract Texture GetTexture();

        /// <summary>
        /// Returns if texture generated is flipped </summary>
        public abstract bool IsTextureFlipped();

        /// <summary>
        /// Returns if texture is generated by a process that bypasses Unity gamma filtering </summary>
        public abstract bool IsExternalTexture();

        /// <summary>
        /// Return the type of player being used </summary>
        public abstract AvailablePlayerType GetPlayerType();

        /// <summary>
        /// Check if video is playing right now or not </summary>
        public abstract bool IsPlaying();

        /// <summary>
        /// Go to a specific location in the provided video. Value should be in seconds. </summary>
        /// Values outside of the range will get mapped to their cooresponding position in the
        /// initial range.
        public abstract void SeekTo(float time);

        /// <summary>
        /// Get the current playback time of the video in seconds. </summary>
        public abstract double GetCurrentTime();

        /// <summary>
        /// Get the current playback frame of the video. </summary>
        public abstract int GetCurrentFrame();

        /// <summary>
        /// Get duration of video in seconds </summary>
        public abstract double GetDuration();
    }
}