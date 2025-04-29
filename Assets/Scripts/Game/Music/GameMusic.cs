using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusic : MonoBehaviour
{
    [SerializeField] List<AudioClip> bgMusics = new List<AudioClip>();
    [SerializeField] AudioSource audioSource;

    public void UpdateMusic()
    {
        if (SceneManager.GetActiveScene().buildIndex == 9)
        {
            if (audioSource.clip != bgMusics[1])
            {
                audioSource.clip = bgMusics[1];
                audioSource.Play();
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 10)
        {
            if (audioSource.clip != bgMusics[2])
            {
                audioSource.clip = bgMusics[2];
                audioSource.Play();
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 11)
        {
            if (audioSource.clip != bgMusics[3])
            {
                audioSource.clip = bgMusics[3];
                audioSource.Play();
            }
        }
        else if (audioSource.clip != bgMusics[0])
        { 
            audioSource.clip = bgMusics[0]; 
            audioSource.Play();
        }
    }

    public void SetClip(int index)
    {
        audioSource.clip = bgMusics[index];
        audioSource.Play();
    }

}
