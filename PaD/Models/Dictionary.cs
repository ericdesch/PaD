using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class Dictionary
    {
        public int DictionaryId { get; set; }

        [Required]
        [StringLength(64)]
        public string Word { get; set; }
    }
}