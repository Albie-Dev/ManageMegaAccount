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
        private readonly IExcelCoreService _excelCoreService;
        private readonly ILogger<ActorService> _logger;

        public ActorService(
            IDbRepository dbRepository,
            IExcelCoreService excelCoreService,
            ILogger<ActorService> logger
        )
        {
            _dbRepository = dbRepository;
            _excelCoreService = excelCoreService;
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
            var result = await GetActorDetailBasePagingAsync(tableParam: tableParam);

            var data = new BasePagedResult<ActorDetailDto>()
            {
                CurrentPage = result.Paged.CurrentPage,
                Items = result.Actors,
                PageSize = result.Paged.PageSize,
                TotalItems = result.Paged.TotalCount,
                TotalPages = result.Paged.TotalPages,
                Filter = tableParam.Filter,
            };

            return data;
        }

        private async Task<(List<ActorDetailDto> Actors, PagedList<ActorEntity> Paged)> GetActorDetailBasePagingAsync(TableParam<ActorFilterProperty> tableParam)
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
                var searchQuery = tableParam.SearchQuery.ToLower();
                collection = collection.Where(ac => ac.Name.ToLower().Contains(searchQuery));
            }

            if (tableParam.Filter != null)
            {
                if (!tableParam.Filter.ActorIds.IsNullOrEmpty())
                {
                    collection = collection.Where(s => tableParam.Filter.ActorIds.Contains(s.Id));
                }
                if (tableParam.Filter.FromBust.HasValue)
                {
                    collection = collection.Where(ac => ac.Bust >= tableParam.Filter.FromBust.Value);
                }
                if (tableParam.Filter.ToBust.HasValue)
                {
                    collection = collection.Where(ac => ac.Bust <= tableParam.Filter.ToBust.Value);
                }
                if (tableParam.Filter.FromWaist.HasValue)
                {
                    collection = collection.Where(ac => ac.Waist >= tableParam.Filter.FromWaist.Value);
                }
                if (tableParam.Filter.ToWaist.HasValue)
                {
                    collection = collection.Where(ac => ac.Waist <= tableParam.Filter.ToWaist.Value);
                }
                if (tableParam.Filter.FromHips.HasValue)
                {
                    collection = collection.Where(ac => ac.Hips >= tableParam.Filter.FromHips.Value);
                }
                if (tableParam.Filter.ToHips.HasValue)
                {
                    collection = collection.Where(ac => ac.Hips <= tableParam.Filter.ToHips.Value);
                }
                if (tableParam.Filter.FromHeight.HasValue)
                {
                    collection = collection.Where(ac => ac.Height >= tableParam.Filter.FromHeight.Value);
                }
                if (tableParam.Filter.ToHeight.HasValue)
                {
                    collection = collection.Where(ac => ac.Height <= tableParam.Filter.ToHeight.Value);
                }

                if (!tableParam.Filter.CupSizeTypes.IsNullOrEmpty())
                {
                    collection = collection.Where(ac => tableParam.Filter.CupSizeTypes.Contains(ac.CupSizeType));
                }

                if (tableParam.Filter.FromDebutDate.HasValue)
                {
                    collection = collection.Where(ac => ac.DebutDate >= tableParam.Filter.FromDebutDate.Value);
                }
                if (tableParam.Filter.ToDebutDate.HasValue)
                {
                    collection = collection.Where(ac => ac.DebutDate <= tableParam.Filter.ToDebutDate.Value);
                }

                if (tableParam.Filter.FromDateOfBirth.HasValue)
                {
                    collection = collection.Where(ac => ac.DateOfBirth >= tableParam.Filter.FromDateOfBirth.Value);
                }
                if (tableParam.Filter.ToDateOfBirth.HasValue)
                {
                    collection = collection.Where(ac => ac.DebutDate <= tableParam.Filter.ToDateOfBirth.Value);
                }

                if (tableParam.Filter.Status.HasValue)
                {
                    collection = collection.Where(ac => ac.Status == tableParam.Filter.Status.Value);
                }

                if (tableParam.Filter.CreatedFromDate.HasValue)
                {
                    collection = collection.Where(ac => ac.CreatedDate >= tableParam.Filter.CreatedFromDate.Value);
                }
                if (tableParam.Filter.CreatedToDate.HasValue)
                {
                    collection = collection.Where(ac => ac.CreatedDate <= tableParam.Filter.CreatedToDate.Value);
                }
            }

            if (tableParam.Sorter != null)
            {
                var sorter = tableParam.Sorter;
                if (!string.IsNullOrEmpty(sorter.KeyName))
                {
                    collection = sorter.KeyName switch
                    {
                        nameof(ActorEntity.Name) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.Name)
                            : collection.OrderByDescending(ac => ac.Name),
                        nameof(ActorEntity.Bust) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.Bust)
                            : collection.OrderByDescending(ac => ac.Bust),
                        nameof(ActorEntity.Waist) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.Waist)
                            : collection.OrderByDescending(ac => ac.Waist),
                        nameof(ActorEntity.Hips) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.Hips)
                            : collection.OrderByDescending(ac => ac.Hips),
                        nameof(ActorEntity.Height) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.Height)
                            : collection.OrderByDescending(ac => ac.Height),
                        nameof(ActorEntity.DebutDate) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.DebutDate)
                            : collection.OrderByDescending(ac => ac.DebutDate),
                        nameof(ActorEntity.CupSizeType) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.CupSizeType)
                            : collection.OrderByDescending(ac => ac.CupSizeType),
                        nameof(ActorEntity.RegionType) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.RegionType)
                            : collection.OrderByDescending(ac => ac.RegionType),
                        nameof(ActorEntity.DateOfBirth) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.DateOfBirth)
                            : collection.OrderByDescending(ac => ac.DateOfBirth),
                        nameof(ActorEntity.CreatedDate) => sorter.IsASC
                            ? collection.OrderBy(ac => ac.CreatedDate)
                            : collection.OrderByDescending(ac => ac.CreatedDate),
                        _ => collection
                    };
                }
            }

            var pagedList = await PagedList<ActorEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);

            var selected = pagedList.Select(ac => {
                var result = ac.Adapt<ActorDetailDto>();
                result.ActorId = ac.Id;
                return result;
            }).ToList();

            return (Actors: selected, Paged: pagedList);
        }

        public async Task<ActorDetailDto> GetActorDetaiAsync(Guid actorId)
        {
            var modelState = actorId.ModelStateValidate(); 
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            var actorEntity = await _dbRepository.FindAsync<ActorEntity>(predicate: s => s.Id == actorId);
            if (actorEntity == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy actor.", errorScope: CErrorScope.PageSumarry);
                throw new MMAException(statusCode: StatusCodes.Status404NotFound,
                    errors: modelState.GetErrors());
            }
            var result = actorEntity.Adapt<ActorDetailDto>();
            result.ActorId = actorEntity.Id;
            return result;
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
            catch (Exception ex)
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
                foreach (var subActorInfo in actorRequestDto.ActorInfos)
                {
                    if (oldSubIds.Contains(subActorInfo.SubActorId))
                    {
                        oldProperty.ForEach(s =>
                        {
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
                actorRequestDto.Adapt(actorEntity);
                actorEntity.ActorInfos = oldProperty;
            }
            actorEntity.ActorInfoProperties = oldProperty.ToJson();
            try
            {
                await _dbRepository.UpdateAsync<ActorEntity>(entity: actorEntity, needSaveChange: true, clearTracker: true);
            }
            catch (Exception ex)
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
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy Actor.", errorScope: CErrorScope.FormSummary);
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
                oldProperty.ForEach(s =>
                {
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

        public async Task<NotificationResponse> DeactiveActorAsync(DeactiveActorRequestDto actorRequestDto)
        {
            var actors = await _dbRepository.GetAsync<ActorEntity>(predicate: at => actorRequestDto.ActorIds.Contains(at.Id)
                && at.Status == CMasterStatus.Active);
            if (!actors.IsNullOrEmpty())
            {
                actors.ForEach(s => s.Status = CMasterStatus.Deactive);
                await _dbRepository.UpdateRangeAsync<ActorEntity>(entities: actors);
            }
            return new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Page,
                Level = CNotificationLevel.Info,
                Type = CNotificationType.Update,
                Message = $"Deactive actors successfully."
            };
        }


        #region import and export
        public async Task<byte[]> ExportActorByExcelTemplateAsyn(TableParam<ActorFilterProperty> tableParam)
        {
            var result = await GetActorDetailBasePagingAsync(tableParam: tableParam);
            var importResult = await _excelCoreService.ExportExcelByTemplateAsync<ActorDetailDto>(exportDataModels: result.Actors,
                fileName: "Excel.ActorExportTemplate.xlsx", assemblyName: "MMA.Service", sheetKey: "{{SheetKey}}", sheetName: "Actor details");
            return importResult;
        }

        public async Task<byte[]> DownloadImportActorExcelTemplateAsync()
        {
            var result = new List<ActorDetailDto>();
            var importResult = await _excelCoreService.ExportExcelByTemplateAsync<ActorDetailDto>(exportDataModels: result,
                fileName: "Excel.ActorImportTemplate.xlsx", assemblyName: "MMA.Service", sheetKey: "{{SheetKey}}", sheetName: "Actors");
            return importResult;;
        }
        #endregion import and export
    }
}