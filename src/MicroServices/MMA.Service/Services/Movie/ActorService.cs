using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class ActorService : IActorService
    {
        private readonly IDbRepository _dbRepository;
        private readonly ILogger<ActorService> _logger;

        public ActorService(
            IDbRepository dbRepository,
            ILogger<ActorService> logger
        )
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }

        public async Task<BasePagedResult<UserBaseInfoDto>> GetActorBaseInfoASync(TableParam<BaseFilter> tableParam)
        {
            var modelState = tableParam.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            IQueryable<ActorEntity> collection = _dbRepository.Queryable<ActorEntity>()
                .OrderByDescending(s => s.CreatedDate);
            if (!string.IsNullOrEmpty(tableParam.SearchQuery))
            {
                var searcheQuery = tableParam.SearchQuery.ToLower();
                collection = collection.Where(r => r.Name.ToLower().Contains(searcheQuery));
            }
            // if (tableParam.Filter != null)
            // {
            //     if (!tableParam.Filter.RoleTypes.IsNullOrEmpty())
            //     {
            //         collection = collection.Where(r => tableParam.Filter.RoleTypes.Contains(r.RoleType));
            //     }
            //     if (tableParam.Filter.FromDate.HasValue)
            //     {
            //         collection = collection.Where(r => r.CreatedDate >= tableParam.Filter.FromDate.Value);
            //     }
            //     if (tableParam.Filter.ToDate.HasValue)
            //     {
            //         collection = collection.Where(r => r.CreatedDate <= tableParam.Filter.ToDate.Value);
            //     }
            // }
            // if (tableParam.Sorter != null)
            // {
            //     var sorter = tableParam.Sorter;
            //     if (!string.IsNullOrEmpty(sorter.KeyName))
            //     {
            //         if (sorter != null && !string.IsNullOrEmpty(sorter.KeyName))
            //         {
            //             collection = sorter.KeyName switch
            //             {
            //                 nameof(RoleEntity.RoleName) => sorter.IsASC
            //                     ? collection.OrderBy(pl => pl.RoleName)
            //                     : collection.OrderByDescending(pl => pl.RoleName),
            //                 nameof(RoleEntity.RoleType) => sorter.IsASC
            //                     ? collection.OrderBy(pl => pl.RoleType)
            //                     : collection.OrderByDescending(pl => pl.RoleType),
            //                 nameof(RoleEntity.CreatedDate) => sorter.IsASC
            //                     ? collection.OrderBy(pl => pl.CreatedDate)
            //                     : collection.OrderByDescending(pl => pl.CreatedDate),
            //                 _ => collection
            //             };
            //         }
            //     }
            // }
            var pagedList = await PagedList<ActorEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);
            var selected = pagedList.Select(ac => new UserBaseInfoDto()
            {
                Avatar = ac.Avatar,
                Email = string.Empty,
                FullName = ac.Name,
                UserId = ac.Id
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

        public async Task<BasePagedResult<ActorDetailDto>> GetActorWithPagingAsync(TableParam<ActorFilterProperty> tableParam)
        {
            var modelState = tableParam.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            IQueryable<ActorEntity> collection = _dbRepository.Queryable<ActorEntity>();


            var pagedList = await PagedList<ActorEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);
            var selected = pagedList.Select(ac => ac.Adapt<ActorDetailDto>()).ToList();
            var data = new BasePagedResult<ActorDetailDto>()
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

        public async Task<NotificationResponse> AddActorAsync(CreateActorRequestDto actorRequestDto)
        {
            var modelState = actorRequestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            var actorEntity = actorRequestDto.Adapt<ActorEntity>();
            try
            {
                await _dbRepository.AddAsync<ActorEntity>(entity: actorEntity, clearTracker: true, needSaveChange: true);
            }
            catch(Exception ex)
            {
                modelState.AddError(field: string.Empty, errorMessage: ex.Message, errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError,
                    errors: modelState.GetErrors());
            }
            return new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Page,
                Level = CNotificationLevel.Success,
                Message = $"Thêm diễn viên {actorEntity.Name} thành công.",
                Type = CNotificationType.Add
            };
        }

        public async Task<NotificationResponse> UpdateActorAsync(UpdateActorRequestDto actorRequestDto)
        {
            var modelState = actorRequestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            var actorEntity = await _dbRepository.FindForUpdateAsync<ActorEntity>(predicate: s => s.Id == actorRequestDto.ActorId);
            if (actorEntity == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: "Không tìm thấy Actor.", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: modelState.GetErrors());
            }

            var oldProperty = actorEntity.ActorInfos;
            if (!actorRequestDto.SubActorIds.IsNullOrEmpty())
            {
                var oldSubIds = oldProperty.Select(s => s.SubActorId).ToList();
                foreach(var subActorInfo in actorRequestDto.ActorInfos)
                {
                    if (oldSubIds.Contains(subActorInfo.SubActorId))
                    {
                        oldProperty.ForEach(s => {
                            if (s.SubActorId == subActorInfo.SubActorId)
                            {
                                s = subActorInfo;
                            }
                        });
                    }
                    else
                    {
                        oldProperty.Add(subActorInfo);
                    }
                }
            }
            else
            {
               actorEntity = actorRequestDto.Adapt<ActorEntity>();
               actorEntity.ActorInfos = oldProperty;
            }
            actorEntity.ActorInfoProperties = oldProperty.ToJson();
            try
            {
                await _dbRepository.UpdateAsync<ActorEntity>(entity: actorEntity, needSaveChange: true, clearTracker: true);
            }
            catch(Exception ex)
            {
                modelState.AddError(field: string.Empty, errorMessage: ex.Message, errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError, errors: modelState.GetErrors());
            }
            return new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Page,
                Level = CNotificationLevel.Success,
                Message = $"Cập nhật thông tin diễn viên {actorEntity.Name} thành công.",
                Type = CNotificationType.Update
            };
        }

        public async Task<NotificationResponse> DeleteActorAsync(DeleteActorRequestDto actorRequestDto)
        {
            var modelState = actorRequestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            var actorEntity = await _dbRepository.FindAsync<ActorEntity>(predicate: s => s.Id == actorRequestDto.ActorId);
            if (actorEntity == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy Actor.");
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: modelState.GetErrors());
            }
            string responseMessage = string.Empty;
            if (!actorRequestDto.SubActorIds.IsNullOrEmpty())
            {
                var oldProperty = actorEntity.ActorInfos;
                var removeProperty = oldProperty.Where(s => actorRequestDto.SubActorIds.Contains(s.SubActorId)).ToList();
                var removeIds = removeProperty.Select(s => s.SubActorId).ToList();
                List<Guid> deactiveIds = await _dbRepository.Queryable<MovieEntity>().Where(s => s.ExactlyActorId.HasValue && removeIds.Contains(s.ExactlyActorId.Value))
                    .Select(s => s.ExactlyActorId ?? Guid.Empty).ToListAsync();
                removeIds.RemoveAll(s => deactiveIds.Contains(s));
                oldProperty.RemoveAll(s => removeIds.Contains(s.SubActorId));
                _logger.LogInformation(message: $"ActorService DeleteActorAsync: Remove SubActorIds: {string.Join(",", values: removeIds)}");
                oldProperty.ForEach(s => {
                    if (deactiveIds.Contains(s.SubActorId))
                    {
                        s.Status = CMasterStatus.Deactive;
                    }
                });
                _logger.LogInformation(message: $"ActorService DeleteActorAsync: Deactive SubActorIds: {string.Join(",", values: deactiveIds)}");
                actorEntity.ActorInfoProperties = oldProperty.ToJson();

                await _dbRepository.UpdateAsync<ActorEntity>(entity: actorEntity, needSaveChange: true, clearTracker: true);
                responseMessage = $"Xóa thành công SubActors: {string.Join(separator: ",", values: removeIds)}.\n Deactive SubActors: {string.Join(separator: ",", values: deactiveIds)}";
            }
            else
            {
                var movieExist = await _dbRepository.GetAsync<MovieEntity>(s => s.ActorId.HasValue && s.ActorId.Value == actorEntity.Id);
                if (!movieExist.IsNullOrEmpty())
                {
                    actorEntity.Status = CMasterStatus.Deactive;
                    await _dbRepository.UpdateAsync<ActorEntity>(entity: actorEntity, needSaveChange: true, clearTracker: true);
                    _logger.LogInformation(message: $"Deactive actor: {actorEntity.Name}");
                }
                else
                {
                    await _dbRepository.DeleteAsync<ActorEntity>(entity: actorEntity, needSaveChange: true, clearTracker: true);
                    _logger.LogInformation(message: $"Xóa actor: {actorEntity.Name}");
                }
                responseMessage = $"Xóa thành công actor: {actorEntity.Name}";
            }
            return new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Page,
                Level = CNotificationLevel.Success,
                Message = responseMessage,
                Type = CNotificationType.Update
            };
        }
    }
}