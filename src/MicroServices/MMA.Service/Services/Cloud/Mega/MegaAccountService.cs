using CG.Web.MegaApiClient;
using MMA.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static CG.Web.MegaApiClient.MegaApiClient;
using System.Reflection;

namespace MMA.Service
{
    public class MegaAccountService : IMegaAccountService
    {
        private readonly IMegaApiClient _megaClient;
        private readonly IExcelCoreService _excelCoreService;
        private readonly ILogger<MegaAccountService> _logger;
        private readonly IDbRepository _repository;
        public MegaAccountService(
            ILogger<MegaAccountService> logger,
            IDbRepository repository,
            IMegaApiClient megaClient,
            IExcelCoreService excelCoreService
        )
        {
            _logger = logger;
            _repository = repository;
            _megaClient = megaClient;
            _excelCoreService = excelCoreService;
        }
        public async Task<byte[]> DownloadMegaAccountImportTemplateAsync()
        {
            var sheets = new List<SheetExportInfo>
            {
                new SheetExportInfo
                {
                    Data = null,
                    ModelType = typeof(MegaAccountImportDto),
                    SheetKey = "{{MegaAccountSheetKey}}",
                    SheetName = I18NHelper.GetString(key: "MegaAccount_Import_SheetName_Entry"),
                    ColumnTitles = GetColumnTitles(type: typeof(MegaAccountImportDto), i18nColumnFormat: "MegaAccount_Import_Column_Title_{PropertyName}_Entry")
                },
                new SheetExportInfo
                {
                    Data = null,
                    ModelType = typeof(MegaAccountFileImportDto),
                    SheetKey = "{{MegaAccountFileSheetKey}}",
                    SheetName = I18NHelper.GetString(key: "MegaAccount_File_Import_SheetName_Entry"),
                    ColumnTitles = GetColumnTitles(type: typeof(MegaAccountFileImportDto), i18nColumnFormat: "MegaAccount_File_Import_Column_Title_{PropertyName}_Entry"),
                    Translations = EnumHelper.GetTranslations(values: new Dictionary<string, Type>()
                    {
                        { nameof(MegaAccountFileImportDto.NodeType), typeof(CNodeType) },
                        { nameof(MegaAccountFileImportDto.Status), typeof(CFileStatus) },
                    })
                }
            };

            var result = await _excelCoreService.ExportExcelMultipleSheetsAsync(sheetExports: sheets, templateFileName: "Excel.MegaAccountImportTemplate.xlsx");
            return result;
        }

        public async Task<BasePagedResult<MegaAccountDetailDto>> GetMegaAccountWithPagingAsync(TableParam<MegaAccountFilterProperty> tableParam)
        {
            var result = await GetMegaAccountDetailBasePagingAsync(tableParam: tableParam);

            var data = new BasePagedResult<MegaAccountDetailDto>()
            {
                CurrentPage = result.Paged.CurrentPage,
                Items = result.MegaAccounts,
                PageSize = result.Paged.PageSize,
                TotalItems = result.Paged.TotalCount,
                TotalPages = result.Paged.TotalPages,
                Filter = tableParam.Filter,
            };

            return data;
        }

