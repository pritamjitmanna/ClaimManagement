using SharedModules;

namespace IRDA.DAL;

public interface IPendingStatusReports
{
    Task<PendingStatusReports?> PendingStatusReportsOnMonthAndYear(Stages stage,int month, int year);

    Task<CommonOutput> UpdatePendingStatusReportsData(PendingStatusReports status);

    Task<CommonOutput> AddPendingStatusReportsData(PendingStatusReports status);
}
