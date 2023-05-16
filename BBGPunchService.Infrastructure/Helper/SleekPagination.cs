using BBGPunchService.Core.Model.TargetEntity;

namespace BBGPunchService.Infrastructure.Helper
{
    internal class SleekPagination<T>
    {
        public int PageSize { get; set; }
        public Func<IQueryable<T>, IQueryable<T>> Where { get; set; }
        public IQueryable<T> AsQueryable { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SleekPagination()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public IEnumerable<T> GetPage(int pageIndex)
        {
            var skip = PageSize * pageIndex;
            var result = AsQueryable;
            if (Where != null)
            {
                result = Where(result);
            }
            return result.Skip(skip).Take(PageSize);
        }
    }
}