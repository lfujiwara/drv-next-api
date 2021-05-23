using System.Collections.Generic;


namespace drv_next_api.Controllers.Models
{
    public class PagedResult<T>
    {
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public IEnumerable<T> Values { get; private set; }

        public PagedResult(int take, int skip, IEnumerable<T> values)
        {
            Take = take;
            Skip = skip;
            Values = values;
        }
    }
}