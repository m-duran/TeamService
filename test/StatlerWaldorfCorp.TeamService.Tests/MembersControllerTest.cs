using Xunit;
using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using StatlerWaldorfCorp.TeamService.Persistence;

[assembly: CollectionBehavior(MaxParallelThreads = 1)]

namespace StatlerWaldorfCorp.TeamService
{
    public class MembersControllerTest
    {
        [Fact]
        public void CreateMemberAddsTeamToList()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestController", teamId);
            repository.Add(team);

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);
            Assert.True(team.Members.Contains(newMember));
        }

        [Fact]
        public void CreateMembertoNonexistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            var result = controller.CreateMember(newMember, teamId);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void GetExistingMemberReturnsMember()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            controller.CreateMember(newMember, teamId);

            Member member = controller.GetMember(teamId, memberId).Value;
            Assert.Equal(member.Id, newMember.Id);
        }

        [Fact]
        public void GetMembersReturnsMembers()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            Guid firstMemberId = Guid.NewGuid();
            Member newMember = new Member(firstMemberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            controller.CreateMember(newMember, teamId);

            Guid secondMemberId = Guid.NewGuid();
            newMember = new Member(secondMemberId);
            newMember.FirstName = "John";
            newMember.LastName = "Doe";
            controller.CreateMember(newMember, teamId);

            var result = controller.GetMembers(teamId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var members = Assert.IsAssignableFrom<ICollection<Member>>(okResult.Value);
            Assert.Equal(2, members.Count());
            Assert.NotNull(members.Where(m => m.Id == firstMemberId).First());
            Assert.NotNull(members.Where(m => m.Id == secondMemberId).First());
        }

        [Fact]
        public void GetMembersForNewTeamIsEmpty()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var result = controller.GetMembers(teamId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var members = Assert.IsAssignableFrom<ICollection<Member>>(okResult.Value);
            Assert.Empty(members);
        }

        [Fact]
        public void GetMembersForNonExistantTeamReturnNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            var result = controller.GetMembers(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetNonExistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            var result = controller.GetMember(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetNonExistantMemberReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var result = controller.GetMember(teamId, Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void UpdateMemberOverwrites()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);

            Member updatedMember = new Member(memberId);
            updatedMember.FirstName = "Bob";
            updatedMember.LastName = "Jones";
            controller.UpdateMember(updatedMember, teamId, memberId);

            team = repository.Get(teamId);
            Member testMember = team.Members.Where(m => m.Id == memberId).First();

            Assert.Equal("Bob", testMember.FirstName);
            Assert.Equal("Jones", testMember.LastName);
        }

        [Fact]
        public void UpdateMembertoNonexistantMemberReturnsNoMatch()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestController", teamId);
            repository.Add(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            controller.CreateMember(newMember, teamId);

            Guid nonMatchedGuid = Guid.NewGuid();
            Member updatedMember = new Member(nonMatchedGuid);
            updatedMember.FirstName = "Bob";
            var result = controller.UpdateMember(updatedMember, teamId, nonMatchedGuid);

            Assert.True(result is NotFoundResult);
        }
    }
}