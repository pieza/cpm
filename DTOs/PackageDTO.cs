using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cpm.DTOs
{
    internal class PackageDTO
    {
        public required string Id { get; set; }
        public required string Identifier { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Owner { get; set; }
        public required string Repo { get; set; }
    }
}
