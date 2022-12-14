using AutoMapper;
using BLL.Dto;
using BLL.Dto.BankSpecificExpenseModels;
using BLL.Helpers;
using DAL.Entities;
using System;

namespace BLL
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<BankAccountDto, UserBankAccount>();

            CreateMap<MonoApiExpense, ExpenseDto>()
                .ForMember(ed => ed.ExpenseIdentInBank, x => x.MapFrom(mae => mae.Id))
                .ForMember(ed => ed.Amount, x => x.MapFrom(mae => mae.OperationAmount / 100.0))
                .ForMember(ed => ed.Time, x => x.MapFrom(mae => DateTimeOffset.FromUnixTimeSeconds(mae.Time).DateTime))
                .ForMember(ed => ed.Description, x => x.MapFrom(mae => mae.Description))
                .ForMember(ed => ed.Id, x => x.MapFrom(mae => 0));

            CreateMap<PrivatApiExpense, ExpenseDto>()
                .ForMember(ed => ed.ExpenseIdentInBank, x => x.MapFrom(pae => Hasher.MD5(pae.Description + pae.TranDateTime.ToString()))) //Uses description and exact time as unique identifier
                .ForMember(ed => ed.Amount, x => x.MapFrom(pae => pae.Cardamount))
                .ForMember(ed => ed.Time, x => x.MapFrom(pae => pae.TranDateTime))
                .ForMember(ed => ed.Description, x => x.MapFrom(pae => pae.Description))
                .ForMember(ed => ed.Id, x => x.MapFrom(pae => 0));


            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<ExpenseDto, Expense>();

            CreateMap<Expense, ExpenseDto>()
                .ForMember(ed => ed.CategoryId, x => x.MapFrom(e => GetCategoryId(e.Category)))
                .ForMember(ed => ed.CategoryName, x => x.MapFrom(e => GetCategoryName(e.Category)));
        }


        private int? GetCategoryId(Category category)
        {
            return category?.Id;
        }

        private string GetCategoryName(Category category)
        {
            return category?.Name;
        }

    }
}