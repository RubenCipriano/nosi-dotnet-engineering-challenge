using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Services
{
    public interface ICacheService<T>
    {
        Task<T?> Get(Guid id);
        Task Set(Guid id, T item);
        Task Remove(Guid id);
    }
}
