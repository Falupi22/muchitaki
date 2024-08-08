using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Scripts.Common.Utils.Timers
{
    public class Timer : MonoBehaviour
    {
        public int duration;
        public int timeRemaining;
        public bool isWorking = false;
        private Action<Timer> callBack;

        #region Constructors

        public Timer(int milliseconds, Action<Timer> callBack)
        {
            duration = milliseconds; 
            this.callBack = callBack;
        }

        #endregion

        #region Methods

        public void Begin()
        {
            if (!isWorking)
            {
                isWorking = true;
                Invoke("_tick", (float)(this.duration));
            }
        }

        public void End()
        {
            if (isWorking)
            {
                isWorking = false;
            }
        }

        private void _tick()
        {
            if (isWorking)
            {
                callBack?.Invoke(this);
                Invoke("_tick", (float)(this.duration));
            }
        }

        #endregion
    }
}
