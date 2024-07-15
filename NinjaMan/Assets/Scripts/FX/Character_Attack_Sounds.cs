using UnityEngine;

public class Character_Attack_Sounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _attackSounds;
    [SerializeField] private AudioClip[] _hurtSounds;
    [SerializeField] private AudioClip[] _deathSounds;
    [SerializeField] private AudioClip _jumpSound;

    private SoundManager _soundManager;

    private void Awake()
    {
        _soundManager = FindObjectOfType<SoundManager>();
    }

    public void PlayAttackSound()
    {
        if (_soundManager != null)
        {
            _soundManager.PlayAttackSound(_attackSounds);
        }
    }

    public void PlayHurtSound()
    {
        if (_soundManager != null)
        {
            _soundManager.PlayHurtSound(_hurtSounds);
        }
    }

    public void PlayDeathSound()
    {
        if (_soundManager != null)
        {
            _soundManager.PlayDeathSound(_deathSounds);
        }
    }

    public void PlayJumpSound()
    {
        if (_soundManager != null)
        {
            _soundManager.PlayJumpSound(_jumpSound);
        }
    }
}
