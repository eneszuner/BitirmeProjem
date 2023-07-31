 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    //Bu aslında Temel voidler için başlangıç
    public interface IResult
    {
        bool Success { get; }//Get demek sadece okuna bilir demek //Başarılı mı ? Başarısız mı ?
        string Message { get; }//yapmaya çalıştığın işem true veya ürün eklendi gibi bir bilgilendirme ile bu sürecin içerisinde bir yönlendirme yapıcağız
    }
}
