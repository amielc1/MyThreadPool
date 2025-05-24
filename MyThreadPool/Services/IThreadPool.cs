namespace MyThreadPool.Services
{
    public interface IThreadPool : IDisposable
    {
        void Execute(ThreadStart task);
    }
}
