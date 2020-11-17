using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Mapping
{
    [DelimitedRecord("|")]
    public class Type
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }
}
