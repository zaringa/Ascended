using UnityEngine;

public class CooldownSystem
{
    private float _currentCooldown;
    private float _baseDuration;
    private float _speedMultiplier = 1f;
    private bool _isOnCooldown;

    public float BaseDuration => _baseDuration;
    public float CurrentCooldown => _currentCooldown;
    public bool IsOnCooldown => _isOnCooldown;
    public float Percentage => _isOnCooldown ? (1f - _currentCooldown / _baseDuration) : 1f;

    public CooldownSystem(float baseDuration)
    {
        _baseDuration = baseDuration;
        _currentCooldown = 0f;
        _isOnCooldown = false;
    }

    public void ActivateCooldown()
    {
        if (!_isOnCooldown)
        {
            _currentCooldown = _baseDuration / _speedMultiplier;
            _isOnCooldown = true;
        }
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (_isOnCooldown)
        {
            _currentCooldown -= deltaTime * _speedMultiplier;
            if (_currentCooldown <= 0f)
            {
                _currentCooldown = 0f;
                _isOnCooldown = false;
            }
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    public float GetRemainingTime() => _isOnCooldown ? _currentCooldown : 0f;
}