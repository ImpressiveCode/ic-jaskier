namespace Codefusion.Jaskier.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBuildInfoService
    {
        Task<IEnumerable<BuildInfo>> GetBuildsInfo(CommitsRange commitsRange);
    }
}
