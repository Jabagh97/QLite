using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    
    public class MobilResponseDto<T> where T : class
    {
        public T Item { get; set; }
        public List<T> ListItems { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
    }
}
