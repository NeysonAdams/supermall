using System;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTimer
{
    private int totalTime;
    private int elapsedTime;
    private bool isRunning = false;
    private string key;

    public Action<string> updete_timer_str;
    public Action<int> updete_int_timer;

    public Action<string> m_updete_timer_str;
    public Action<int> m_updete_int_timer;

    public AsyncTimer(int _totalTime, string _key)
    {
        totalTime = _totalTime;
        key = _key;
        if(PlayerPrefs.HasKey(key))
        {
            long sawed_data = long.Parse(PlayerPrefs.GetString(key));
            long current_data = GetUnixTimestamp();
            if (totalTime > (int)(current_data - sawed_data))
            {
                totalTime = (int)(current_data - sawed_data);
            }
            else
            {
                totalTime = -1;
            }
        }
    }

    public int TotalTime
    {
        set => totalTime = value;
        get => totalTime;
    }

    public void CheckTimer(Action no_sawed_timer_action, Action continue_action)
    {
        if (!PlayerPrefs.HasKey(key))
            no_sawed_timer_action?.Invoke();
        else
        {
            PlayerPrefs.DeleteKey(key);
            continue_action?.Invoke();
        }
    }

    private long GetUnixTimestamp()
    {
        System.DateTime currentDate = System.DateTime.UtcNow;
        long unixTimestamp = (long)(currentDate.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        return unixTimestamp;
    }

    public async void StartTimer(Action after_action)
    {
        if (!isRunning)
        {
            elapsedTime = 0;
            isRunning = true;
            if(totalTime == -1)
            {
                after_action?.Invoke();
                isRunning = false;
                return;
            }

            PlayerPrefs.SetString(key, GetUnixTimestamp().ToString());

            while (elapsedTime < totalTime && isRunning)
            {
                await Task.Delay(1000); // Регулируйте частоту проверки оставшегося времени
                elapsedTime += 1; // Увеличиваем счетчик времени
                updete_timer_str?.Invoke(GetRemainingTime());
                updete_int_timer?.Invoke(elapsedTime);
                m_updete_timer_str?.Invoke(GetRemainingTime());
                m_updete_int_timer?.Invoke(elapsedTime);
            }

            isRunning = false;
            PlayerPrefs.DeleteKey(key);
            after_action?.Invoke();
        }
    }


    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetRemainingTime()
    {
        TimeSpan remainingTimeSpan = TimeSpan.FromSeconds(Mathf.Max(0f, totalTime - elapsedTime));
        return $"{remainingTimeSpan.Hours:D2}:{remainingTimeSpan.Minutes:D2}:{remainingTimeSpan.Seconds:D2}";
    }
}
