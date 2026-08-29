using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CozyHome.Audio
{
    [RequireComponent(typeof(AudioListener))]
    public class SceneAudioRecorder : MonoBehaviour
    {
        private readonly List<float> recordedSamples = new List<float>();
        private bool isRecording;
        private int sampleRate;
        private int recordedChannels;

        public bool IsRecording { get; private set; }

        private void Awake()
        {
            sampleRate = AudioSettings.outputSampleRate > 0 ? AudioSettings.outputSampleRate : 44100;
            isRecording = false;
        }

        public void StartRecording()
        {
            recordedSamples.Clear();
            isRecording = true;
            IsRecording = true;
        }

        public string StopRecordingAndSave()
        {
            if (!isRecording)
            {
                return string.Empty;
            }

            isRecording = false;
            IsRecording = false;

            string outputPath = MediaExportUtility.BuildExportPath("audio", ".wav");
            WriteWavFile(outputPath);
            Debug.Log("SceneAudioRecorder: Audio exported to " + outputPath);
            return outputPath;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!isRecording || data == null || data.Length == 0)
            {
                return;
            }

            recordedChannels = channels > 0 ? channels : 2;

            lock (recordedSamples)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    recordedSamples.Add(data[i]);
                }
            }
        }

        private void WriteWavFile(string outputPath)
        {
            lock (recordedSamples)
            {
                if (recordedSamples.Count == 0)
                {
                    return;
                }

                using FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                using BinaryWriter writer = new BinaryWriter(stream);

                int channelCount = recordedChannels > 0 ? recordedChannels : 2;
                int bitsPerSample = 16;
                int byteRate = sampleRate * channelCount * (bitsPerSample / 8);
                int blockAlign = channelCount * (bitsPerSample / 8);
                int dataSize = recordedSamples.Count * sizeof(short);
                int riffSize = 36 + dataSize;

                writer.Write(new char[] { 'R', 'I', 'F', 'F' });
                writer.Write(riffSize);
                writer.Write(new char[] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channelCount);
                writer.Write(sampleRate);
                writer.Write(byteRate);
                writer.Write((short)blockAlign);
                writer.Write((short)bitsPerSample);
                writer.Write(new char[] { 'd', 'a', 't', 'a' });
                writer.Write(dataSize);

                foreach (float sample in recordedSamples)
                {
                    float clamped = Mathf.Clamp(sample, -1f, 1f);
                    short pcmSample = (short)(clamped * short.MaxValue);
                    writer.Write(pcmSample);
                }
            }
        }
    }
}
