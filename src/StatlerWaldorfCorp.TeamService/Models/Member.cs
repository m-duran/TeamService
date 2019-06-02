

using System;

namespace StatlerWaldorfCorp.TeamService.Models
{
    public class Member
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Member()
        {
        }

        public Member(Guid id)
        {
            Id = id;
        }

        public Member(string firstName, string lastName, Guid id)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return LastName;
        }
    }
}