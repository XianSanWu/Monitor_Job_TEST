using Models.Entities;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace JobScheduling.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
        }
    }
}
