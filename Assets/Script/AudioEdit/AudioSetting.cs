using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public Slider volumeSlider; // Kết nối Slider từ Inspector
    private AudioManager audioManager; // Tham chiếu đến AudioManager

    void Start()
    {
        // Tìm AudioManager trong scene
        audioManager = AudioManager.instance;

        // Đặt giá trị Slider ban đầu bằng âm lượng hiện tại
        volumeSlider.value = audioManager.sound.volume;

        // Lắng nghe sự kiện thay đổi giá trị của Slider
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    // Xử lý sự kiện khi giá trị của Slider thay đổi
    public void OnVolumeChanged(float volume)
    {
        // Đảm bảo AudioManager tồn tại
        if (audioManager != null)
        {
            // Đặt âm lượng của AudioSource trong AudioManager bằng giá trị của Slider
            audioManager.sound.volume = volume;
        }
    }
}
