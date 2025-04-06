using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jacmazon_ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleServiceController> _logger;

        public GoogleServiceController(IConfiguration configuration, ILogger<GoogleServiceController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarEvents()
        {
            try
            {
                // 從 credentials.json 讀取憑證
                string credPath = "credentials.json";
                var credential = GoogleCredential.FromFile(credPath)
                    .CreateScoped(CalendarService.Scope.CalendarReadonly);

                // 建立 Calendar API 服務
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Calendar API .NET Core"
                });

                // 取得事件列表
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMinDateTimeOffset = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                Events events = await request.ExecuteAsync();

                return Ok(events.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing Google Calendar API");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
