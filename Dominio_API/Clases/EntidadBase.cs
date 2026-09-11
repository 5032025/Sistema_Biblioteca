using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public abstract class EntidadBase
    {

        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }


    }
}
