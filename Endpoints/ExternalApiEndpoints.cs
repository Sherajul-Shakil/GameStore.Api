using System.Net.Http.Headers;
using System.Text.Json;

namespace GameStore.Api.Endpoints;

public static class ExternalApiEndpoints
{
	public static RouteGroupBuilder MapExternalApiEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/multipleApi")
					.RequireAuthorization();

		group.MapGet("/", async (HttpContext httpContext) =>
		{
			var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6IjU3NyIsInVzZXJuYW1lIjoiMDE3NTg4MjEzODIiLCJwb3NpdGlvbiI6IlRlY2huaWNhbCBBc3Npc3RhbnQiLCJpYXQiOjE3MTA5OTEwODcsImV4cCI6MTc0MjUyNzA4N30.jCkILTGaFCTxc_ydF9FL7JazkoDaK0r2FXGx3Cd_2pY";
			var apiUrls = new List<string>
			{
				// "http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-01&end_date=2024-10&report_type=1&division[]=50&district[]=81",
				// "http://dmtta.icddrb.org:82/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-01&end_date=2024-10&report_type=1&division[]=50&district[]=81"
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/monthly_trend_tb_cases_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_facility_wise_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_facility_wise_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/monthly_trend_tb_cases_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/summary_data_upazilla_wise_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81",
				"http://actb.icddrb.org/index.php/ExternalApi/summary_data_upazilla_wise_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81"
			};

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// Prepare the API call tasks
			var tasks = apiUrls.Select(async url =>
			{
				try
				{
					var response = await httpClient.GetAsync(url);
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var parsedJson = JsonSerializer.Deserialize<object>(content);
						return parsedJson ?? new { error = "Null response data", url };
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						return new { error = response.StatusCode, message = errorContent, url };
					}
				}
				catch (Exception ex)
				{
					return new { exception = ex.Message, url };
				}
			});

			// Await all tasks concurrently
			var formattedResponses = await Task.WhenAll(tasks);

			return Results.Ok(new
			{
				status = "Success",
				message = "Data fetched and formatted",
				data = formattedResponses
			});
		});

		return group;
	}
}
