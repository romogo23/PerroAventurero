using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }

    }
}
