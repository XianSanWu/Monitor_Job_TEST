using FluentValidation;
using static Models.Dto.Requests.WorkflowStepsRequest;

namespace Models.Dto.Requests.Validation
{
    public class WorkflowStepsRequestValidator
    {
        public class WorkflowStepsSearchListRequestValidator : AbstractValidator<WorkflowStepsSearchListRequest>
        {
            public WorkflowStepsSearchListRequestValidator()
            {
                RuleFor(x => x.FieldModel).NotNull().WithMessage("FieldModel 不可為 null");

#pragma warning disable CS8602 // 可能 null 參考的取值 (dereference)。
                RuleFor(x => x.FieldModel.Channel).NotNull().NotEmpty().WithMessage("來源不可空白");
#pragma warning restore CS8602 // 可能 null 參考的取值 (dereference)。

            }

        }

    }
}
