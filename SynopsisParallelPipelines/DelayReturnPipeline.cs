using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public class DelayReturnPipeline<T> : IPipeline<T, T>
    {
        public async Task<T> Process(T input)
        {
            await Task.Delay(1000);
            return input;
        }
    }
}