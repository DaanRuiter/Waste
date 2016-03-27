using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private static List<Timer> TIMERS = new List<Timer>();
    public static void UPDATE_TIMERS()
    {
        for (int i = 0; i < TIMERS.Count; i++)
        {
            if (!TIMERS[i].m_paused)
                TIMERS[i].UpdateTimer();
        }
    }

    public delegate void OnTimerCompleteEventHandler();
    public OnTimerCompleteEventHandler OnTimerCompleteEvent;

    private float m_duration;
    private float m_startTime;
    private float m_pausedTime;

    private bool m_done;
    private bool m_paused;

    public Timer(float duration, OnTimerCompleteEventHandler callback = null)
    {
        m_duration = duration;
        m_startTime = Time.time;
        m_done = false;

        if (callback != null)
            OnTimerCompleteEvent += callback;

        TIMERS.Add(this);
    }
    ~Timer()
    {
        TIMERS.Remove(this);
    }

    public void Start()
    {
        m_paused = false;
        m_pausedTime = 0f;
        m_startTime = Time.time;
        m_done = false;

        if (!TIMERS.Contains(this))
            TIMERS.Add(this);
    }
    public void Stop()
    {
        if (TIMERS.Contains(this))
            TIMERS.Remove(this);
    }
    public void TogglePause()
    {
        m_paused = !m_paused;
        if (m_paused)
            m_pausedTime = Time.time;
        else
            m_pausedTime = 0f;
    }
    public void Pause()
    {
        if (!m_paused)
        {
            m_paused = true;
            m_pausedTime = Time.time;
        }
    }
    public void Resume()
    {
        m_paused = false;
        if (m_pausedTime > 0f)
        {
            m_startTime += m_pausedTime - m_startTime;
        }
    }

    /// <summary>
    /// This method is called by the game engine, no need to call it yourself. You can if you want to, cheeky bastard.
    /// </summary>
    public void UpdateTimer()
    {
        if (m_done)
            return;

        if (Time.time >= m_startTime + m_duration)
        {
            m_done = true;
            if (OnTimerCompleteEvent != null)
                OnTimerCompleteEvent();
        }
        else
        {
            m_done = false;
        }
    }

    public bool IsDone
    {
        get
        {
            return Time.time >= m_startTime + m_duration ? true : false;
        }
    }
    public float Duration
    {
        get
        {
            return m_duration;
        }
    }
}