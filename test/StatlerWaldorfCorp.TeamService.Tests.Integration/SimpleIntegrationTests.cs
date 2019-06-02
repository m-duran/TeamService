using Xunit;
using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

namespace StatlerWaldorfCorp.TeamService.Tests.Integration
{
    public class SimpleIntegrationTests
    {
        private readonly TestServer testServer;
        private readonly HttpClient testClient;
        private readonly Team teamZombie;

        public SimpleIntegrationTests()
        {
            testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            testClient = testServer.CreateClient();

            teamZombie = new Team()
            {
                Id = Guid.NewGuid(),
                Name = "Zombie"
            };
        }
        [Fact]
        public async void TestTeamPostAndGet()
        {
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(teamZombie),
                UnicodeEncoding.UTF8,
                "application/json"
            );

            HttpResponseMessage postResponse = await testClient.PostAsync(
                "/teams",
                stringContent);
            postResponse.EnsureSuccessStatusCode();

            var getResponse = await testClient.GetAsync("/teams");
            getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();
            List<Team> teams = JsonConvert.DeserializeObject<List<Team>>(raw);
            Assert.Single(teams);
            Assert.Equal("Zombie", teams[0].Name);
            Assert.Equal(teamZombie.Id, teams[0].Id);
        }
    }
}