﻿using Models.Dto.Responses;
using Models.Entities;
using static Models.Dto.Responses.WorkflowStepsResponse.WorkflowStepsSearchListResponse;

namespace WebApi.Profile
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            CreateMap<WorkflowEntity, WorkflowStepsSearchResponse>();
            CreateMap<AppMhProjectEntity, AppMhProjectResponse>();
            CreateMap<AppMhResultEntity, AppMhResultResponse>();
            CreateMap<BatchIdAppMhResultSuccessCountEntity, BatchIdAppMhResultSuccessCountResponse>();
        }
    }
}
