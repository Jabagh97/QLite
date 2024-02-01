using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Quavis.QorchLite.Common
{


    public class TimerUtil
    {
        public Task SetTimer(int waitTimeSec, Action<CancellationToken> toAction)
            => SetTimer(waitTimeSec, toAction, CancellationToken.None);

        public async Task SetTimer(int waitTimeSec, Action<CancellationToken> toAction, CancellationToken ct)
        {
            await Task.Delay(waitTimeSec * 1000);
            if(!ct.IsCancellationRequested)
                toAction(ct);           
        }

        public Task<T> Repeat<T>(Func<object, CancellationToken, T> exec, object stateObj, int intervalMSec, int retryCount, CancellationToken ct, bool waitOnStart = false)
        {
            var t = Task.Factory.StartNew<T>((obj) =>
            {
                if (intervalMSec < 10)
                    intervalMSec = 10;
                int n = 0;
                T res = default(T);
                while (true)
                {
                    n++;
                    if ((retryCount > 0 && n > retryCount) || ct.IsCancellationRequested)
                        break;

                    if (waitOnStart)
                        Thread.Sleep(intervalMSec);

                    if (ct.IsCancellationRequested)
                        break;

                    res = exec.Invoke(obj, ct);
                    if (!EqualityComparer<T>.Default.Equals(res, default(T)))
                    {
                        break;
                    }

                    if (!waitOnStart)
                        Thread.Sleep(intervalMSec);
                }
                return res;

            }, stateObj, ct);
            t.ContinueWith(ErrorHandler, TaskContinuationOptions.OnlyOnFaulted);
            return t;
        }

        public Task<bool> Repeat(Func<object, CancellationToken, bool> exec, object stateObj, int intervalMSec, int retryCount, CancellationToken ct, bool waitOnStart = false)
        {
            return Repeat<bool>(exec, stateObj, intervalMSec, retryCount, ct, waitOnStart);
        }

        //public Task<PlatformData> WaitforPlatformData(Func<object, CancellationToken, PlatformData> exec, object requestId, int intervalMSec, int retryCount, CancellationToken ct, bool waitOnStart = false)
        //{
        //    return Repeat<PlatformData>(exec, requestId, intervalMSec, retryCount, ct, waitOnStart);
        //}


        private void ErrorHandler<T>(Task<T> t, object ts)
        {
            if (t.Exception != null)
                LoggerAdapter.Error(t.Exception.InnerException);
        }

    }
}
