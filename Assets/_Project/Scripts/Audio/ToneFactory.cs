using UnityEngine;

namespace VRTraining.Audio
{
    // makes beeps in code there no audio files
    public static class ToneFactory
    {
        private const int SampleRate = 44100;

        // sequence of equal-length tones eg rising pair = success low buzz = error
        public static AudioClip Create(string name, float[] frequencies, float toneSeconds, bool square = false)
        {
            int samplesPerTone = Mathf.RoundToInt(SampleRate * toneSeconds);
            var data = new float[samplesPerTone * frequencies.Length];

            for (int t = 0; t < frequencies.Length; t++)
            {
                for (int i = 0; i < samplesPerTone; i++)
                {
                    float time = i / (float)SampleRate;
                    float wave = Mathf.Sin(2f * Mathf.PI * frequencies[t] * time);
                    if (square) wave = Mathf.Sign(wave) * 0.6f;

                    // short fade in/out avoids clicks
                    float edge = Mathf.Min(1f, Mathf.Min(i, samplesPerTone - i) / (SampleRate * 0.01f));
                    data[t * samplesPerTone + i] = wave * edge * 0.4f;
                }
            }

            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
