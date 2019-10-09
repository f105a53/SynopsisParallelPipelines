using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public interface IPipeline<TInput, TOutput>
    {
        Task<TOutput> Process(TInput input);
    }
}