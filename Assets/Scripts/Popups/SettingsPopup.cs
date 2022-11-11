using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : Popup
{
    public Sprite activeSoundSprite, activeMusicSprite;
    public Sprite inactiveSoundSprite, inactiveMusicSprite;
    public Button soundButton, musicButton;
    
    public void OnSoundButtonClick()
    {
        SoundManager.Instance.ToggleMute();
        soundButton.image.sprite = (SoundManager.Instance.audioSource.mute) ? inactiveSoundSprite : activeSoundSprite;

    }
    
    public void OnMusicButtonClick()
    {
        SoundManager.Instance.ToggleMuteMusic();
        musicButton.image.sprite = (SoundManager.Instance.musicSource.mute) ? inactiveMusicSprite : activeMusicSprite;

    }
}
