using System.Collections.Generic;

namespace drv_next_api.QueryServices.Common
{
    public class PagedResult<T>
    {
        public readonly int Take;
        public readonly int Skip;
        public readonly IEnumerable<T> Values;

        public PagedResult(int take, int skip, IEnumerable<T> values)
        {
            Take = take;
            Skip = skip;
            Values = values;
        }
    }
}