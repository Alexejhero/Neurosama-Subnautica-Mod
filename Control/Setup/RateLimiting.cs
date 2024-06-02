using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Control.Setup;

public static class RateLimiting
{
    public const int GlobalAnonymousMaxTokens = 10;
    public const int GlobalAnonymousReplenishTokens = 2;
    public const string AnonymousPartitionKey = "$AnonymousSharedBucket";

    public const int GlobalUserMaxTokens = 50;
    public const int GlobalUserReplenishTokens = 10;

    public const string ApiPolicy = "API";

    public static readonly TokenBucketRateLimiterOptions GlobalAnonymousShortBucketOptions = new()
    {
        QueueLimit = 1,
        QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
        TokenLimit = GlobalAnonymousMaxTokens,
        TokensPerPeriod = GlobalAnonymousReplenishTokens
    };
    public static readonly TokenBucketRateLimiterOptions GlobalUserShortBucketOptions = new()
    {
        QueueLimit = 1,
        QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
        TokenLimit = GlobalUserMaxTokens,
        TokensPerPeriod = GlobalUserReplenishTokens
    };

    public static void SetRejectionResponse(this RateLimiterOptions op)
    {
        op.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        op.OnRejected = (ctx, ct) =>
        {
            HttpResponse response = ctx.HttpContext.Response;
            if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
            {
                response.Headers.RetryAfter = ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                response.WriteAsync($"You are being rate limited, please try again in {retryAfter}", ct);
            }
            else
            {
                response.WriteAsync("You are being rate limited, please wait", ct);
            }
            return ValueTask.CompletedTask;
        };
    }

    public static void SetGlobalLimit(this RateLimiterOptions op)
    {
        PartitionedRateLimiter<HttpContext> shortTermLimit = PartitionedRateLimiter.Create<HttpContext, (string, IPAddress)>(ctx =>
        {
            return RateLimitPartition.GetTokenBucketLimiter(
                (ctx.User.Identity?.Name ?? AnonymousPartitionKey, ctx.Connection.RemoteIpAddress!),
                k => k.Item1 == AnonymousPartitionKey ? GlobalAnonymousShortBucketOptions : GlobalUserShortBucketOptions
            );
        });
        //op.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        //    PartitionedRateLimiter.Create<...>(...),
        //    PartitionedRateLimiter.Create<...>(...),
        //);
        op.GlobalLimiter = shortTermLimit;
    }

    public static void ConfigureRateLimiting(this RateLimiterOptions op)
    {
        op.SetRejectionResponse();
        op.SetGlobalLimit();
    }
}
