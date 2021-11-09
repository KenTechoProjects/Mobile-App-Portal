using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YellowStone.Models
{
    public class Attachment : Base
    {
        public string AccountNumber { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

    }
}
