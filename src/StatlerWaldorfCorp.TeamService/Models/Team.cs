

using System;
using System.Collections.Generic;

namespace StatlerWaldorfCorp.TeamService.Models
{
    public class Team
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ICollection<Member> Members { get; set; }
        public Team()
        {
            Members = new List<Member>();
        }

        public Team(string name) : this()
        {
            Name = name;
        }

        public Team(string name, Guid id) : this(name)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}