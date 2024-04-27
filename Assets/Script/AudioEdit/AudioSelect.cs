using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSelect : MonoBehaviour
{
    public Dropdown musicDropdown; // Kết nối Dropdown từ Inspector
    private AudioManager audioManager; // Tham chiếu đến AudioManager

    public AudioClip[] musicOptions; // Danh sách các tệp âm thanh

    void Start()
    {
        // Tìm AudioManager trong scene
        audioManager = AudioManager.instance;

        // Xác định các tùy chọn âm nhạc cho Dropdown
        string[] musicNames = new string[musicOptions.Length];
        for (int i = 0; i < musicOptions.Length; i++)
        {
            musicNames[i] = musicOptions[i].name;
        }

        // Đặt tùy chọn cho Dropdown
        musicDropdown.ClearOptions();
        musicDropdown.AddOptions(new List<string>(musicNames));

        // Lắng nghe sự kiện thay đổi lựa chọn của Dropdown
        musicDropdown.onValueChanged.AddListener(OnMusicChanged);
    }

    // Xử lý sự kiện khi lựa chọn trong Dropdown thay đổi
    public void OnMusicChanged(int selectedIndex)
    {
        // Đảm bảo AudioManager tồn tại
        if (audioManager != null && selectedIndex >= 0 && selectedIndex < musicOptions.Length)
        {
            // Chọn và phát nhạc tương ứng từ danh sách âm nhạc
            audioManager.sound.clip = musicOptions[selectedIndex];
            audioManager.sound.Play();
        }
    }
}
