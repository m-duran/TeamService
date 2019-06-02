
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;

namespace StatlerWaldorfCorp.TeamService
{
    [Route("/teams/{teamId}/[controller]")]
    public class MembersController : ControllerBase
    {
        ITeamRepository repository;

        public MembersController(ITeamRepository repo)
        {
            repository = repo;
        }

        [HttpGet]
        public virtual ActionResult<ICollection<Member>> GetMembers(Guid teamId)
        {
            Team team = repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(team.Members);
            }
        }

        [HttpGet]
        [Route("/teams/{teamId}/[controller]/{memberId}")]
        public virtual ActionResult<Member> GetMember(Guid teamId, Guid memberId)
        {
            Team team = repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            else
            {
                Member member = team.Members.Where(m => m.Id == memberId).FirstOrDefault();

                if (member == null)
                {
                    return NotFound();
                }
                else
                {
                    return member;
                }
            }
        }

        [HttpPut]
        [Route("/teams/{teamId}/[controller]/{memberId}")]
        public virtual IActionResult UpdateMember(Member updateMember, Guid teamId, Guid memberId)
        {
            Team team = repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            else
            {
                Member member = team.Members.Where(m => m.Id == memberId).FirstOrDefault();
                if (member == null)
                {
                    return NotFound();
                }
                else
                {
                    team.Members.Remove(member);
                    team.Members.Add(updateMember);
                    return Ok();
                }
            }
        }

        [HttpPost]
        public virtual IActionResult CreateMember(Member newMember, Guid teamId)
        {
            Team team = repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            else
            {
                team.Members.Add(newMember);
                var teamMember = new { TeamId = team.Id, MemberId = newMember.Id };
                return Created($"/teams/{teamMember.TeamId}/[controller]/{teamMember.MemberId}", teamMember);
            }
        }

        [HttpGet]
        [Route("/members/{memberId}/team")]
        public IActionResult GetTeamForMember(Guid memberId)
        {
            var teamId = GetTeamIdForMember(memberId);
            if (teamId != Guid.Empty)
            {
                return Ok(new { TeamId = teamId });
            }
            else
            {
                return NotFound();
            }
        }

        private Guid GetTeamIdForMember(Guid memberId)
        {
            foreach (var team in repository.List())
            {
                var member = team.Members.FirstOrDefault(m => m.Id == memberId);
                if (member != null)
                {
                    return team.Id;
                }
            }
            return Guid.Empty;
        }
    }
}