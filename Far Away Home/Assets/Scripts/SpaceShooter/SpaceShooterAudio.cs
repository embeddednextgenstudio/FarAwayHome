using UnityEngine;

// Lightweight synthesized arcade effects: no external audio files are required.
public static class SpaceShooterAudio
{
    private const int SampleRate = 44100;
    private static AudioClip laserClip;
    private static AudioClip explosionClip;
    private static AudioClip hitClip;

    public static void PlayLaser(AudioSource source)
    {
        if (source == null) return;
        if (laserClip == null) laserClip = CreateLaser();
        source.PlayOneShot(laserClip, 0.38f);
    }

    public static void PlayExplosion(AudioSource source)
    {
        if (source == null) return;
        if (explosionClip == null) explosionClip = CreateExplosion();
        source.PlayOneShot(explosionClip, 0.55f);
    }

    public static void PlayHit(AudioSource source)
    {
        if (source == null) return;
        if (hitClip == null) hitClip = CreateHit();
        source.PlayOneShot(hitClip, 0.6f);
    }

    private static AudioClip CreateLaser()
    {
        const float duration = 0.14f;
        int count = Mathf.CeilToInt(SampleRate * duration);
        float[] samples = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)SampleRate;
            float progress = t / duration;
            float frequency = Mathf.Lerp(1050f, 260f, progress);
            float envelope = Mathf.Pow(1f - progress, 1.4f);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.65f;
        }

        return MakeClip("Synth Laser", samples);
    }

    private static AudioClip CreateExplosion()
    {
        const float duration = 0.42f;
        int count = Mathf.CeilToInt(SampleRate * duration);
        float[] samples = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)SampleRate;
            float progress = t / duration;
            float envelope = Mathf.Pow(1f - progress, 2.1f);
            float noise = Mathf.Sin(i * 127.1f) * Mathf.Sin(i * 311.7f);
            float bass = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(115f, 42f, progress) * t);
            samples[i] = (noise * 0.72f + bass * 0.5f) * envelope * 0.7f;
        }

        return MakeClip("Synth Explosion", samples);
    }

    private static AudioClip CreateHit()
    {
        const float duration = 0.2f;
        int count = Mathf.CeilToInt(SampleRate * duration);
        float[] samples = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)SampleRate;
            float progress = t / duration;
            float frequency = Mathf.Lerp(190f, 75f, progress);
            float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * frequency * t));
            samples[i] = wave * (1f - progress) * 0.32f;
        }

        return MakeClip("Synth Player Hit", samples);
    }

    private static AudioClip MakeClip(string clipName, float[] samples)
    {
        AudioClip clip = AudioClip.Create(clipName, samples.Length, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
