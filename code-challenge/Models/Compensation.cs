using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    public class Compensation
    {
        // Need to use a CompensationId as a primary key, as this is likely
        // cleaner than trying to use the Employee object. Need a key here since
        // we are persisting the data
        [System.ComponentModel.DataAnnotations.Key]
        public string CompensationId { get; set; }
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
        public virtual Employee Employee { get; set; }

    }
}
