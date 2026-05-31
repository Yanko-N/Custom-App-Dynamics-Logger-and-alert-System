using System.Net.Http.Json;
using System.Text.Json;

// -------------------------------------------------------
// AppLogger Demo — sets up a service, hook, alert, then
// fires enough ERROR logs to trigger the alert.
//
// 1. Start the API (AppLoggerDynamic project).
// 2. Open Swagger at http://localhost:5045/swagger, create
//    an Account and then an ApiKey for that account.
// 3. Paste the RawKey below and run this app.
// -------------------------------------------------------

const string BaseUrl = "http://localhost:5045";
const string ApiKey  = "PASTE_YOUR_RAW_KEY_HERE";  // <-- replace this

var http = new HttpClient();
http.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

Console.WriteLine("=== AppLogger Demo ===\n");

// --- 1. Create a service ---
Console.WriteLine("[1] Creating service...");
var serviceRes = await http.PostAsJsonAsync($"{BaseUrl}/api/service", new
{
    name        = "demo-service",
    environment = "local",
    version     = "1.0.0"
});
serviceRes.EnsureSuccessStatusCode();
var serviceDoc = await serviceRes.Content.ReadFromJsonAsync<JsonElement>();
int serviceId = serviceDoc.GetProperty("id").GetInt32();
Console.WriteLine($"    Service ID: {serviceId}");

// --- 2. Create a hook ---
// Use https://webhook.site to get a free test URL, or any HTTP endpoint.
Console.Write("\nPaste a webhook URL (e.g. https://webhook.site/your-uuid): ");
string hookUrl = Console.ReadLine()?.Trim() ?? "https://webhook.site/test";

Console.WriteLine("[2] Creating hook...");
var hookRes = await http.PostAsJsonAsync($"{BaseUrl}/api/hook", new
{
    serviceId,
    name   = "demo-hook",
    url    = hookUrl,
    secret = (string?)null
});
hookRes.EnsureSuccessStatusCode();
var hookDoc = await hookRes.Content.ReadFromJsonAsync<JsonElement>();
int hookId = hookDoc.GetProperty("id").GetInt32();
Console.WriteLine($"    Hook ID: {hookId}  ->  {hookUrl}");

// --- 3. Create an alert ---
// Fires when ERROR logs exceed 2 within 30 seconds.
Console.WriteLine("\n[3] Creating alert (ERROR > 2 in 30 s)...");
var alertRes = await http.PostAsJsonAsync($"{BaseUrl}/api/alert", new
{
    serviceId,
    name           = "demo-alert",
    level          = "ERROR",
    condition      = "GreaterThan",
    thresholdValue = 2,
    windowSeconds  = 30
});
alertRes.EnsureSuccessStatusCode();
var alertDoc = await alertRes.Content.ReadFromJsonAsync<JsonElement>();
int alertId = alertDoc.GetProperty("id").GetInt32();
Console.WriteLine($"    Alert ID: {alertId}");

// --- 4. Ingest ERROR logs to trigger the alert ---
Console.WriteLine("\n[4] Ingesting 3 ERROR logs...");
for (int i = 1; i <= 3; i++)
{
    var logRes = await http.PostAsJsonAsync($"{BaseUrl}/api/log", new
    {
        serviceId,
        level      = "ERROR",
        message    = $"Demo error #{i} — something went wrong",
        stackTrace = (string?)null,
        traceId    = Guid.NewGuid().ToString()
    });
    logRes.EnsureSuccessStatusCode();
    var logDoc = await logRes.Content.ReadFromJsonAsync<JsonElement>();
    Console.WriteLine($"    Log {i} ingested. ID: {logDoc.GetProperty("id").GetRawText()}");
    await Task.Delay(300);
}

Console.WriteLine("\n=== Done ===");
Console.WriteLine("Check your webhook URL for the fired alert payload.");
Console.WriteLine($"Swagger: {BaseUrl}/swagger");
