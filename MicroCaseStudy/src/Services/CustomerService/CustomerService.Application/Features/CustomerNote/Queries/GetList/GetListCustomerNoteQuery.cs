using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Features.CustomerNote.Queries.GetList;

public class GetListCustomerNoteQuery: IRequest<Response<GetListResponse<GetListCustomerNoteDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    public GetListCustomerNoteQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListCustomerNoteQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListCustomerNoteQueryHandler
        : IRequestHandler<GetListCustomerNoteQuery,
            Response<GetListResponse<GetListCustomerNoteDto>>>
    {
        private readonly ICustomerNoteRepository _customerNoteRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListCustomerNoteQueryHandler(ICustomerNoteRepository customerNoteRepository,
            IMapper mapper, IBaseService baseService)
        {
            _customerNoteRepository = customerNoteRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListCustomerNoteDto>>> Handle(
            GetListCustomerNoteQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<Domain.Entities.CustomerNote> userRoles = await _customerNoteRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                include: m => m.Include(b => b.Customer),   
                size: request.PageRequest.PageSize,
                enableTracking: false
            );
            
            GetListResponse<GetListCustomerNoteDto> mappedUserRoleListModel = _mapper.Map<
                GetListResponse<GetListCustomerNoteDto>
            >(userRoles);
            return _baseService.CreateSuccessResult<GetListResponse<GetListCustomerNoteDto>>(
                mappedUserRoleListModel,
                InternalsConstants.Success
            );
            
        }
    }
    
    
}