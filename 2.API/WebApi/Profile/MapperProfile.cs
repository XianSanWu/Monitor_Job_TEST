using Models.Dto.Requests;
using Models.Entities.Requests;
using static Models.Dto.Requests.WorkflowStepsRequest;
using static Models.Dto.Responses.AppMhProjectResponse.AppMhProjectSearchListResponse;
using static Models.Dto.Responses.AppMhResultResponse.AppMhResultSearchListResponse;
using static Models.Dto.Responses.BatchIdAppMhResultSuccessCountResponse.BatchIdAppMhResultSuccessCountSearchListResponse;
using static Models.Dto.Responses.WorkflowStepsResponse;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;
using static Models.Entities.Requests.WorkflowStepsEntityRequest;
using static Models.Entities.Responses.AppMhProjectEntityResponse;
using static Models.Entities.Responses.AppMhResultEntityResponse;
using static Models.Entities.Responses.BatchIdAppMhResultSuccessCountEntityResponse;
using static Models.Entities.Responses.WorkflowEntityResponse;

namespace WebApi.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            #region Service Request → Repository Request
            // Service Request → Repository Request
            // AppMh
            CreateMap<BatchIdAppMhResultSuccessCountRequest, BatchIdAppMhResultSuccessCountEntityRequest>();

            // Mailhunter
            CreateMap<MailhunterLogParseRequest, MailhunterLogParseEntityRequest>();
            
            // WorkflowSteps
            CreateMap<WorkflowStepsSearchListRequest, WorkflowStepsSearchListEntityRequest>();
            CreateMap<WorkflowStepsSearchListFieldModelRequest, WorkflowStepsSearchListFieldModelEntityRequest>();

            CreateMap<WorkflowUpdateListRequest, UpdateWorkflowListEntityRequest>();
            CreateMap<WorkflowStepsUpdateFieldRequest, WorkflowStepsUpdateFieldEntityRequest>();
            CreateMap<WorkflowStepsUpdateConditionRequest, WorkflowStepsUpdateConditionEntityRequest>();

            #endregion

            #region Repository Response → Service Response
            // Service Repository Response → Service Response
            // WorkflowSteps
            // WorkflowStepsEntity
            CreateMap<WorkflowStepsEntitySearchListResponse, WorkflowStepsSearchListResponse>()
                .ForMember(dest => dest.SearchItem, opt => opt.MapFrom(src => src.SearchItem));
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();

            // AppMhResult
            // AppMhResultEntity
            CreateMap<AppMhResultEntity, AppMhResultSearchResponse>();

            // AppMhProjectEntity
            CreateMap<AppMhProjectEntity, AppMhProjectSearchResponse>();

            // BatchIdAppMhResultSuccessCountEntity
            CreateMap<BatchIdAppMhResultSuccessCountEntity, BatchIdAppMhResultSuccessCountSearchResponse>();
            #endregion
        }
    }
}
