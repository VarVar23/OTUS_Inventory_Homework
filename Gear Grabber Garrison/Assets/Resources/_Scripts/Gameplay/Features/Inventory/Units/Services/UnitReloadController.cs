using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class UnitReloadController
    {
        public Action<Guid, float> ChangeReloadPercent;
        private bool _isRun; 

        private List<ReloadState> _activeReloads = new();

        public void StartReload(Guid guid, float reloadSeconds)
        {
            if (_activeReloads.Count == 0)
                _isRun = true;

            _activeReloads.Add(new ReloadState(guid, reloadSeconds));
        }

        public void StopReload(Guid guid)
        {
            for(int i = _activeReloads.Count - 1; i >= 0; i--)
            {
                if (_activeReloads[i].Guid == guid) _activeReloads.RemoveAt(i);
            }

            if (_activeReloads.Count == 0)
                _isRun = false;
        }

        public void Tick(float deltaTime)
        {
            if(!_isRun) return;

            for (int i = 0; i < _activeReloads.Count; i++)
            {
                _activeReloads[i].Elapsed += deltaTime;

                float percent = Mathf.Clamp01(1f - _activeReloads[i].Elapsed / _activeReloads[i].ReloadSeconds);
                ChangeReloadPercent?.Invoke(_activeReloads[i].Guid, percent);

                if (_activeReloads[i].Elapsed >= _activeReloads[i].ReloadSeconds)
                {
                    ChangeReloadPercent?.Invoke(_activeReloads[i].Guid, 0f);
                }
            }
        }

        private class ReloadState
        {
            public float ReloadSeconds;
            public float Elapsed;
            public Guid Guid;

            public ReloadState(Guid guid, float reloadSeconds)
            {
                Guid = guid;
                ReloadSeconds = reloadSeconds;
                Elapsed = 0f;
            }
        }
    }
}