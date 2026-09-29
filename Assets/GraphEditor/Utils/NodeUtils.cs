using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphEditor
{
    public enum SlotFeatureType
    {
        Action,
        Basic,
    }

    public static class NodeUtils
    {
        public static AudioClip CurrentAudioClip { get; private set; }
        public static void PlayAudioClip(AudioClip clip)
        {
            CurrentAudioClip = clip;
            if (AnyClipPlaying())
            {
                StopAllAudioClip();
            }
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod("PlayPreviewClip");
            method.Invoke(null, new object[] { CurrentAudioClip, 0, false });
        }

        public static void StopAllAudioClip()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod("StopAllPreviewClips");
            method.Invoke(null, null);
        }

        private static bool AnyClipPlaying()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod("IsPreviewClipPlaying");
            bool playing = (bool)method.Invoke(null, null);

            return playing;
        }

    }
}