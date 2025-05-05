using CG.Web.MegaApiClient;
using MMA.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static CG.Web.MegaApiClient.MegaApiClient;
using System.Dynamic;
using System.Reflection;

namespace MMA.Service
{
    public class MegaService : IMegaService
    {
        private readonly IMegaApiClient _megaClient;
        private readonly IExcelCoreService _excelCoreService;
        private readonly ILogger<MegaService> _logger;
        private readonly IDbRepository _repository;
        public MegaService(
            ILogger<MegaService> logger,
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

        public async Task<BasePagedResult<MegaAccountDetailDto>> GetMegaAccountWithPagingAsync(TableParam<MegaAccountFilterProperty> tableParam)
        {
            var modelState = tableParam.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            IQueryable<MegaAccountEntity> collection = _repository.Queryable<MegaAccountEntity>()
                .OrderBy(s => s.ExpiredDate);
            if (!string.IsNullOrEmpty(tableParam.SearchQuery))
            {
                collection = collection.Where(s => (s.AccountName
                    + s.TotalFileSize + s.ExpiredDate).ToLower().Contains(tableParam.SearchQuery.ToLower()));
            }
            if (tableParam.Filter != null)
            {
                if (tableParam.Filter.FromCreatedDate.HasValue)
                {
                    collection = collection.Where(s => s.CreatedDate >= tableParam.Filter.FromCreatedDate.Value);
                }
                if (tableParam.Filter.ToCreatedDate.HasValue)
                {
                    collection = collection.Where(s => s.CreatedDate <= tableParam.Filter.ToCreatedDate.Value);
                }
                if (tableParam.Filter.FromModifiedDate.HasValue)
                {
                    collection = collection.Where(s => s.ModifiedDate >= tableParam.Filter.FromModifiedDate.Value);
                }
                if (tableParam.Filter.ToModifiedDate.HasValue)
                {
                    collection = collection.Where(s => s.ModifiedDate <= tableParam.Filter.ToModifiedDate.Value);
                }
                if (tableParam.Filter.FromExpiredDate.HasValue)
                {
                    collection = collection.Where(s => s.ExpiredDate >= tableParam.Filter.FromExpiredDate.Value);
                }
                if (tableParam.Filter.ToExpiredDate.HasValue)
                {
                    collection = collection.Where(s => s.ExpiredDate <= tableParam.Filter.ToExpiredDate.Value);
                }
                if (tableParam.Filter.FromLastLogin.HasValue)
                {
                    collection = collection.Where(s => s.LastLogin >= tableParam.Filter.FromLastLogin.Value);
                }
                if (tableParam.Filter.ToLastLogin.HasValue)
                {
                    collection = collection.Where(s => s.LastLogin <= tableParam.Filter.ToLastLogin.Value);
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
            var pagedList = await PagedList<MegaAccountEntity>.ToPagedListAsync(
                source: collection, pageNumber: tableParam.PageNumber,
                pageSize: tableParam.PageSize);
            var selectedTasks = pagedList.Select(async megaAccount =>
            {
                var createdByInfo = new UserBaseInfoDto();
                var modifiedByInfo = new UserBaseInfoDto();
                var createdBy = await _repository.FindAsync<UserEntity>(s => s.Id == megaAccount.CreatedBy);

                if (megaAccount.CreatedBy != megaAccount.ModifiedBy)
                {
                    var modifiedBy = await _repository.FindAsync<UserEntity>(s => s.Id == megaAccount.ModifiedBy);
                    modifiedByInfo.FullName = modifiedBy?.FullName ?? "System";
                    modifiedByInfo.Avatar = modifiedBy?.Avatar ?? string.Empty;
                }

                createdByInfo.FullName = createdBy?.FullName ?? "System";
                createdByInfo.Avatar = createdBy?.Avatar ?? string.Empty;

                return new MegaAccountDetailDto()
                {
                    MegaAccountId = megaAccount.Id,
                    AccountName = megaAccount.AccountName,
                    PasswordHashed = megaAccount.PasswordHashed,
                    RecoveryKey = megaAccount.RecoveryKeyHashed,
                    LastLogin = megaAccount.LastLogin,
                    ExpiredDate = megaAccount.ExpiredDate,
                    CreatedDate = megaAccount.CreatedDate,
                    ModifiedDate = megaAccount.ModifiedDate,
                    CreatedUserInfo = createdByInfo,
                    ModifiedUserInfo = modifiedByInfo
                };
            }).ToList();

            var selected = await Task.WhenAll(selectedTasks);

            var data = new BasePagedResult<MegaAccountDetailDto>()
            {
                CurrentPage = pagedList.CurrentPage,
                Items = selected.ToList(),
                PageSize = pagedList.PageSize,
                TotalItems = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                Filter = tableParam.Filter,
            };

            return data;
        }

        public async Task<(Dictionary<string, ImportResult<object>>, byte[])> ImportMegaAccountsAsync(Stream fileStream)
        {
            var megaAccountProps = typeof(MegaAccountImportDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> megaAccountColumnTitles = new Dictionary<string, string>();
            var megaAccountFileProps = typeof(MegaAccountFileImportDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> megaAccountFileColumnTitles = new Dictionary<string, string>();

            foreach (var prop in megaAccountProps)
            {
                var originalName = prop.Name;
                string translateName = I18NHelper.GetString($"MegaAccount_Import_Column_Title_{originalName}_Entry");
                megaAccountColumnTitles[translateName] = originalName;
            }

            foreach (var prop in megaAccountFileProps)
            {
                var originalName = prop.Name;
                string translateName = I18NHelper.GetString($"MegaAccount_File_Import_Column_Title_{originalName}_Entry");
                megaAccountFileColumnTitles[translateName] = originalName;
            }
            
            var sheetConfigs = new Dictionary<string, (Type DtoType, Dictionary<string, string> ColumnTitles)>
            {
                {
                    I18NHelper.GetString(key: "MegaAccount_Import_SheetName_Entry"),
                    (
                        typeof(MegaAccountImportDto),
                        megaAccountColumnTitles
                    )
                },
                {
                    I18NHelper.GetString(key: "MegaAccount_File_Import_SheetName_Entry"),
                    (
                        typeof(MegaAccountFileImportDto),
                        megaAccountFileColumnTitles
                    )
                }
            };

            var enumMaps = EnumHelper.GetTranslations(values: new Dictionary<string, Type>()
            {
                { nameof(MegaAccountFileImportDto.NodeType), typeof(CNodeType) },
                { nameof(MegaAccountFileImportDto.Status), typeof(CFileStatus) },
            });

            var result = await _excelCoreService.ImportExcelByTemplateAsync(fileStream, sheetConfigs, enumMaps);
            return result;
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