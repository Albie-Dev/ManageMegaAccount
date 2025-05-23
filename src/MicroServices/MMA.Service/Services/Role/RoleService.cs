using MMA.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace MMA.Service
{
    public class RoleService : IRoleService
    {
        private readonly IDbRepository _repository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IDbRepository repository,
            ILogger<RoleService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        #region get all roles
        public async Task<List<RoleDetailResponseDto>> GetAllRolesAsync()
        {
            var roles = await _repository.Queryable<RoleEntity>()
                .Select(s => new RoleDetailResponseDto()
                {
                    RoleId = s.Id,
                    RoleName = s.RoleName,
                    RoleType = s.RoleType
                })
                .ToListAsync();

            return roles;
        }
        #endregion get all roles

        #region sync new role
        public async Task<NotificationResponse> SyncRolesAsync()
        {
            var response = new NotificationResponse()
            {
                Type = CNotificationType.Add,
                DisplayType = CNotificationDisplayType.Page
            };
            var existRoles = await _repository.GetAsync<RoleEntity>();
            var newRoles = new List<RoleEntity>();
            foreach (CRoleType roleType in Enum.GetValues<CRoleType>())
            {
                if (roleType == CRoleType.None)
                {
                    continue;
                }
                if (existRoles.Any(r => r.RoleType == roleType))
                {
                    _logger.LogInformation($"Vai trò {roleType.ToString()} đã có -> bỏ qua sync.");
                    continue;
                }
                newRoles.Add(new RoleEntity()
                {
                    RoleName = roleType.ToString(),
                    RoleType = roleType
                });
            }
            if (newRoles.IsNullOrEmpty())
            {
                _logger.LogWarning($"Không có vai trò mới nào được thêm.");
                response.Message = $"Không có vai trò mới nào được thêm. SyncDate = {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}";
                response.Level = CNotificationLevel.Warning;
                return response;
            }
            try
            {
                int newRoleAdded = await _repository.AddRangeAsync<RoleEntity>(entities: newRoles, clearTracker: true);
                response.Message = $"Sync vai trò mới thành công. Role = {string.Join(",", newRoles.Select(s => s.RoleName).ToList())}, SyncDate = {DateTimeOffset.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}";
                response.Level = CNotificationLevel.Success;
            }
            catch (Exception ex)
            {
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError,
                    errors: new List<ErrorDetailDto>()
                    {
                        new ErrorDetailDto()
                        {
                            Error = $"{ex.Message}",
                            ErrorScope = CErrorScope.PageSumarry,
                            Field = string.Empty
                        }
                    });
            }
            return await Task.FromResult<NotificationResponse>(response);
        }
        #endregion sync new role

        #region get all roles
        public async Task<BasePagedResult<RoleDetailResponseDto>> GetWithPagingAsync(TableParam<RoleFilterProperty> tableParam)
        {
            var modelState = tableParam.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            IQueryable<RoleEntity> collection = _repository.Queryable<RoleEntity>()
                .OrderByDescending(s => s.CreatedDate);
            if (!string.IsNullOrEmpty(tableParam.SearchQuery))
            {
                var searcheQuery = tableParam.SearchQuery.ToLower();
                collection = collection.Where(r => r.RoleName.ToLower().Contains(searcheQuery));
            }
            if (tableParam.Filter != null)
            {
                if (!tableParam.Filter.RoleTypes.IsNullOrEmpty())
                {
                    collection = collection.Where(r => tableParam.Filter.RoleTypes.Contains(r.RoleType));
                }
                if (tableParam.Filter.FromDate.HasValue)
                {
                    collection = collection.Where(r => r.CreatedDate >= tableParam.Filter.FromDate.Value);
                }
                if (tableParam.Filter.ToDate.HasValue)
                {
                    collection = collection.Where(r => r.CreatedDate <= tableParam.Filter.ToDate.Value);
                }
            }
            if (tableParam.Sorter != null)
            {
                var sorter = tableParam.Sorter;
                if (!string.IsNullOrEmpty(sorter.KeyName))
                {
                    if (sorter != null && !string.IsNullOrEmpty(sorter.KeyName))
                    {
                        collection = sorter.KeyName switch
                        {
                            nameof(RoleEntity.RoleName) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.RoleName)
                                : collection.OrderByDescending(pl => pl.RoleName),
                            nameof(RoleEntity.RoleType) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.RoleType)
                                : collection.OrderByDescending(pl => pl.RoleType),
                            nameof(RoleEntity.CreatedDate) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.CreatedDate)
                                : collection.OrderByDescending(pl => pl.CreatedDate),
                            _ => collection
                        };
                    }
                }
            }
            var pagedList = await PagedList<RoleEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);
            var selected = pagedList.Select(role => new RoleDetailResponseDto()
            {
                RoleId = role.Id,
                RoleName = role.RoleName,
                RoleType = role.RoleType,
                CreatedDate = role.CreatedDate,
                ModifiedDate = role.ModifiedDate
            }).ToList();
            var data = new BasePagedResult<RoleDetailResponseDto>()
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
        #endregion get all roles
    }
}