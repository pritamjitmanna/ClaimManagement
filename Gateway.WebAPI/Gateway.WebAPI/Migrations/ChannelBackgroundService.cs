using System.Threading.Channels;

namespace Gateway.WebAPI.Notifications;

public class ChannelBackgroundService:BackgroundService{
    private readonly Channel<NotificationModel> _channel;
    private readonly ChannelWriter<NotificationModel> _channelWriter;
    private readonly ChannelReader<NotificationModel> _channelReader;
    private readonly AuthDBContext _dbContext;

    public ChannelBackgroundService(AuthDBContext dbContext)
    {
        _dbContext = dbContext;
        _channel = Channel.CreateUnbounded<NotificationModel>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        });
        _channelWriter = _channel.Writer;
        _channelReader = _channel.Reader;
    }

    public async Task<bool> QueueModelAsync(NotificationModel model,CancellationToken cancellationToken=default)
    {
        try
        {
            await _channelWriter.WriteAsync(model, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception (logging, etc.)
            Console.WriteLine($"Error queuing model: {ex.Message}");
            return false;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var model in _channelReader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await AddNotificationToDatabaseAsync(model, stoppingToken);
            }
            catch (Exception ex)
            {
                // Handle exception (logging, etc.)
                // Optionally, you can log the error or take other actions.
                throw;
            }
        }
    }

    private async Task AddNotificationToDatabaseAsync(NotificationModel model,CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.NotificationModels.AddAsync(model, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Handle exception (logging, etc.)
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _channelWriter.Complete();
        await base.StopAsync(cancellationToken);
    }
}