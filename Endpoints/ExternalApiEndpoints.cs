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
			var apiUrls = new Dictionary<string, string>
			{
				// { "summary_data", "http://actb.icddrb.org/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				// { "monthly_trend", "http://actb.icddrb.org/index.php/ExternalApi/monthly_trend_tb_cases_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				// { "bar_chart_quarterly", "http://actb.icddrb.org/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				// Add more URLs as needed
				{ "summary_data_2023_09_to_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_summary_data_2023_09_to_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "summary_data_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_summary_data_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/summary_data_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "bar_chart_quarterly_2023_09_to_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_bar_chart_quarterly_2023_09_to_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/bar_chart_quarterly_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "monthly_trend_tb_cases_2023_09_to_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/monthly_trend_tb_cases_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_monthly_trend_tb_cases_2023_09_to_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/monthly_trend_tb_cases_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "summary_data_facility_wise_2023_09_to_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_facility_wise_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "summary_data_facility_wise_2024_06", "http://dmtta.icddrb.org:82/index.php/ExternalApi/summary_data_facility_wise_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_summary_data_upazilla_wise_2023_09_to_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/summary_data_upazilla_wise_rdlc?internal=yes&adult_child=1&start_date=2023-09&end_date=2024-06&report_type=1&division[]=50&district[]=81" },
				{ "actb_summary_data_upazilla_wise_2024_06", "http://actb.icddrb.org/index.php/ExternalApi/summary_data_upazilla_wise_rdlc?internal=yes&adult_child=1&start_date=2024-06&end_date=2024-06&report_type=1&division[]=50&district[]=81" }

			};

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var formattedResponses = new Dictionary<string, object>();

			var tasks = apiUrls.Select(async kvp =>
			{
				var key = kvp.Key;
				var url = kvp.Value;

				try
				{
					var response = await httpClient.GetAsync(url);
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var parsedJson = JsonSerializer.Deserialize<object>(content);
						formattedResponses[key] = parsedJson ?? new { error = "Null response data", url };
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						formattedResponses[key] = new
						{
							error = response.StatusCode.ToString(),
							 //message = errorContent,
							url
						};
					}
				}
				catch (Exception ex)
				{
					formattedResponses[key] = new
					{
						exception = ex.Message,
						url
					};
				}
			});

			// Await all tasks concurrently
			await Task.WhenAll(tasks);

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