        private async Task<(List<MegaAccountDetailDto> MegaAccounts, PagedList<MegaAccountDetailDto> Paged)> GetMegaAccountDetailBasePagingAsync(TableParam<MegaAccountFilterProperty> tableParam)
        {
            var modelState = tableParam.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            IQueryable<MegaAccountEntity> collection = _repository.Queryable<MegaAccountEntity>()
                .OrderBy(s => s.ExpiredDate);
            IQueryable<UserEntity> createByCollection = _repository.Queryable<UserEntity>();
            IQueryable<UserEntity> modifiedByCollection = _repository.Queryable<UserEntity>();

            var query = collection
            .GroupJoin(createByCollection, ma => ma.CreatedBy, user => user.Id,
                (ma, createdByGroup) => new { ma, createdByGroup })
            .SelectMany(temp => temp.createdByGroup.DefaultIfEmpty(),
                (temp, createdBy) => new { temp.ma, createdBy })
            .GroupJoin(modifiedByCollection, temp => temp.ma.ModifiedBy, user => user.Id,
                (temp, modifiedByGroup) => new { temp.ma, temp.createdBy, modifiedByGroup })
            .SelectMany(temp => temp.modifiedByGroup.DefaultIfEmpty(),
                (temp, modifiedBy) => new MegaAccountDetailDto
                {
                    MegaAccountId = temp.ma.Id,
                    AccountName = temp.ma.AccountName,
                    Password = temp.ma.PasswordHashed,
                    RecoveryKey = temp.ma.RecoveryKeyHashed,
                    LastLogin = temp.ma.LastLogin,
                    ExpiredDate = temp.ma.ExpiredDate,
                    CreatedDate = temp.ma.CreatedDate,
                    ModifiedDate = temp.ma.ModifiedDate,
                    CreatedByProperty = temp.createdBy != null ? new UserBaseInfoDto
                    {
                        Avatar = temp.createdBy.Avatar,
                        Email = temp.createdBy.Email,
                        FullName = temp.createdBy.FullName,
                        UserId = temp.createdBy.Id
                    } : new UserBaseInfoDto
                    {
                        FullName = CoreConstant.SYSTEM_NAME,
                        Email = CoreConstant.SYSTEM_EMAIL,
                        Avatar = CoreConstant.SYSTEM_AVATAR,
                        UserId = CoreConstant.SYSTEM_ACCOUNT_ID
                    },
                    ModifiedByProperty = modifiedBy != null ? new UserBaseInfoDto
                    {
                        Avatar = modifiedBy.Avatar,
                        Email = modifiedBy.Email,
                        FullName = modifiedBy.FullName,
                        UserId = modifiedBy.Id
                    } : new UserBaseInfoDto
                    {
                        FullName = CoreConstant.SYSTEM_NAME,
                        Email = CoreConstant.SYSTEM_EMAIL,
                        Avatar = CoreConstant.SYSTEM_AVATAR,
                        UserId = CoreConstant.SYSTEM_ACCOUNT_ID
                    }
                });
            if (!string.IsNullOrEmpty(tableParam.SearchQuery))
            {
                collection = collection.Where(s => (s.AccountName
                    + s.TotalFileSize + s.ExpiredDate).ToLower().Contains(tableParam.SearchQuery.ToLower()));
            }
            if (tableParam.Filter != null)
            {
                if (!tableParam.Filter.MegaAccountIds.IsNullOrEmpty())
                {
                    query = query.Where(s => tableParam.Filter.MegaAccountIds.Contains(s.MegaAccountId));
                }
                if (tableParam.Filter.CreatedDateRange.Start.HasValue)
                {
                    query = query.Where(ac => ac.CreatedDate >= tableParam.Filter.CreatedDateRange.Start.Value);
                }
                if (tableParam.Filter.CreatedDateRange.End.HasValue)
                {
                    query = query.Where(ac => ac.CreatedDate <= tableParam.Filter.CreatedDateRange.End.Value);
                }
                if (tableParam.Filter.ModifiedDateRange.Start.HasValue)
                {
                    query = query.Where(ac => ac.ModifiedDate >= tableParam.Filter.ModifiedDateRange.Start.Value);
                }
                if (tableParam.Filter.ModifiedDateRange.End.HasValue)
                {
                    query = query.Where(ac => ac.ModifiedDate <= tableParam.Filter.ModifiedDateRange.End.Value);
                }
                if (tableParam.Filter.LastLoginRange.Start.HasValue)
                {
                    query = query.Where(ac => ac.LastLogin >= tableParam.Filter.LastLoginRange.Start.Value);
                }
                if (tableParam.Filter.LastLoginRange.End.HasValue)
                {
                    query = query.Where(ac => ac.LastLogin <= tableParam.Filter.LastLoginRange.End.Value);
                }
                if (tableParam.Filter.ExpiredDateRange.Start.HasValue)
                {
                    query = query.Where(ac => ac.ExpiredDate >= tableParam.Filter.ExpiredDateRange.Start.Value);
                }
                if (tableParam.Filter.ExpiredDateRange.End.HasValue)
                {
                    query = query.Where(ac => ac.ExpiredDate <= tableParam.Filter.ExpiredDateRange.End.Value);
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
                            nameof(MegaAccountEntity.AccountName) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.AccountName)
                                : collection.OrderByDescending(pl => pl.AccountName),
                            nameof(MegaAccountEntity.CreatedDate) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.CreatedDate)
                                : collection.OrderByDescending(pl => pl.CreatedDate),
                            nameof(MegaAccountEntity.ModifiedDate) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.ModifiedDate)
                                : collection.OrderByDescending(pl => pl.ModifiedDate),
                            nameof(MegaAccountEntity.LastLogin) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.LastLogin)
                                : collection.OrderByDescending(pl => pl.LastLogin),
                            nameof(MegaAccountEntity.ExpiredDate) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.ExpiredDate)
                                : collection.OrderByDescending(pl => pl.ExpiredDate),
                            nameof(MegaAccountEntity.TotalFileSize) => sorter.IsASC
                                ? collection.OrderBy(pl => pl.TotalFileSize)
                                : collection.OrderByDescending(pl => pl.TotalFileSize),
                            _ => collection
                        };
                    }
                }
            }
            var pagedList = await PagedList<MegaAccountDetailDto>.ToPagedListAsync(
                source: query, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);

            return (MegaAccounts: pagedList, Paged: pagedList);
        }

        public async Task<byte[]> ImportMegaAccountsAsync(Stream fileStream)
        {
            string megaAccountSheetKey = I18NHelper.GetString(key: "MegaAccount_Import_SheetName_Entry");
            string megaAccountFileSheetKey = I18NHelper.GetString(key: "MegaAccount_File_Import_SheetName_Entry");
            var sheetConfigs = new Dictionary<string, (Type DtoType, Dictionary<string, string> ColumnTitles)>
            {
                {
                    megaAccountSheetKey,
                    (
                        typeof(MegaAccountImportDto),
                        GetColumnTitles(type: typeof(MegaAccountImportDto),
                            i18nColumnFormat: "MegaAccount_Import_Column_Title_{PropertyName}_Entry",
                            isImport: true)
                    )
                },
                {
                    megaAccountFileSheetKey,
                    (
                        typeof(MegaAccountFileImportDto),
                        GetColumnTitles(type: typeof(MegaAccountFileImportDto),
                            i18nColumnFormat: "MegaAccount_File_Import_Column_Title_{PropertyName}_Entry",
                            isImport: true)
                    )
                }
            };

            var translations = EnumHelper.GetTranslations(values: new Dictionary<string, Type>()
            {
                { nameof(MegaAccountFileImportDto.NodeType), typeof(CNodeType) },
                { nameof(MegaAccountFileImportDto.Status), typeof(CFileStatus) },
            });

            List<MegaAccountImportDto> megaAccounts = new List<MegaAccountImportDto>();
            List<MegaAccountFileImportDto> megaAccountFiles = new List<MegaAccountFileImportDto>();

            ImportResult<object> importResult = await _excelCoreService.ImportExcelByTemplateWithMultipleSheetAsync(excelStream: fileStream,
                sheetConfigs: sheetConfigs, translations: translations);
            if (importResult.Result)
            {
                if (importResult.ResultDics.TryGetValue(key: megaAccountSheetKey, value: out List<ImportRow<object>>? megaRowDatas))
                {
                    if (!megaRowDatas.IsNullOrEmpty())
                    {
                        megaAccounts = megaRowDatas.Select(s => s.Data).OfType<MegaAccountImportDto>().ToList();
                    }
                }

                if (importResult.ResultDics.TryGetValue(key: megaAccountFileSheetKey, value: out List<ImportRow<object>>? megaFileRowDatas))
                {
                    if (!megaFileRowDatas.IsNullOrEmpty())
                    {
                        megaAccountFiles = megaFileRowDatas.Select(s => s.Data).OfType<MegaAccountFileImportDto>().ToList();
                    }
                }

                List<MegaAccountEntity> addNewMegaAccounts = new List<MegaAccountEntity>();
                List<MegaAccountEntity> updateMegaAccounts = new List<MegaAccountEntity>();

                foreach (var megaAccount in megaAccounts)
                {
                    var entity = await _repository.FindForUpdateAsync<MegaAccountEntity>(predicate: s => s.AccountName.ToLower() == megaAccount.AccountName.ToLower());
                    bool isAddNew = false;
                    if (entity == null)
                    {
                        isAddNew = true;
                        entity = new MegaAccountEntity();
                    }
                    entity.AccountName = megaAccount.AccountName;
                    entity.PasswordHashed = PasswordHasher.HashPassword(password: megaAccount.Password);
                    entity.RecoveryKeyHashed = PasswordHasher.HashPassword(password: megaAccount.RecoveryKey);
                    entity.ExpiredDate = megaAccount.ExpiredDate ?? entity.ExpiredDate;
                    entity.LastLogin = megaAccount.ExpiredDate.HasValue && megaAccount.ExpiredDate.Value < DateTimeOffset.UtcNow
                        ? megaAccount.ExpiredDate.Value.AddDays(5) : entity.LastLogin;
                    entity.PrivatePassowrd = megaAccount.Password;
                    entity.PrivateRecoveryKey = megaAccount.RecoveryKey;
                    var megaAccountFileProperties = megaAccountFiles.Where(s => s.OwnerAccount.ToLower() == entity.AccountName.ToLower())
                        .Select(s => new FileProperty()
                        {
                            Id = s.Id,
                            NodeType = s.NodeType,
                            Name = s.Name,
                            Size = s.Size,
                            Status = s.Status,
                            Owner = s.Owner,
                            CreationDate = s.CreatedDate,
                            ModificationDate = s.ModifiedDate
                        }).ToList();
                    if (!megaAccountFileProperties.IsNullOrEmpty())
                    {
                        entity.TotalFileSize = megaAccountFileProperties.Sum(s => s.Size);
                        entity.Files = megaAccountFileProperties.ToJson();
                    }
                    if (isAddNew)
                    {
                        addNewMegaAccounts.Add(entity);
                    }
                    else
                    {
                        updateMegaAccounts.Add(entity);
                    }
                }

                if (!addNewMegaAccounts.IsNullOrEmpty())
                {
                    await _repository.AddRangeAsync<MegaAccountEntity>(entities: addNewMegaAccounts, clearTracker: true);
                }
            }
            return importResult.FileResult;
        }

        private Dictionary<string, string> GetColumnTitles(Type type, string i18nColumnFormat, bool isImport = false)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> columnTitles = new Dictionary<string, string>();
            foreach (var prop in props)
            {
                var originalName = prop.Name;
                string format = i18nColumnFormat.Replace(oldValue: "{PropertyName}", newValue: originalName);
                string translateName = I18NHelper.GetString(format);
                if (!isImport)
                {
                    columnTitles[originalName] = translateName;
                }
                else
                {
                    columnTitles[translateName] = originalName;
                }
            }
            return columnTitles;
        }

        public async Task<NotificationResponse> LoginMegaAccountWithIdAsync(MegaAccountLoginRequestDto requestDto)
        {
            NotificationResponse notificationResponse = new NotificationResponse();
            var modelState = requestDto.ModelStateValidate();
            var megaAccount = await _repository.FindForUpdateAsync<MegaAccountEntity>(predicate: s => s.Id == requestDto.MegaAccountId);
            if (megaAccount == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy mega account with ID = {requestDto.MegaAccountId}");
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: modelState.GetErrors());
            }
            var megaLoginResponse = await _megaClient.LoginAsync(email: megaAccount.AccountName, password: megaAccount.PrivatePassowrd,
                mfaKey: megaAccount.PrivateRecoveryKey);
            try
            {
                if (_megaClient.IsLoggedIn)
                {
                    _logger.LogInformation($"Mega login success with AccountName = {megaAccount.AccountName}, SessionId = {megaLoginResponse.SessionId}, MasterKey = {megaLoginResponse.MasterKey}, MegaAccountId = {megaAccount.Id}");
                    megaAccount.LastLogin = DateTimeOffset.UtcNow;
                    megaAccount.ExpiredDate = DateTimeOffset.UtcNow.AddDays(5);
                    await _repository.UpdateAsync<MegaAccountEntity>(entity: megaAccount);
                    await _megaClient.LogoutAsync();
                    _logger.LogInformation($"Mega lout success: LoginStatus = {_megaClient.IsLoggedIn}, MegaAccountId = {megaAccount.Id}, AccountName = {megaAccount.AccountName}");
                    notificationResponse.DisplayType = CNotificationDisplayType.Page;
                    notificationResponse.Level = CNotificationLevel.Info;
                    notificationResponse.Type = CNotificationType.Auth;
                    notificationResponse.Message = $"Mega account {megaAccount.AccountName} login success.";
                }
                return notificationResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MegaLoginAsync: {ex.Message}");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = ex.Message,
                        ErrorScope = CErrorScope.PageSumarry
                    }
                });
            }
        }

        public async Task MegaLoginAsync(LoginRequestDto requestDto)
        {
            _logger.LogInformation($"MegaLoginAsync start request login with Email: {requestDto.Email}");
            try
            {
                // Login to Mega account
                LogonSessionToken megaLoginResponse = await _megaClient.LoginAsync(email: requestDto.Email, password: requestDto.Password);

                if (_megaClient.IsLoggedIn)
                {
                    var megaAccountEntity = await _repository.FindForUpdateAsync<MegaAccountEntity>(s => s.AccountName == requestDto.Email);
                    // Fetch root nodes
                    IEnumerable<INode> nodes = (await _megaClient.GetNodesAsync()).Where(node => node.Type == NodeType.File);
                    List<FileProperty> fileProperties = new List<FileProperty>();
                    // Process nodes recursively
                    await ProcessNodesAsync(nodes, fileProperties, string.Empty);

                    if (megaAccountEntity != null)
                    {
                        megaAccountEntity.LastLogin = DateTimeOffset.UtcNow;
                        megaAccountEntity.ExpiredDate = DateTimeOffset.UtcNow.AddDays(28);
                        megaAccountEntity.TotalFileSize = fileProperties.Sum(file => file.Size);
                        var files = megaAccountEntity.FileProperties;
                        foreach (var file in files)
                        {
                            if (!fileProperties.Select(s => s.Id).Contains(file.Id))
                            {
                                file.Status = CFileStatus.Deleted;
                                fileProperties.Add(file);
                            }
                        }
                        megaAccountEntity.Files = fileProperties.ToJson();
                        await _repository.UpdateAsync<MegaAccountEntity>(entity: megaAccountEntity, clearTracker: true, needSaveChange: true);
                        _logger.LogInformation($"UpdateMegaAccount successfully: Email = {megaAccountEntity.AccountName}");
                    }
                    else
                    {
                        megaAccountEntity = new MegaAccountEntity
                        {
                            AccountName = requestDto.Email,
                            PasswordHashed = PasswordHasher.HashPassword(password: requestDto.Password),
                            RecoveryKeyHashed = PasswordHasher.HashPassword(password: await _megaClient.GetRecoveryKeyAsync()),
                            LastLogin = DateTimeOffset.UtcNow,
                            ExpiredDate = DateTimeOffset.UtcNow.AddDays(28),
                            TotalFileSize = fileProperties.Sum(file => file.Size),
                            Files = fileProperties.ToJson(),
                        };

                        await _repository.AddAsync<MegaAccountEntity>(entity: megaAccountEntity, clearTracker: true, needSaveChange: true);
                        _logger.LogInformation($"AddMegaAccount successfully: Email = {megaAccountEntity.AccountName}");
                    }

                    _logger.LogInformation($"MegaLoginAsync login success with Email = {requestDto.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MegaLoginAsync: {ex.Message}");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = ex.Message,
                        ErrorScope = CErrorScope.PageSumarry
                    }
                });
            }
        }

        public async Task<NotificationResponse> SyncMegaAccountDataAsync(Guid megaAccountId)
        {
            NotificationResponse notificationResponse = new NotificationResponse() { DisplayType = CNotificationDisplayType.Page, Type = CNotificationType.Update };
            try
            {
                var megaAccount = await _repository.FindForUpdateAsync<MegaAccountEntity>(predicate: s => s.Id == megaAccountId);
                if (megaAccount == null)
                {
                    notificationResponse.Level = CNotificationLevel.Warning;
                    notificationResponse.Message = $"Không tìm thấy tài khoản mega với Id = {megaAccountId}";
                    return notificationResponse;
                }
                var megaLoginResponse = await _megaClient.LoginAsync(email: megaAccount.AccountName, password: megaAccount.PrivatePassowrd,
                    mfaKey: megaAccount.PrivateRecoveryKey);
                if (_megaClient.IsLoggedIn)
                {
                    var nodes = await _megaClient.GetNodesAsync();
                    List<FileProperty> files = new List<FileProperty>();
                    await ProcessNodesAsync(nodes: nodes, fileProperties: files, parentId: string.Empty);
                    var fileProperties = megaAccount.FileProperties;
                    megaAccount.LastLogin = DateTimeOffset.UtcNow;
                    megaAccount.ExpiredDate = DateTimeOffset.UtcNow.AddDays(5);
                    megaAccount.TotalFileSize = fileProperties.Sum(file => file.Size);
                    foreach (var file in files)
                    {
                        if (!fileProperties.Select(s => s.Id).Contains(file.Id))
                        {
                            file.Status = CFileStatus.Deleted;
                            fileProperties.Add(file);
                        }
                    }
                    megaAccount.Files = fileProperties.ToJson();
                    await _repository.UpdateAsync<MegaAccountEntity>(entity: megaAccount, clearTracker: true, needSaveChange: true);
                    _logger.LogInformation($"UpdateMegaAccount successfully: Email = {megaAccount.AccountName}");
                    notificationResponse.Level = CNotificationLevel.Success;
                    notificationResponse.Message = $"Đồng bộ dữ liệu từ tài khoản Mega thành công. Ngày = {DateTimeOffset.UtcNow.ToLocalTime()}";
                    await _megaClient.LogoutAsync();
                    return notificationResponse;
                }
                else
                {
                    notificationResponse.Level = CNotificationLevel.Warning;
                    notificationResponse.Message = $"Đăng nhập thất bại đến mega. Email = {megaAccount.AccountName}";
                    return notificationResponse;
                }
            }
            catch(Exception ex)
            {
                notificationResponse.Level = CNotificationLevel.Error;
                notificationResponse.Message = $"Đã có lỗi xảy ra khi đồng bộ dữ liệu từ tài khoản mega với Id = {megaAccountId}. Lỗi : {ex.Message}";
                return notificationResponse;
            }
        }

        public async Task<List<FileProperty>> GetMegaAccountDataAsync(Guid megaAccountId)
        {
            List<ErrorDetailDto> errors = new List<ErrorDetailDto>();
            var megaAccount = await _repository.FindAsync<MegaAccountEntity>(predicate: s => s.Id == megaAccountId);
            if (megaAccount == null)
            {
                errors.Add(new ErrorDetailDto() { Error = $"Không tìm thấy tài khoản Mega có Id = {megaAccountId}", ErrorScope = CErrorScope.PageSumarry });
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: errors);
            }
            return megaAccount.FileProperties;
        }

        private async Task ProcessNodesAsync(IEnumerable<INode> nodes, List<FileProperty> fileProperties, string parentId)
        {
            foreach (var node in nodes)
            {
                switch (node.Type)
                {
                    case NodeType.Directory:
                        // Process child nodes recursively if it's a directory
                        var childNodes = await _megaClient.GetNodesAsync(parent: node);
                        await ProcessNodesAsync(childNodes, fileProperties, node.Id);
                        break;

                    case NodeType.File:
                        // Extract file information and add to file properties list
                        var fileProperty = new FileProperty
                        {
                            Id = node.Id,
                            NodeType = CNodeType.File,
                            Name = node.Name,
                            Size = node.Size,
                            ModificationDate = node.ModificationDate,
                            Fingerprint = node.Fingerprint,
                            ParentId = parentId ?? node.ParentId,
                            CreationDate = node.CreationDate,
                            Owner = node.Owner,
                            Status = CFileStatus.Active,
                            DownloadUrl = (await _megaClient.GetDownloadLinkAsync(node))?.ToString() ?? string.Empty
                        };
                        fileProperties.Add(fileProperty);
                        break;

                    case NodeType.Trash:
                        // Handle trash nodes if necessary
                        break;

                    case NodeType.Root:
                        // Handle root nodes if necessary
                        break;

                    case NodeType.Inbox:
                        // Handle inbox nodes if necessary
                        break;

                    default:
                        break;
                }
            }
        }

    }
}