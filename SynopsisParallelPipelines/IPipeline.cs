using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SynopsisParallelPipelines
{
    public interface IPipeline<TInput,TOutput>
    {
        Task<TOutput> Process(TInput input);
    }
    
}
