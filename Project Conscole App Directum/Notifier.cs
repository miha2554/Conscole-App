using System.Threading;

namespace Test
{
    public class Notifier
    {
        private Thread notifierThread;
        private long timeToSleep;
        private Action onNotify;

        public Notifier(long notificationTime, Action onNotify)
        {
            this.notifierThread = new Thread(this.DoWork);
            this.timeToSleep = notificationTime;
            this.onNotify = onNotify;

            notifierThread.Start();
        }

        private void DoWork()
        {
            while(timeToSleep - Int32.MaxValue > 0) {
                Task.Delay(Int32.MaxValue).Wait();
                timeToSleep = timeToSleep - Int32.MaxValue;
            }

            Task.Delay((int)(timeToSleep)).Wait();

            onNotify();
        }
    }
}
