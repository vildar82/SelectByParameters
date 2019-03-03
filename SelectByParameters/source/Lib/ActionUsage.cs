namespace SelectByParameters.Lib
{
    using System;
    using JetBrains.Annotations;

    public class ActionUsage : IDisposable
    {
        private readonly Action after;
        private readonly Action<object> afterObj;
        private readonly object obj;

        public ActionUsage([NotNull] Action before, Action after)
        {
            before();
            this.after = after;
        }

        public ActionUsage([NotNull] object obj, Action preset, Action<object> after)
        {
            this.obj = obj;
            afterObj = after;
        }

        public void Dispose()
        {
            if (afterObj != null)
            {
                afterObj(obj);
            }
            else
            {
                after();
            }
        }
    }
}
