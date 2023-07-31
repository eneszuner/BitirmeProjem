using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    //generic constraint generic=kısıt demek
    //class: referenas tip olabilir demek
    //IEntity :IEntity olabilir veya impelemente eden bir nesne olabilir
    //new(): new'lenebilir olmalı
    public interface IEntityRepository<T> where T:class, IEntity,new()
    {
        //Expression= yani sen bu yapıyla gidipte categoriye göre getir ürünün fiyatına göre getir ayrı ayrı metotlar yazman gerekmiycek
        //filter=null filtre vermiyede bilirsin demek 
        List<T> GetAll(Expression<Func<T,bool>> filter=null);//Hepsini Getir
        //filtre vermemişse tüm datayı istio demektir
        T Get(Expression<Func<T, bool>> filter);//filter zorunlu 
        //filtreleyip vericek
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
