using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(T data, string message) : base(data, false, message)//data ver mesaj ver
        {

        }

        public ErrorDataResult(T data) : base(data, false)//ister sadece data ver
        {

        }


        //bu ikisni fazla kullanmayız
        public ErrorDataResult(string message) : base(default, false, message)//ister sadece mesaj ver
        {

        }

        public ErrorDataResult() : base(default, false)//istersen hiç birşey verme
        {

        }

    }
}
