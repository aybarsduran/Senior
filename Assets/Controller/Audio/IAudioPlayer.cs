namespace IdenticalStudios
{
    public interface IAudioPlayer : ICharacterModule
    {
        void PlaySound(Sound sound, float volume = 1f);
        void PlaySound(Sound sound, float speed, float volume = 1f);
        void PlaySounds(Sound[] sounds, float volume = 1f);
        void PlaySounds(Sound[] sounds, float speed, float volume = 1f);
        void ClearAllQueuedSounds();

        /// <summary>
        /// A duration of 0 will play the sound indefinitely until manually stopped.
        /// </summary>
        void LoopSound(Sound sound, float duration);
        void StopLoopingSound(Sound sound, float stopTime = 0.5f);
        void StopAllLoopingSounds();
    }
}