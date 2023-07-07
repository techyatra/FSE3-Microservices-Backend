using FluentValidation;
using ToDoList.Model;

namespace ToDoListProducer.Model
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            //RuleFor(model => model.Id).NotNull().NotEmpty().WithMessage("Please specify a ID");
            RuleFor(model => model.Name).NotNull().NotEmpty().WithMessage("Please specify a name");
            RuleFor(model => model.Description).NotNull().NotEmpty().WithMessage("Please specify a Description");
            RuleFor(model => model.StartDate).NotNull().NotEmpty().WithMessage("Please specify a StartDate");
            RuleFor(model => model.EndDate).NotNull().NotEmpty().WithMessage("Please specify a EndDate");
            RuleFor(model => model.Status).NotNull().NotEmpty().WithMessage("Please specify a Status");
            RuleFor(model => model.TotalEffortRequired).NotNull().NotEmpty().WithMessage("Please specify a TotalEffortRequired");
            RuleFor(model => model.StartDate).LessThan(a=>a.EndDate).WithMessage("Task end date should be greater than Task start date");
        }
    }
}
