using System;
using System.Linq;
using backend.Data;
using backend.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using backend.Models.Robots;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services
{
    public abstract class RobotServiceTask
    {
        // JsonSerializer configurations
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = false,
            IgnoreNullValues = true,
            WriteIndented = false
        };

        public readonly ILogger<RobotService> _logger;
        internal IServiceProvider Services { get; set; }
        private readonly HttpClient _client;
        public List<Robot> Hosts { get; set; }

        public RobotServiceTask(ILogger<RobotService> logger, IHttpClientFactory clientFactory,
            IServiceProvider services)
        {
            _logger = logger;
            Services = services;
            _client = clientFactory.CreateClient();
        }

        protected async Task LoadStatus(Robot host, ApplicationDbContext db)
        {
            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.SendAsync(HttpRequestMessage(host, "/status"));

                if (response.IsSuccessStatusCode)
                {
                    
                    await using var responseStream = await response.Content.ReadAsStreamAsync();
                    var updateRobot = await JsonSerializer.DeserializeAsync<Robot>(responseStream, _options);
                    
                    // Update existing values
                    updateRobot.Id = host.Id;
                    updateRobot.BasePath = host.BasePath;
                    updateRobot.Token = host.Token;
                    updateRobot.IsOnline = true;
                    updateRobot.VelocityId = host.VelocityId;
                    updateRobot.PositionId = host.PositionId;
                    
                    db.Robots.Update(updateRobot);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                _logger.LogCritical("The Robot may be is offline: No status could be loaded");
            }
        }


        protected async Task LoadMissions(Robot host, ApplicationDbContext db)
        {
            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.SendAsync(HttpRequestMessage(host, "/missions"));

                if (response.IsSuccessStatusCode)
                {
                    await using var responseStream = await response.Content.ReadAsStreamAsync();

                    var missions =
                        await JsonSerializer.DeserializeAsync<List<Mission>>(responseStream, _options);
                    foreach (var mission in missions)
                    {
                        mission.RobotId = host.Id;
                        if (!db.Missions.Any(m => m.Guid.Contains(mission.Guid) && m.Url.Equals(mission.Url)))
                        {
                            db.Missions.Update(mission);
                        }
                    }
                    await db.SaveChangesAsync();

                    foreach (var mission in db.Missions)
                    {
                        await GetMissionByUrl(mission.Url, host, db);
                    }

                    host.IsOnline = true;
                    db.Robots.Update(host);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                SetRobotOffline(host, db, e);
            }
        }

        private async Task GetMissionByUrl(string url, Robot host, ApplicationDbContext db)
        {
            try
            {
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.SendAsync(HttpRequestMessage(host, url));
                if (response.IsSuccessStatusCode)
                {
                    await using var respStream = await response.Content.ReadAsStreamAsync();
                    var mission = await JsonSerializer.DeserializeAsync<Mission>(respStream, _options);
                    Console.WriteLine("Tester..");
                    var currentMission = db.Missions.First(s => s.Guid.Equals(mission.Guid));
                    // Update in db
                    mission.Id = currentMission.Id;
                    db.Missions.Update(mission);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //
        // protected async Task PostToQueue(Robot host, ApplicationDbContext db)
        // {
        //     if (!host.IsOnline) return;
        //     var IsAvailible = db.MissionQueueRequests.Any(s => s.RobotId.Equals(host.Id));
        //     if (!IsAvailible)
        //     {
        //         return;
        //     }
        //
        //     var res = db.MissionQueueRequests.First(
        //         s => s.RobotId.Equals(host.Id));
        //     try
        //     {
        //         var json = JsonSerializer.Serialize(res, _options);
        //         var details = JsonConvert.DeserializeObject(json);
        //        // var client = new HttpClient();
        //         _client.DefaultRequestHeaders.Accept.Clear();
        //         _client.DefaultRequestHeaders.Authorization =
        //             new AuthenticationHeaderValue("Basic", host.Token);
        //         _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //         await _client.PostAsJsonAsync(host.BasePath + "/mission_queue", details);
        //
        //         var id = await db.MissionQueueRequests.FindAsync(res.Id);
        //         db.MissionQueueRequests.Remove(id);
        //         await db.SaveChangesAsync();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine();
        //     }
        // }
        //
        // public async Task GetQueue(Robot host, ApplicationDbContext db)
        // {
        //     if (!host.IsOnline) return;
        //     
        //     try
        //     {
        //         _client.DefaultRequestHeaders.Accept.Clear();
        //         var httpResponse = await _client.SendAsync(HttpRequestMessage(host, "/mission_queue"));
        //
        //         if (httpResponse.IsSuccessStatusCode)
        //         {
        //             await using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
        //             
        //             var response = await JsonSerializer.DeserializeAsync<List<MissionQueuesResponse>>(responseStream, _options);
        //
        //             foreach (var queuesResponse in response)
        //             {
        //                 queuesResponse.RobotId = host.Id;
        //                 var isAvailable = db.MissionQueuesResponse.Any(s => s.Id.Equals(queuesResponse.Id));
        //
        //                 if (isAvailable)
        //                     db.MissionQueuesResponse.Update(queuesResponse);
        //                 else
        //                 {
        //                     var missq = new MissionQueuesResponse
        //                     {
        //                         Robot = host,
        //                         RobotId = host.Id,
        //                         State = queuesResponse.State,
        //                         Url = queuesResponse.Url
        //                     };
        //
        //                     await db.MissionQueuesResponse.AddAsync(missq);
        //                 }
        //             }
        //             await db.SaveChangesAsync();
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogCritical("The Robot may be is offline");
        //     }
        // }

        private void SetRobotOffline(Robot host, ApplicationDbContext db, Exception e)
        {
            var robot = db.Robots.Find(host.Id);
            robot.IsOnline = false;
            db.Update(robot);
            db.SaveChanges();
            _logger.LogCritical("The Robot may be is offline: " + host.BasePath);
        }

        private static HttpRequestMessage HttpRequestMessage(Robot host, string path)
        {
            // This method will be injected by the caller with appropriated data ex: requestUrl and authorization
            var request = new HttpRequestMessage(HttpMethod.Get, host.BasePath + path);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization",
                $"Basic {host.Token}");
            return request;
        }
    }
}