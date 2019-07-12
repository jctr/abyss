using Abyss.Entities;
using Qmmands;
using System.Threading.Tasks;

namespace Abyss.Results
{
    public abstract class ActionResult : CommandResult
    {
        public abstract Task<ResultCompletionData> ExecuteResultAsync(AbyssCommandContext context);

        public abstract Task<ResultCompletionData> UpdateResultAsync(AbyssUpdateContext context);

        public static implicit operator Task<ActionResult>(ActionResult res)
        {
            return Task.FromResult(res);
        }

        public static implicit operator ValueTask<ActionResult>(ActionResult res)
        {
            return new ValueTask<ActionResult>(res);
        }
    }
}