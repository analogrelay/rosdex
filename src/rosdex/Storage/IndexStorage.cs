using System.Threading;
using System.Threading.Tasks;
using Rosdex.Model;

namespace Rosdex.Storage
{
    public abstract class IndexStorage
    {
        public abstract Task<bool> StoreSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken = default);
    }
}
