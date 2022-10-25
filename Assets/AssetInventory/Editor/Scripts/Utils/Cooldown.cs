using System;
using System.Threading.Tasks;

namespace AssetInventory
{
    public class Cooldown
    {
        public bool Enabled = true;

        private DateTime _lastCooldown;
        private readonly float _interval;
        private readonly int _duration;

        public Cooldown(int interval, int duration)
        {
            _lastCooldown = DateTime.Now;
            _interval = interval;
            _duration = duration * 1000;
        }

        public async Task Do()
        {
            if (!Enabled || _interval == 0 || (DateTime.Now - _lastCooldown).TotalMinutes < _interval) return;

            await Task.Delay(_duration);

            _lastCooldown = DateTime.Now;
        }
    }
}