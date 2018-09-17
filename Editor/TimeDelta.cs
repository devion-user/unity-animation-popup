using System;
using UnityEngine;

namespace Devi.Graph
{
    internal class TimeDelta
    {
        private long mTicks;
        private bool mNeedFirstDelta;

        public TimeDelta(bool auto)
        {
            if (auto)
            {
                mNeedFirstDelta = false;
                mTicks = DateTime.Now.Ticks;
            }
            else
            {
                mNeedFirstDelta = true;
            }
        }

        public void Reset()
        {
            mNeedFirstDelta = false;
            mTicks = DateTime.Now.Ticks;
        }

        public float UpdateDelta(bool repaintOnly)
        {
            if (!repaintOnly || Event.current.type == EventType.Repaint)
            {
                if (mNeedFirstDelta)
                {
                    mNeedFirstDelta = false;
                    mTicks = DateTime.Now.Ticks;
                    return 0f;
                }

                var nowTicks = DateTime.Now.Ticks;
                var delta = (nowTicks - mTicks) / 1E+07f;
                mTicks = nowTicks;
                return delta;
            }
            return 0f;
        }
    }
}