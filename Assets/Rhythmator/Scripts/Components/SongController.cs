#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SongController
{
    Rhythm rhythm;
    bool songPlaying;
    MarkupsDrawer markupsDrawer;
    WaveformDrawer waveformDrawer;
    TimelineController timelineController;
    Texture backwardIcon, pauseIcon, playIcon;

    public SongController(Rhythm rhythm, MarkupsDrawer markupsDrawer, WaveformDrawer waveformDrawer, TimelineController timelineController)
    {
        this.rhythm = rhythm;
        this.markupsDrawer = markupsDrawer;
        this.waveformDrawer = waveformDrawer;
        this.timelineController = timelineController;
        backwardIcon = Resources.Load("backward") as Texture;
        pauseIcon = Resources.Load("pause") as Texture;
        playIcon = Resources.Load("play") as Texture;
    }

    public void PlayAudio()
    {
        if (!songPlaying) {
            AudioUtility.PlayClip(rhythm.clip);
            AudioUtility.SetClipSamplePosition(rhythm.clip, waveformDrawer.SecondsToSample(markupsDrawer.GetNeedle()));
            timelineController.NotifySongStart();
        }
        songPlaying = true;
    }

    public void PauseAudio()
    {
        if (songPlaying) {
            AudioUtility.StopAllClips();
            songPlaying = false;
        }
    }

    public void DrawGUI()
    {
        try {
            if (songPlaying) {
                markupsDrawer.SetNeedle(AudioUtility.GetClipPosition(rhythm.clip));
                if (waveformDrawer.SecondsToPixels(markupsDrawer.GetNeedle()) - waveformDrawer.GetScroll() > waveformDrawer.GetRect().width) {
                    waveformDrawer.SetScroll(waveformDrawer.SecondsToPixels(markupsDrawer.GetNeedle()));
                }
                if (waveformDrawer.SecondsToPixels(markupsDrawer.GetNeedle()) < waveformDrawer.GetScroll()) {
                    waveformDrawer.SetScroll(waveformDrawer.SecondsToPixels(markupsDrawer.GetNeedle()));
                }
            }

            //Music control
            if (GUILayout.Button(backwardIcon, GUILayout.Width(40), GUILayout.Height(40))) {
                markupsDrawer.SetNeedle(0);
                AudioUtility.SetClipSamplePosition(rhythm.clip, waveformDrawer.SecondsToSample(markupsDrawer.GetNeedle()));
            }

            if (GUILayout.Button(pauseIcon, GUILayout.Width(40), GUILayout.Height(40))) {
                if (songPlaying) {
                    AudioUtility.StopAllClips();
                    songPlaying = false;
                }
            }

            if (GUILayout.Button(playIcon, GUILayout.Width(40), GUILayout.Height(40))) {
                Debug.Log("Press play, song playing? " + songPlaying);
                if (!songPlaying) {
                    AudioUtility.PlayClip(rhythm.clip);
                    AudioUtility.SetClipSamplePosition(rhythm.clip, waveformDrawer.SecondsToSample(markupsDrawer.GetNeedle()));
                    timelineController.NotifySongStart();
                }
                songPlaying = true;
            }

            //if (GUILayout.Button("test")) {
            //    AudioUtility.PrintAllMethods();
            //}
        }
        catch(Exception e) {
            Debug.LogError(e);
        }
    }

    public void UpdateReference(Rhythm rhythm)
    {
        this.rhythm = rhythm;
    }

    public bool IsPlaying()
    {
        return songPlaying;
    }

}
#endif