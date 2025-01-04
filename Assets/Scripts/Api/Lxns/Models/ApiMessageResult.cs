using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api.Lxns.Models
{
    [Serializable]
    public class ApiMessageResult<T>
    {
        public bool success;
        public int code;
        public T data;
    }
}
