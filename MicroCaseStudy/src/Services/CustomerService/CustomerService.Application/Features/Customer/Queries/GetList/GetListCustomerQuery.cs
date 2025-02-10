using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Persistance.Abstract.Repositories;
using MediatR;

namespace CustomerService.Application.Features.Customer.Queries.GetList;

public class GetListCustomerQuery: IRequest<Response<GetListResponse<GetListCustomerDto>>>
{
    public PageRequest PageRequest { get; set; }
    public DynamicQuery DynamicQuery { get; set; } 
    
    public GetListCustomerQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListCustomerQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }
    
    public class GetListCustomerQueryHandler
        : IRequestHandler<GetListCustomerQuery,
            Response<GetListResponse<GetListCustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService; 

        public GetListCustomerQueryHandler(ICustomerRepository customerRepository,
            IMapper mapper, IBaseService baseService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<Response<GetListResponse<GetListCustomerDto>>> Handle(
            GetListCustomerQuery request,
            CancellationToken cancellationToken
        )
        {
            Paginate<Domain.Entities.Customer> customer = await _customerRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking:false
            );
            
            GetListResponse<GetListCustomerDto> response = _mapper.Map<
                GetListResponse<GetListCustomerDto> >(customer);
            
            return _baseService.CreateSuccessResult<GetListResponse<GetListCustomerDto>>(response,
                InternalsConstants.Success);
        }
    }
    
    
    
}