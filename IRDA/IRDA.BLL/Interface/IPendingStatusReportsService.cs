using IRDA.DAL;
using SharedModules;

namespace IRDA.BLL;

public interface IPendingStatusReportsService
{
    Task<IEnumerable<ClaimStatusReportDTO>> GetPendingStatusReports(int month,int year);
    Task<CommonOutput> AddPendingStatusReports(int month,int year);
}
