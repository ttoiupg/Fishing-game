using System;
using System.Collections.Generic;

namespace Halfmoon.Utilities
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; protected set; }
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }
        public void Start()
        {
            Time = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public abstract void Tick(float deltaTime);
    }

    public class Countdowntimer : Timer
    {
        public Countdowntimer(float value) : base(value) { }
        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }
            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }
        public bool IsFinished => Time <= 0;

        public void Reset() => Time = initialTime;
        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }
        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = 0;
        public float GetTime() => Time;
    }

    public class SectionTimer : Timer
    {
        public SectionTimer() : base(0) { }

        public SectionTimer(TimerSection[] sections) : base(0){this.Sections = sections;}
        private TimerSection[] Sections { get; set; }
        public Action<int> OnSectionMeet = delegate { };
        public override void Tick(float deltaTime)
        {
            if (!IsRunning) return;
            Time += deltaTime;
            for (var i = 0; i < Sections.Length; i++)
            {
                if (Time < Sections[i].Time ||Sections[i].Met) continue;
                OnSectionMeet.Invoke(i);
                Sections[i].Met = true;
            }
        }
        
        public void Reset()
        {
            Time = 0;
            foreach (var section in Sections)
            {
                section.Met = false;
            }
        }
        public float GetTime() => Time;
    }

    public class TimerSection
    {
        public bool Met;
        public float Time;

        public TimerSection(float time)
        {
            Time = time;
        }
    }
}
