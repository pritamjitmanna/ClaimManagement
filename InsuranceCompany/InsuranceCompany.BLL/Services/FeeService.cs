using AutoMapper;
using InsuranceCompany.DAL;

namespace InsuranceCompany.BLL;

public class FeeService : IFeeService
{

    private readonly IFee _feeRepository;
    private readonly IMapper _mapper;
    //private readonly ILog _logger;


    public FeeService(IFee feeRepository, IMapper mapper)
    {
        _feeRepository = feeRepository;
        _mapper = mapper;
        //_logger = logger;
    }
    public async Task<FeeDTO?> GetFeesByEstimatedLoss(int estimatedLoss)
    {

        FeeDTO? fees;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
        try
        {

            Fee? fee = await _feeRepository.GetFeesByEstimatedLoss(estimatedLoss);

            fees = _mapper.Map<FeeDTO>(fee);

        }
        catch (Exception ex)
        {
            //log
           // _logger.Error("Ran with this problem " + ex.Message + " in FeeService");
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        return fees;
    }
}

