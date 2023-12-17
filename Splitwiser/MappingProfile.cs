using AutoMapper;
using Splitwiser.Models;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.PaymentInGroup;
using System.Collections.Generic;

namespace Splitwiser
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserViewModel, UserViewModelCheckbox>();
            CreateMap<GroupPaymentHistoryViewModel, GroupPaymentHistoryEntity>();
            CreateMap<PaymentInGroupEntity, PaymentInGroupViewModel>();
        }
    }
}
