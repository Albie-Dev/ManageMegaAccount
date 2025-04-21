using MMA.Domain;

namespace MMA.Service
{
    public class UserService : IUserService
    {
        private readonly IDbRepository _repository;
        public UserService(
            IDbRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<BasePagedResult<UserBaseInfoDto>> GetUsersForFilterAsync(TableParam<BaseFilter> tableParam)
        {
            IQueryable<UserEntity> collection = _repository.Queryable<UserEntity>();
            if (!string.IsNullOrEmpty(tableParam.SearchQuery))
            {
                string searchQuery = tableParam.SearchQuery.Trim().ToLower();
                collection = collection.Where(s => s.FullName.ToLower().Contains(searchQuery)
                    || s.FullName.ToLower().Contains(searchQuery)
                    || s.Email.ToLower().Contains(searchQuery)
                );
            }

            var pagedList = await PagedList<UserEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);

            var selected = pagedList.Select(us => new UserBaseInfoDto()
            {
                Avatar = us.Avatar,
                Email = us.Email,
                FullName = us.FullName,
                UserId = us.Id
            }).ToList();

            var data = new BasePagedResult<UserBaseInfoDto>()
            {
                CurrentPage = pagedList.CurrentPage,
                Items = selected,
                PageSize = pagedList.PageSize,
                TotalItems = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                Filter = tableParam.Filter,
            };

            return data;
        }
    }
}